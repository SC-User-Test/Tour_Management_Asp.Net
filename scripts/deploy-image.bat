@echo off
setlocal enabledelayedexpansion

echo ====================================
echo AWS EKS Deployment Script
echo Tour Management Application
echo ====================================
echo.

where aws >nul 2>&1
if !ERRORLEVEL! neq 0 (
    echo ERROR: AWS CLI is not installed
    exit /b 1
)

where kubectl >nul 2>&1
if !ERRORLEVEL! neq 0 (
    echo ERROR: kubectl is not installed
    exit /b 1
)

echo AWS EKS Configuration
echo ---------------------
set /p AWS_REGION="Enter AWS Region (e.g., us-east-1): "
set /p CLUSTER_NAME="Enter EKS Cluster Name: "

if "!AWS_REGION!"=="" (
    echo ERROR: AWS Region is required
    exit /b 1
)
if "!CLUSTER_NAME!"=="" (
    echo ERROR: Cluster Name is required
    exit /b 1
)

echo.
echo Docker Image Configuration
echo --------------------------
set /p IMAGE_URI="Enter Docker Image URI (with tag): "

if "!IMAGE_URI!"=="" (
    echo ERROR: Docker Image URI is required
    exit /b 1
)

echo.
echo Application Configuration
echo -------------------------
set /p DB_CONNECTION_STRING="Enter Database Connection String: "

if "!DB_CONNECTION_STRING!"=="" (
    echo WARNING: No database connection string provided. Using default placeholder.
    set DB_CONNECTION_STRING=Data Source=sql-server;Initial Catalog=tourdb;User ID=sa;Password=YourPassword123;MultipleActiveResultSets=true
)

echo.
echo ====================================
echo Deployment Summary
echo ====================================
echo AWS Region: !AWS_REGION!
echo EKS Cluster: !CLUSTER_NAME!
echo Image: !IMAGE_URI!
echo Namespace: tour-management
echo.

set /p CONFIRM="Proceed with deployment? (y/n): "
if /i not "!CONFIRM!"=="y" (
    echo Deployment cancelled.
    exit /b 0
)

echo.
echo Step 1: Configuring kubectl for EKS...
aws eks update-kubeconfig --region !AWS_REGION! --name !CLUSTER_NAME!
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to configure kubectl for EKS cluster
    exit /b 1
)

echo Verifying cluster connectivity...
kubectl cluster-info
if !ERRORLEVEL! neq 0 (
    echo ERROR: Cannot connect to Kubernetes cluster
    exit /b 1
)

echo.
echo Step 2: Creating temporary manifest directory...
set TEMP_DIR=%TEMP%\k8s-deploy-%RANDOM%
mkdir "!TEMP_DIR!"

xcopy /E /I /Q kubernetes\*.* "!TEMP_DIR!\"

echo Step 3: Updating Kubernetes manifests...
for %%f in (!TEMP_DIR!\*.yaml) do (
    powershell -Command "(Get-Content '%%f') -replace '{{IMAGE_URI}}', '!IMAGE_URI!' | Set-Content '%%f'"
    powershell -Command "(Get-Content '%%f') -replace '{{DB_CONNECTION_STRING}}', '!DB_CONNECTION_STRING!' | Set-Content '%%f'"
)

echo.
echo Step 4: Applying Kubernetes manifests...

echo Creating namespace...
kubectl apply -f "!TEMP_DIR!\namespace.yaml"

echo Deploying application...
kubectl apply -f "!TEMP_DIR!\deployment.yaml"

echo Creating service...
kubectl apply -f "!TEMP_DIR!\service.yaml"

echo Creating ingress...
kubectl apply -f "!TEMP_DIR!\ingress.yaml"

echo.
echo Step 5: Waiting for deployment rollout...
kubectl rollout status deployment/tour-management -n tour-management --timeout=5m
if !ERRORLEVEL! neq 0 (
    echo WARNING: Deployment rollout did not complete successfully
    echo Check pod status with: kubectl get pods -n tour-management
)

echo.
echo Step 6: Verifying deployment...
echo.
echo Pods:
kubectl get pods -n tour-management

echo.
echo Services:
kubectl get svc -n tour-management

echo.
echo Ingress:
kubectl get ingress -n tour-management

echo.
echo ====================================
echo Deployment Completed Successfully!
echo ====================================
echo.
echo Application ingress is being provisioned. Check status with:
echo kubectl get ingress -n tour-management
echo.
echo Useful commands:
echo   View logs: kubectl logs -l app=tour-management -n tour-management
echo   Scale deployment: kubectl scale deployment/tour-management --replicas=3 -n tour-management
echo   Delete deployment: kubectl delete namespace tour-management
echo.

rmdir /S /Q "!TEMP_DIR!"
endlocal