@echo off
setlocal enabledelayedexpansion

echo ====================================
echo Docker Build and Push Script
echo ====================================
echo.

set PROJECT_NAME=tour-management
set DOCKERFILE_PATH=DotNetFrameworkProject_CE040_CE087\Tour_Management\Dockerfile
set BUILD_CONTEXT=.\DotNetFrameworkProject_CE040_CE087\Tour_Management

for /f "delims=" %%a in ('powershell -Command "'%PROJECT_NAME%'.ToLower() -replace '[^a-z0-9]+','-' -replace '^-+','' -replace '-+$',''"') do set IMAGE_NAME=%%a

echo Select Docker Registry
echo 1. AWS ECR (Elastic Container Registry)
echo 2. Docker Hub
set /p REGISTRY_CHOICE="Enter choice (1 or 2): "

if "!REGISTRY_CHOICE!"=="1" (
    echo.
    echo AWS ECR Configuration
    echo ---------------------
    set /p AWS_REGION="Enter AWS Region (e.g., us-east-1): "
    set /p AWS_ACCOUNT_ID="Enter AWS Account ID: "
    set /p ECR_REPO="Enter ECR Repository Name (default: !IMAGE_NAME!): "
    if "!ECR_REPO!"=="" set ECR_REPO=!IMAGE_NAME!
    
    set REGISTRY_URL=!AWS_ACCOUNT_ID!.dkr.ecr.!AWS_REGION!.amazonaws.com
    
    echo.
    echo Authenticating with AWS ECR...
    aws ecr get-login-password --region !AWS_REGION! | docker login --username AWS --password-stdin !REGISTRY_URL!
    if !ERRORLEVEL! neq 0 (
        echo ERROR: ECR authentication failed
        exit /b 1
    )
    
    echo Checking if ECR repository exists...
    aws ecr describe-repositories --repository-names !ECR_REPO! --region !AWS_REGION! >nul 2>&1
    if !ERRORLEVEL! neq 0 (
        echo Creating ECR repository: !ECR_REPO!
        aws ecr create-repository --repository-name !ECR_REPO! --region !AWS_REGION!
    )
    
    set FULL_IMAGE_NAME=!REGISTRY_URL!/!ECR_REPO!
    
) else if "!REGISTRY_CHOICE!"=="2" (
    echo.
    echo Docker Hub Configuration
    echo ------------------------
    set /p DOCKER_USERNAME="Enter Docker Hub username: "
    set /p DOCKER_PASSWORD="Enter Docker Hub password: "
    
    echo Authenticating with Docker Hub...
    echo !DOCKER_PASSWORD! | docker login --username !DOCKER_USERNAME! --password-stdin
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Docker Hub authentication failed
        exit /b 1
    )
    
    set FULL_IMAGE_NAME=!DOCKER_USERNAME!/!IMAGE_NAME!
) else (
    echo Invalid choice. Exiting.
    exit /b 1
)

echo.
set /p IMAGE_TAG="Enter image tag (default: latest): "
if "!IMAGE_TAG!"=="" set IMAGE_TAG=latest
for /f "delims=" %%a in ('powershell -Command "'!IMAGE_TAG!'.ToLower() -replace '[^a-z0-9.-]+','-' -replace '^-+','' -replace '-+$',''"') do set IMAGE_TAG=%%a

set FINAL_IMAGE=!FULL_IMAGE_NAME!:!IMAGE_TAG!

echo.
echo ====================================
echo Build Configuration
echo ====================================
echo Image Name: !FINAL_IMAGE!
echo Dockerfile: !DOCKERFILE_PATH!
echo Context: !BUILD_CONTEXT!
echo.

set /p CONFIRM="Proceed with build? (y/n): "
if /i not "!CONFIRM!"=="y" (
    echo Build cancelled.
    exit /b 0
)

echo.
echo Building Docker image...
docker build -f "!DOCKERFILE_PATH!" -t "!FINAL_IMAGE!" "!BUILD_CONTEXT!"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker build failed
    exit /b 1
)

echo.
echo Pushing image to registry...
docker push "!FINAL_IMAGE!"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker push failed
    exit /b 1
)

echo.
echo ====================================
echo Build and Push Completed Successfully!
echo ====================================
echo Image: !FINAL_IMAGE!
echo.
echo To deploy this image to Kubernetes:
echo .\scripts\deploy-image.bat
echo.

endlocal