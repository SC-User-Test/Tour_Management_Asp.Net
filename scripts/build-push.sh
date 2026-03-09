#!/bin/bash
set -e

echo "===================================="
echo "Docker Build and Push Script"
echo "===================================="
echo ""

# Project configuration
PROJECT_NAME="tour-management"
DOCKERFILE_PATH="DotNetFrameworkProject_CE040_CE087/Tour_Management/Dockerfile"
BUILD_CONTEXT="./DotNetFrameworkProject_CE040_CE087/Tour_Management"

# Sanitize image name
IMAGE_NAME=$(echo "$PROJECT_NAME" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9' '-' | sed 's/^-*//;s/-*$//')

echo "Select Docker Registry:"
echo "1. AWS ECR (Elastic Container Registry)"
echo "2. Docker Hub"
read -p "Enter choice (1 or 2): " REGISTRY_CHOICE

if [ "$REGISTRY_CHOICE" = "1" ]; then
    echo ""
    echo "AWS ECR Configuration"
    echo "---------------------"
    read -p "Enter AWS Region (e.g., us-east-1): " AWS_REGION
    read -p "Enter AWS Account ID: " AWS_ACCOUNT_ID
    read -p "Enter ECR Repository Name (default: $IMAGE_NAME): " ECR_REPO
    ECR_REPO=${ECR_REPO:-$IMAGE_NAME}
    
    REGISTRY_URL="$AWS_ACCOUNT_ID.dkr.ecr.$AWS_REGION.amazonaws.com"
    
    echo ""
    echo "Authenticating with AWS ECR..."
    aws ecr get-login-password --region "$AWS_REGION" | docker login --username AWS --password-stdin "$REGISTRY_URL"
    
    if [ $? -ne 0 ]; then
        echo "ERROR: ECR authentication failed"
        exit 1
    fi
    
    echo "Checking if ECR repository exists..."
    aws ecr describe-repositories --repository-names "$ECR_REPO" --region "$AWS_REGION" >/dev/null 2>&1 || {
        echo "Creating ECR repository: $ECR_REPO"
        aws ecr create-repository --repository-name "$ECR_REPO" --region "$AWS_REGION"
    }
    
    FULL_IMAGE_NAME="$REGISTRY_URL/$ECR_REPO"
    
elif [ "$REGISTRY_CHOICE" = "2" ]; then
    echo ""
    echo "Docker Hub Configuration"
    echo "------------------------"
    read -p "Enter Docker Hub username: " DOCKER_USERNAME
    read -sp "Enter Docker Hub password: " DOCKER_PASSWORD
    echo ""
    
    echo "Authenticating with Docker Hub..."
    echo "$DOCKER_PASSWORD" | docker login --username "$DOCKER_USERNAME" --password-stdin
    
    if [ $? -ne 0 ]; then
        echo "ERROR: Docker Hub authentication failed"
        exit 1
    fi
    
    FULL_IMAGE_NAME="$DOCKER_USERNAME/$IMAGE_NAME"
else
    echo "Invalid choice. Exiting."
    exit 1
fi

echo ""
read -p "Enter image tag (default: latest): " IMAGE_TAG
IMAGE_TAG=${IMAGE_TAG:-latest}
IMAGE_TAG=$(echo "$IMAGE_TAG" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9.-' '-' | sed 's/^-*//;s/-*$//')

FINAL_IMAGE="$FULL_IMAGE_NAME:$IMAGE_TAG"

echo ""
echo "===================================="
echo "Build Configuration"
echo "===================================="
echo "Image Name: $FINAL_IMAGE"
echo "Dockerfile: $DOCKERFILE_PATH"
echo "Context: $BUILD_CONTEXT"
echo ""

read -p "Proceed with build? (y/n): " CONFIRM
if [ "$CONFIRM" != "y" ] && [ "$CONFIRM" != "Y" ]; then
    echo "Build cancelled."
    exit 0
fi

echo ""
echo "Building Docker image..."
docker build -f "$DOCKERFILE_PATH" -t "$FINAL_IMAGE" "$BUILD_CONTEXT"

if [ $? -ne 0 ]; then
    echo "ERROR: Docker build failed"
    exit 1
fi

echo ""
echo "Pushing image to registry..."
docker push "$FINAL_IMAGE"

if [ $? -ne 0 ]; then
    echo "ERROR: Docker push failed"
    exit 1
fi

echo ""
echo "===================================="
echo "Build and Push Completed Successfully!"
echo "===================================="
echo "Image: $FINAL_IMAGE"
echo ""
echo "To deploy this image to Kubernetes:"
echo "./scripts/deploy-image.sh"
echo ""