# Tour Management Application - Deployment Guide

## Overview

This guide provides comprehensive instructions for deploying the Tour Management ASP.NET Framework 4.7.2 application to AWS EKS (Elastic Kubernetes Service) using Docker containers.

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Application Architecture](#application-architecture)
3. [Local Development Setup](#local-development-setup)
4. [Building Docker Image](#building-docker-image)
5. [AWS EKS Setup](#aws-eks-setup)
6. [Kubernetes Deployment](#kubernetes-deployment)
7. [Configuration Management](#configuration-management)
8. [Monitoring and Troubleshooting](#monitoring-and-troubleshooting)
9. [Security Considerations](#security-considerations)
10. [Scaling and Performance](#scaling-and-performance)

---

## Prerequisites

### Required Software

- **Docker Desktop for Windows** (version 4.x or higher)
  - Windows containers support enabled
  - WSL 2 backend recommended
- **AWS CLI** (version 2.x)
  - Configure with: `aws configure`
- **kubectl** (version 1.28 or higher)
  - Install: `choco install kubernetes-cli` (Windows)
- **Git** (for version control)

### AWS Account Requirements

- AWS Account with appropriate IAM permissions
- IAM permissions for:
  - EKS cluster creation and management
  - ECR repository creation and image push
  - EC2 instances and networking
  - IAM role creation for service accounts
  - Application Load Balancer (ALB) management

### Windows Container Requirements

- **CRITICAL**: AWS EKS requires Windows Server 2019 or 2022 worker nodes for Windows containers
- EKS cluster must have Windows node group configured
- Windows containers cannot run on Linux nodes

---

## Application Architecture

### Technology Stack

- **Framework**: .NET Framework 4.7.2
- **Application Type**: ASP.NET Web Forms
- **Web Server**: IIS (Internet Information Services)
- **Database**: SQL Server (external, not containerized)
- **Container OS**: Windows Server Core

### Container Architecture

```
Multi-Stage Docker Build:
├── Build Stage (mcr.microsoft.com/dotnet/framework/sdk:4.8)
│   ├── NuGet package restoration
│   ├── MSBuild compilation
│   └── Application publish
└── Runtime Stage (mcr.microsoft.com/dotnet/framework/aspnet:4.8)
    ├── IIS web server
    ├── ASP.NET runtime
    └── Published application files
```

### Kubernetes Architecture

```
AWS EKS Cluster
├── Namespace: tour-management
├── Deployment: tour-management (2 replicas)
├── Service: tour-management-service (ClusterIP)
├── Ingress: tour-management-ingress (ALB)
└── ConfigMap: tour-management-config
```

---

## Local Development Setup

### Step 1: Clone Repository

```bash
git clone <repository-url>
cd TourCompTest
```

### Step 2: Review Configuration

Check the `Web.config` file for database connection strings and application settings:

```xml
<connectionStrings>
  <add name="dbconnection" 
       connectionString="Data Source=<server>;Initial Catalog=tourdb;User ID=<user>;Password=<password>" 
       providerName="System.Data.SqlClient"/>
</connectionStrings>
```

### Step 3: Build with Docker Compose (Local Testing)

```bash
docker-compose up --build
```

Access the application at: `http://localhost:8080`

### Step 4: Test Application Locally

1. Verify homepage loads correctly
2. Test user login functionality
3. Verify database connectivity
4. Check tour display and booking features

---

## Building Docker Image

### Option 1: Using Build Script (Recommended)

#### Linux/macOS:

```bash
chmod +x scripts/build-push.sh
./scripts/build-push.sh
```

#### Windows:

```cmd
scripts\build-push.bat
```

The script will:
1. Prompt for registry selection (AWS ECR or Docker Hub)
2. Collect registry credentials
3. Build the Docker image
4. Push to the selected registry
5. Auto-create ECR repository if it doesn't exist

### Option 2: Manual Build

```bash
# Build image
docker build -f DotNetFrameworkProject_CE040_CE087/Tour_Management/Dockerfile \
  -t tour-management:latest \
  ./DotNetFrameworkProject_CE040_CE087/Tour_Management

# Tag for ECR
docker tag tour-management:latest \
  <account-id>.dkr.ecr.<region>.amazonaws.com/tour-management:latest

# Push to ECR
aws ecr get-login-password --region <region> | \
  docker login --username AWS --password-stdin \
  <account-id>.dkr.ecr.<region>.amazonaws.com

docker push <account-id>.dkr.ecr.<region>.amazonaws.com/tour-management:latest
```

---

## AWS EKS Setup

### Step 1: Create EKS Cluster with Windows Support

```bash
# Create EKS cluster (Linux nodes for system components)
eksctl create cluster \
  --name tour-management-cluster \
  --region us-east-1 \
  --nodegroup-name linux-nodes \
  --node-type t3.medium \
  --nodes 2

# Add Windows node group
eksctl create nodegroup \
  --cluster tour-management-cluster \
  --region us-east-1 \
  --name windows-nodes \
  --node-type t3.large \
  --nodes 2 \
  --node-ami-family WindowsServer2022FullContainer
```

### Step 2: Install AWS Load Balancer Controller

```bash
# Create IAM policy
curl -o iam-policy.json https://raw.githubusercontent.com/kubernetes-sigs/aws-load-balancer-controller/main/docs/install/iam_policy.json

aws iam create-policy \
  --policy-name AWSLoadBalancerControllerIAMPolicy \
  --policy-document file://iam-policy.json

# Create IAM service account
eksctl create iamserviceaccount \
  --cluster=tour-management-cluster \
  --namespace=kube-system \
  --name=aws-load-balancer-controller \
  --attach-policy-arn=arn:aws:iam::<account-id>:policy/AWSLoadBalancerControllerIAMPolicy \
  --approve

# Install controller with Helm
helm repo add eks https://aws.github.io/eks-charts
helm repo update

helm install aws-load-balancer-controller eks/aws-load-balancer-controller \
  -n kube-system \
  --set clusterName=tour-management-cluster \
  --set serviceAccount.create=false \
  --set serviceAccount.name=aws-load-balancer-controller
```

### Step 3: Configure kubectl

```bash
aws eks update-kubeconfig \
  --region us-east-1 \
  --name tour-management-cluster

# Verify connection
kubectl cluster-info
kubectl get nodes
```

---

## Kubernetes Deployment

### Option 1: Using Deployment Script (Recommended)

#### Linux/macOS:

```bash
chmod +x scripts/deploy-image.sh
./scripts/deploy-image.sh
```

#### Windows:

```cmd
scripts\deploy-image.bat
```

The script will:
1. Configure kubectl for your EKS cluster
2. Prompt for Docker image URI
3. Prompt for database connection string
4. Update Kubernetes manifests
5. Apply all resources in correct order
6. Wait for deployment rollout
7. Display access information

### Option 2: Manual Deployment

```bash
# Create namespace
kubectl apply -f kubernetes/namespace.yaml

# Update deployment.yaml with your image URI
sed -i 's|{{IMAGE_URI}}|<your-image-uri>|g' kubernetes/deployment.yaml
sed -i 's|{{DB_CONNECTION_STRING}}|<your-connection-string>|g' kubernetes/deployment.yaml

# Apply resources
kubectl apply -f kubernetes/deployment.yaml
kubectl apply -f kubernetes/service.yaml
kubectl apply -f kubernetes/ingress.yaml

# Check deployment status
kubectl rollout status deployment/tour-management -n tour-management
```

### Verify Deployment

```bash
# Check pods
kubectl get pods -n tour-management

# Check services
kubectl get svc -n tour-management

# Check ingress
kubectl get ingress -n tour-management

# View logs
kubectl logs -l app=tour-management -n tour-management --tail=100
```

---

## Configuration Management

### Environment Variables

The application uses the following environment variables (configured in `deployment.yaml`):

- `DB_CONNECTION_STRING`: SQL Server connection string
- `ASPNET_ENVIRONMENT`: Application environment (Production)
- `TZ`: Timezone setting (default: UTC)

### Database Connection String Format

```
Data Source=<sql-server-host>,<port>;Initial Catalog=tourdb;User ID=<username>;Password=<password>;MultipleActiveResultSets=true;Encrypt=True;TrustServerCertificate=False
```

### Secrets Management

For production deployments, use Kubernetes Secrets:

```bash
# Create secret for database connection
kubectl create secret generic db-credentials \
  --from-literal=connection-string="<your-connection-string>" \
  -n tour-management

# Update deployment to use secret
kubectl set env deployment/tour-management \
  --from=secret/db-credentials \
  -n tour-management
```

### AWS Secrets Manager Integration

For enhanced security, use AWS Secrets Manager with the Secrets Store CSI Driver:

```bash
# Install Secrets Store CSI Driver
helm repo add secrets-store-csi-driver https://kubernetes-sigs.github.io/secrets-store-csi-driver/charts
helm install csi-secrets-store secrets-store-csi-driver/secrets-store-csi-driver --namespace kube-system

# Install AWS Provider
kubectl apply -f https://raw.githubusercontent.com/aws/secrets-store-csi-driver-provider-aws/main/deployment/aws-provider-installer.yaml
```

---

## Monitoring and Troubleshooting

### Health Checks

The deployment includes three types of health probes:

1. **Liveness Probe**: Checks if container is running
2. **Readiness Probe**: Checks if container can accept traffic
3. **Startup Probe**: Checks if application has started (Windows containers need more time)

### Common Issues and Solutions

#### Issue 1: Pods in CrashLoopBackOff

```bash
# Check pod logs
kubectl logs <pod-name> -n tour-management

# Describe pod for events
kubectl describe pod <pod-name> -n tour-management

# Common causes:
# - Invalid database connection string
# - Missing environment variables
# - Container image pull errors
```

#### Issue 2: Ingress Not Creating Load Balancer

```bash
# Check AWS Load Balancer Controller logs
kubectl logs -n kube-system deployment/aws-load-balancer-controller

# Verify IAM permissions
# Ensure subnet tags are correct:
# kubernetes.io/role/elb=1 (public subnets)
# kubernetes.io/cluster/<cluster-name>=owned/shared
```

#### Issue 3: Windows Container Fails to Start

```bash
# Verify node has Windows OS
kubectl get nodes -o wide

# Check node selector in deployment
kubectl get deployment tour-management -n tour-management -o yaml | grep -A 2 nodeSelector

# Ensure Windows nodes are ready
kubectl get nodes -l kubernetes.io/os=windows
```

#### Issue 4: Database Connection Errors

```bash
# Test connectivity from pod
kubectl exec -it <pod-name> -n tour-management -- powershell

# Test SQL Server connection
Test-NetConnection -ComputerName <sql-server-host> -Port 1433

# Verify connection string format
# Ensure firewall rules allow EKS security group
```

### Viewing Logs

```bash
# Stream logs from all pods
kubectl logs -l app=tour-management -n tour-management -f

# View logs from specific pod
kubectl logs <pod-name> -n tour-management --tail=500

# View previous container logs (if restarted)
kubectl logs <pod-name> -n tour-management --previous
```

### CloudWatch Integration

For centralized logging, install Fluent Bit:

```bash
# Create CloudWatch namespace
kubectl create namespace amazon-cloudwatch

# Deploy Fluent Bit DaemonSet
kubectl apply -f https://raw.githubusercontent.com/aws-samples/amazon-cloudwatch-container-insights/latest/k8s-deployment-manifest-templates/deployment-mode/daemonset/container-insights-monitoring/fluent-bit/fluent-bit.yaml
```

---

## Security Considerations

### Network Security

1. **VPC Configuration**:
   - Deploy EKS in private subnets
   - Use NAT Gateway for outbound traffic
   - Configure security groups to restrict access

2. **Ingress Security**:
   - Enable HTTPS with ACM certificates
   - Configure WAF rules on ALB
   - Restrict source IP ranges if needed

3. **Pod Security**:
   - Windows containers run as ContainerAdministrator by default
   - Implement least privilege principles
   - Use Pod Security Standards (Restricted profile)

### Secrets Management Best Practices

1. **Never commit secrets to Git**
2. **Use AWS Secrets Manager or Parameter Store**
3. **Rotate credentials regularly**
4. **Enable secret encryption at rest in EKS**
5. **Use IAM roles for service accounts (IRSA)**

### Database Security

1. **Use RDS with encryption at rest**
2. **Enable SSL/TLS for database connections**
3. **Restrict database security group to EKS cluster**
4. **Use IAM database authentication when possible**
5. **Implement connection pooling and proper timeout settings**

### Container Image Security

1. **Scan images for vulnerabilities**:
   ```bash
   aws ecr start-image-scan --repository-name tour-management --image-id imageTag=latest
   ```

2. **Enable ECR image scanning**
3. **Use specific image tags (not 'latest' in production)**
4. **Implement image signing and verification**

---

## Scaling and Performance

### Horizontal Pod Autoscaling

```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: tour-management-hpa
  namespace: tour-management
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: tour-management
  minReplicas: 2
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 80
```

Apply HPA:
```bash
kubectl apply -f hpa.yaml
kubectl get hpa -n tour-management
```

### Cluster Autoscaling

Enable EKS cluster autoscaler for Windows node group:

```bash
kubectl apply -f https://raw.githubusercontent.com/kubernetes/autoscaler/master/cluster-autoscaler/cloudprovider/aws/examples/cluster-autoscaler-autodiscover.yaml

kubectl -n kube-system annotate deployment.apps/cluster-autoscaler \
  cluster-autoscaler.kubernetes.io/safe-to-evict="false"

kubectl -n kube-system set image deployment.apps/cluster-autoscaler \
  cluster-autoscaler=k8s.gcr.io/autoscaling/cluster-autoscaler:v1.28.0
```

### Performance Tuning

#### Application-Level Optimizations

1. **Enable output caching in Web.config**:
   ```xml
   <system.web>
     <caching>
       <outputCacheSettings>
         <outputCacheProfiles>
           <add name="CacheFor1Hour" duration="3600" varyByParam="*" />
         </outputCacheProfiles>
       </outputCacheSettings>
     </caching>
   </system.web>
   ```

2. **Enable IIS compression**:
   ```xml
   <system.webServer>
     <httpCompression>
       <dynamicTypes>
         <add mimeType="text/*" enabled="true" />
         <add mimeType="application/javascript" enabled="true" />
       </dynamicTypes>
     </httpCompression>
   </system.webServer>
   ```

3. **Optimize database queries**:
   - Use connection pooling
   - Implement proper indexing
   - Cache frequently accessed data

#### Container-Level Optimizations

1. **Resource Requests and Limits**:
   ```yaml
   resources:
     requests:
       cpu: "500m"
       memory: "1Gi"
     limits:
       cpu: "1000m"
       memory: "2Gi"
   ```

2. **Startup Time Optimization**:
   - Increase `initialDelaySeconds` for probes
   - Use startup probes for slow-starting apps
   - Pre-warm application during container build

### Monitoring Performance

```bash
# View resource usage
kubectl top nodes
kubectl top pods -n tour-management

# Check pod resource allocation
kubectl describe node <node-name> | grep -A 5 "Allocated resources"
```

---

## Rollback Procedures

### Rollback Deployment

```bash
# View rollout history
kubectl rollout history deployment/tour-management -n tour-management

# Rollback to previous version
kubectl rollout undo deployment/tour-management -n tour-management

# Rollback to specific revision
kubectl rollout undo deployment/tour-management -n tour-management --to-revision=2

# Check rollback status
kubectl rollout status deployment/tour-management -n tour-management
```

### Complete Environment Cleanup

```bash
# Delete namespace (removes all resources)
kubectl delete namespace tour-management

# Delete EKS cluster
eksctl delete cluster --name tour-management-cluster --region us-east-1

# Delete ECR repository
aws ecr delete-repository --repository-name tour-management --region us-east-1 --force
```

---

## Additional Resources

### Documentation Links

- [AWS EKS Documentation](https://docs.aws.amazon.com/eks/)
- [Windows Containers on EKS](https://docs.aws.amazon.com/eks/latest/userguide/windows-support.html)
- [AWS Load Balancer Controller](https://kubernetes-sigs.github.io/aws-load-balancer-controller/)
- [.NET Framework Docker Images](https://hub.docker.com/_/microsoft-dotnet-framework)
- [Kubernetes Documentation](https://kubernetes.io/docs/)

### Support Commands

```bash
# Get cluster information
kubectl cluster-info dump > cluster-dump.txt

# Export all resources
kubectl get all -n tour-management -o yaml > resources.yaml

# Generate support bundle
kubectl-supportbundle --namespace tour-management
```

### Contact and Support

For application-specific issues, contact the development team.
For AWS/infrastructure issues, contact your DevOps team.

---

## Changelog

- **Version 1.0** (2026-03-09): Initial deployment guide for Tour Management application

---

**End of Deployment Guide**