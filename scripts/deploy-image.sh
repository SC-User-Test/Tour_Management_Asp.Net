#!/bin/bash
set -e
set -o pipefail

echo "===================================="
echo "AWS EKS Deployment Script"
echo "Tour Management Application"
echo "===================================="
echo ""

# Validate prerequisites
command -v aws >/dev/null 2>&1 || { echo "ERROR: AWS CLI is not installed"; exit 1; }
command -v kubectl >/dev/null 2>&1 || { echo "ERROR: kubectl is not installed"; exit 1; }

# Collect deployment parameters
echo "AWS EKS Configuration"
echo "---------------------"
read -p "Enter AWS Region (e.g., us-east-1): " AWS_REGION
read -p "Enter EKS Cluster Name: " CLUSTER_NAME

if [ -z "$AWS_REGION" ] || [ -z "$CLUSTER_NAME" ]; then
    echo "ERROR: AWS Region and Cluster Name are required"
    exit 1
fi

echo ""
echo "Docker Image Configuration"
echo "--------------------------"
read -p "Enter Docker Image URI (with tag): " IMAGE_URI

if [ -z "$IMAGE_URI" ]; then
    echo "ERROR: Docker Image URI is required"
    exit 1
fi

echo ""
echo "Application Configuration"
echo "-------------------------"
read -p "Enter Database Connection String: " DB_CONNECTION_STRING

if [ -z "$DB_CONNECTION_STRING" ]; then
    echo "WARNING: No database connection string provided. Using default placeholder."
    DB_CONNECTION_STRING="Data Source=sql-server;Initial Catalog=tourdb;User ID=sa;Password=YourPassword123;MultipleActiveResultSets=true"
fi

echo ""
echo "===================================="
echo "Deployment Summary"
echo "===================================="
echo "AWS Region: $AWS_REGION"
echo "EKS Cluster: $CLUSTER_NAME"
echo "Image: $IMAGE_URI"
echo "Namespace: tour-management"
echo ""

read -p "Proceed with deployment? (y/n): " CONFIRM
if [ "$CONFIRM" != "y" ] && [ "$CONFIRM" != "Y" ]; then
    echo "Deployment cancelled."
    exit 0
fi

echo ""
echo "Step 1: Configuring kubectl for EKS..."
aws eks update-kubeconfig --region "$AWS_REGION" --name "$CLUSTER_NAME"

if [ $? -ne 0 ]; then
    echo "ERROR: Failed to configure kubectl for EKS cluster"
    exit 1
fi

echo "Verifying cluster connectivity..."
kubectl cluster-info || {
    echo "ERROR: Cannot connect to Kubernetes cluster"
    exit 1
}

echo ""
echo "Step 2: Creating temporary manifest directory..."
TEMP_DIR=$(mktemp -d)
trap "rm -rf $TEMP_DIR" EXIT

cp -r kubernetes/* "$TEMP_DIR/"

echo "Step 3: Updating Kubernetes manifests..."
find "$TEMP_DIR" -type f -name "*.yaml" -exec sed -i.bak 's|{{IMAGE_URI}}|'"$IMAGE_URI"'|g' {} \;
find "$TEMP_DIR" -type f -name "*.yaml" -exec sed -i.bak 's|{{DB_CONNECTION_STRING}}|'"$DB_CONNECTION_STRING"'|g' {} \;
find "$TEMP_DIR" -name "*.bak" -delete

echo ""
echo "Step 4: Applying Kubernetes manifests..."

echo "Creating namespace..."
kubectl apply -f "$TEMP_DIR/namespace.yaml"

echo "Deploying application..."
kubectl apply -f "$TEMP_DIR/deployment.yaml"

echo "Creating service..."
kubectl apply -f "$TEMP_DIR/service.yaml"

echo "Creating ingress..."
kubectl apply -f "$TEMP_DIR/ingress.yaml"

echo ""
echo "Step 5: Waiting for deployment rollout..."
kubectl rollout status deployment/tour-management -n tour-management --timeout=5m

if [ $? -ne 0 ]; then
    echo "WARNING: Deployment rollout did not complete successfully"
    echo "Check pod status with: kubectl get pods -n tour-management"
fi

echo ""
echo "Step 6: Verifying deployment..."
echo ""
echo "Pods:"
kubectl get pods -n tour-management

echo ""
echo "Services:"
kubectl get svc -n tour-management

echo ""
echo "Ingress:"
kubectl get ingress -n tour-management

echo ""
INGRESS_HOST=$(kubectl get ingress tour-management-ingress -n tour-management -o jsonpath='{.status.loadBalancer.ingress[0].hostname}' 2>/dev/null || echo "pending")

echo "===================================="
echo "Deployment Completed Successfully!"
echo "===================================="
echo ""
if [ "$INGRESS_HOST" != "pending" ] && [ -n "$INGRESS_HOST" ]; then
    echo "Application URL: http://$INGRESS_HOST"
else
    echo "Ingress is being provisioned. Check status with:"
    echo "kubectl get ingress -n tour-management"
fi
echo ""
echo "Useful commands:"
echo "  View logs: kubectl logs -l app=tour-management -n tour-management"
echo "  Scale deployment: kubectl scale deployment/tour-management --replicas=3 -n tour-management"
echo "  Delete deployment: kubectl delete namespace tour-management"
echo ""