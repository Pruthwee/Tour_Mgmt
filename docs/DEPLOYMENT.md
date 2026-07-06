# Tour_Management - Deployment Guide

## Overview

This guide covers the complete deployment process for the **Tour_Management** ASP.NET Web Forms application (.NET Framework 4.7.2) to AWS EKS (Elastic Kubernetes Service).

> **Important**: This is a **Windows container** application. It requires Windows nodes in your EKS cluster.

---

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Project Structure](#project-structure)
3. [Local Development with Docker](#local-development-with-docker)
4. [Build and Push Docker Image](#build-and-push-docker-image)
5. [AWS EKS Prerequisites](#aws-eks-prerequisites)
6. [EKS Cluster Setup](#eks-cluster-setup)
7. [Kubernetes Deployment](#kubernetes-deployment)
8. [Configuration Management](#configuration-management)
9. [Health Checks and Monitoring](#health-checks-and-monitoring)
10. [Scaling and Management](#scaling-and-management)
11. [Troubleshooting](#troubleshooting)
12. [Security Considerations](#security-considerations)
13. [Rollback Procedures](#rollback-procedures)

---

## Prerequisites

### Local Development Tools
- **Docker Desktop** (Windows containers mode enabled)
- **AWS CLI** v2.x or later
- **kubectl** v1.24 or later
- **eksctl** v0.120 or later (for cluster creation)
- **Git**

### AWS Requirements
- AWS Account with appropriate IAM permissions
- IAM user/role with:
  - `AmazonEKSClusterPolicy`
  - `AmazonEKSWorkerNodePolicy`
  - `AmazonEC2ContainerRegistryFullAccess`
  - `AmazonEKSServicePolicy`

### .NET Framework Requirements
- .NET Framework 4.7.2 SDK (for local builds)
- Visual Studio 2019/2022 or MSBuild tools (for local builds)
- SQL Server (external dependency - not containerized)

---

## Project Structure

```
TourMgmt-container/
├── Dockerfile                          # Windows container multi-stage build
├── docker-compose.yml                  # Local development compose file
├── .dockerignore                       # Docker build context exclusions
├── Tour_Management.sln                 # Visual Studio solution
├── DotNetFrameworkProject_CE040_CE087/
│   └── Tour_Management/
│       ├── Tour_Management.csproj      # Project file (.NET Framework 4.7.2)
│       ├── Web.config                  # Application configuration
│       ├── HealthCheck.ashx            # Health check endpoint
│       ├── *.aspx                      # ASP.NET Web Forms pages
│       └── App_Data/                   # Database files (use external SQL Server)
├── kubernetes/
│   ├── namespace.yaml                  # Kubernetes namespace
│   ├── deployment.yaml                 # Application deployment
│   ├── service.yaml                    # ClusterIP service
│   └── ingress.yaml                    # AWS ALB ingress
├── scripts/
│   ├── build-push.sh                   # Linux/macOS build & push script
│   ├── build-push.bat                  # Windows build & push script
│   ├── deploy-image.sh                 # Linux/macOS EKS deploy script
│   └── deploy-image.bat                # Windows EKS deploy script
└── docs/
    └── DEPLOYMENT.md                   # This file
```

---

## Local Development with Docker

### Prerequisites for Windows Containers
Docker Desktop must be switched to **Windows containers** mode:
1. Right-click Docker Desktop tray icon
2. Select "Switch to Windows containers..."
3. Confirm the switch

### Build the Image Locally

```bash
# From the TourMgmt-container directory
docker build -f Dockerfile -t tour-management:local .
```

### Run with Docker Compose

```bash
# Set required environment variables
export DB_CONNECTION_STRING="Data Source=your-sql-server;Initial Catalog=tourdb;User ID=sa;Password=YourPassword123!"

# Start the application
docker-compose up -d

# View logs
docker-compose logs -f tour-management

# Stop the application
docker-compose down
```

The application will be available at: `http://localhost:8080`

Health check endpoint: `http://localhost:8080/HealthCheck.ashx`

### Database Setup

The application requires a SQL Server database. Run the following scripts from `Database.txt`:

```sql
-- 1. Create database
CREATE DATABASE tourdb;

-- 2. Create UserInfo table
CREATE TABLE [dbo].[UserInfo] (
    [Email]     VARCHAR (50) NOT NULL,
    [FirstName] VARCHAR (50) NOT NULL,
    [LastName]  VARCHAR (50) NOT NULL,
    [Gender]    VARCHAR (10) NOT NULL,
    [Password]  VARCHAR (50) NOT NULL,
    [dob]       DATE         NOT NULL,
    [Street]    VARCHAR (50) NOT NULL,
    [City]      VARCHAR (50) NOT NULL,
    [State]     VARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Email] ASC),
    CONSTRAINT [CK_Gender] CHECK ([Gender]='Female' OR [Gender]='Male')
);

-- 3. Create Tour table
CREATE TABLE [dbo].[Tour] (
    [TOUR_ID]   NUMERIC (5)   IDENTITY (1, 1) NOT NULL,
    [TOUR_NAME] VARCHAR (20)  NOT NULL,
    [PLACE]     VARCHAR (20)  NOT NULL,
    [DAYS]      NUMERIC (2)   NOT NULL,
    [PRICE]     NUMERIC (6)   NOT NULL,
    [LOCATIONS] VARCHAR (100) NOT NULL,
    [TOUR_INFO] VARCHAR (200) NOT NULL,
    [pic]       VARCHAR (200) NULL,
    PRIMARY KEY CLUSTERED ([TOUR_ID] ASC)
);

-- 4. Create booking table
CREATE TABLE [dbo].[booking] (
    [TOUR_ID]   INT          IDENTITY (1, 1) NOT NULL,
    [TOUR_NAME] VARCHAR (50) NULL,
    [PLACE]     VARCHAR (50) NULL,
    [Email]     VARCHAR (50) NULL,
    [FirstName] VARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([TOUR_ID] ASC)
);
```

---

## Build and Push Docker Image

### Using Linux/macOS (build-push.sh)

```bash
# Make script executable
chmod +x scripts/build-push.sh

# Run from TourMgmt-container directory
./scripts/build-push.sh
```

### Using Windows (build-push.bat)

```cmd
# Run from TourMgmt-container directory
scripts\build-push.bat
```

The script will prompt you to:
1. Enter an image tag (defaults to `latest`)
2. Select registry type (AWS ECR or Docker Hub)
3. Enter registry credentials and details

> **Note**: Windows container images can only be built on Windows hosts with Docker Desktop in Windows containers mode.

---

## AWS EKS Prerequisites

### 1. Install Required Tools

```bash
# Install AWS CLI
curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o "awscliv2.zip"
unzip awscliv2.zip && sudo ./aws/install

# Install kubectl
curl -LO "https://dl.k8s.io/release/$(curl -L -s https://dl.k8s.io/release/stable.txt)/bin/linux/amd64/kubectl"
chmod +x kubectl && sudo mv kubectl /usr/local/bin/

# Install eksctl
curl --silent --location "https://github.com/weaveworks/eksctl/releases/latest/download/eksctl_$(uname -s)_amd64.tar.gz" | tar xz -C /tmp
sudo mv /tmp/eksctl /usr/local/bin
```

### 2. Configure AWS CLI

```bash
aws configure
# Enter: AWS Access Key ID, Secret Access Key, Region, Output format
```

### 3. Verify AWS Access

```bash
aws sts get-caller-identity
aws eks list-clusters --region us-east-1
```

---

## EKS Cluster Setup

### Critical: Windows Node Group Requirement

The Tour_Management application uses Windows containers and **requires Windows worker nodes** in your EKS cluster.

### Create EKS Cluster with Windows Node Group

```bash
# Create cluster with eksctl
eksctl create cluster \
  --name tour-management-cluster \
  --region us-east-1 \
  --version 1.28 \
  --nodegroup-name linux-nodes \
  --node-type t3.medium \
  --nodes 2 \
  --nodes-min 1 \
  --nodes-max 4

# Add Windows node group
eksctl create nodegroup \
  --cluster tour-management-cluster \
  --region us-east-1 \
  --name windows-nodes \
  --node-type m5.xlarge \
  --nodes 2 \
  --nodes-min 1 \
  --nodes-max 4 \
  --node-ami-family WindowsServer2022FullContainer
```

### Enable Windows Support

```bash
# Apply VPC resource controller for Windows support
kubectl apply -f https://amazon-eks.s3.us-west-2.amazonaws.com/manifests/us-west-2/vpc-resource-controller/latest/vpc-resource-controller.yaml

# Enable Windows IPAM
kubectl set env daemonset aws-node -n kube-system ENABLE_WINDOWS_IPAM=true
```

### Install AWS Load Balancer Controller

```bash
# Create IAM policy for ALB controller
curl -O https://raw.githubusercontent.com/kubernetes-sigs/aws-load-balancer-controller/v2.6.0/docs/install/iam_policy.json
aws iam create-policy \
  --policy-name AWSLoadBalancerControllerIAMPolicy \
  --policy-document file://iam_policy.json

# Create service account
eksctl create iamserviceaccount \
  --cluster=tour-management-cluster \
  --namespace=kube-system \
  --name=aws-load-balancer-controller \
  --role-name AmazonEKSLoadBalancerControllerRole \
  --attach-policy-arn=arn:aws:iam::$(aws sts get-caller-identity --query Account --output text):policy/AWSLoadBalancerControllerIAMPolicy \
  --approve

# Install via Helm
helm repo add eks https://aws.github.io/eks-charts
helm repo update
helm install aws-load-balancer-controller eks/aws-load-balancer-controller \
  -n kube-system \
  --set clusterName=tour-management-cluster \
  --set serviceAccountName=aws-load-balancer-controller
```

### Configure kubectl

```bash
aws eks update-kubeconfig --region us-east-1 --name tour-management-cluster
kubectl cluster-info
kubectl get nodes
```

---

## Kubernetes Deployment

### Option 1: Using Deploy Script (Recommended)

```bash
# Linux/macOS
chmod +x scripts/deploy-image.sh
./scripts/deploy-image.sh

# Windows
scripts\deploy-image.bat
```

The script will prompt for:
- AWS Region
- EKS Cluster Name
- Docker Image URI
- DB_CONNECTION_STRING

### Option 2: Manual Deployment

```bash
# 1. Update deployment.yaml with your image URI
sed -i 's|{{IMAGE_URI}}|YOUR_REGISTRY/tour-management:latest|g' kubernetes/deployment.yaml
sed -i 's|{{DB_CONNECTION_STRING}}|YOUR_CONNECTION_STRING|g' kubernetes/deployment.yaml

# 2. Apply manifests in order
kubectl apply -f kubernetes/namespace.yaml
kubectl apply -f kubernetes/deployment.yaml
kubectl apply -f kubernetes/service.yaml
kubectl apply -f kubernetes/ingress.yaml

# 3. Wait for rollout
kubectl rollout status deployment/tour-management -n tour-management --timeout=300s

# 4. Verify resources
kubectl get pods,svc,ingress -n tour-management
```

### Verify Deployment

```bash
# Check pod status
kubectl get pods -n tour-management

# Check pod logs
kubectl logs -l app=tour-management -n tour-management

# Describe pod for events
kubectl describe pod -l app=tour-management -n tour-management

# Test health endpoint
kubectl port-forward svc/tour-management-service 8080:80 -n tour-management
curl http://localhost:8080/HealthCheck.ashx
```

---

## Configuration Management

### Environment Variables

| Variable | Description | Required |
|----------|-------------|----------|
| `ASPNET_ENV` | Application environment | Yes (default: Production) |
| `TOUR_MANAGEMENT_ENV` | Application environment | Yes (default: Production) |
| `DB_CONNECTION_STRING` | SQL Server connection string | Yes |

### Connection String Format

```
Data Source=your-sql-server-host;Initial Catalog=tourdb;User ID=sa;Password=YourPassword123!;MultipleActiveResultSets=True
```

### Using Kubernetes Secrets for Connection Strings

```bash
# Create secret for database connection string
kubectl create secret generic tour-management-secrets \
  --from-literal=db-connection-string="Data Source=your-sql-server;Initial Catalog=tourdb;User ID=sa;Password=YourPassword123!" \
  -n tour-management
```

Update `deployment.yaml` to use the secret:

```yaml
env:
  - name: DB_CONNECTION_STRING
    valueFrom:
      secretKeyRef:
        name: tour-management-secrets
        key: db-connection-string
```

### Web.config Transformation

For production deployments, the `Web.Release.config` transform removes debug attributes. The connection string should be provided via environment variables and injected at runtime.

---

## Health Checks and Monitoring

### Health Check Endpoint

The application exposes a health check at `/HealthCheck.ashx`:

```bash
curl http://your-app-url/HealthCheck.ashx
# Response: {"status":"healthy","application":"Tour_Management","timestamp":"2024-01-01T00:00:00.000Z"}
```

### Kubernetes Probes Configuration

The deployment is configured with:
- **Liveness Probe**: `/HealthCheck.ashx` - checks if the application is running
  - Initial delay: 60 seconds (Windows containers take longer to start)
  - Period: 30 seconds
- **Readiness Probe**: `/HealthCheck.ashx` - checks if the application is ready to serve traffic
  - Initial delay: 45 seconds
  - Period: 15 seconds

### Monitoring with CloudWatch

```bash
# Install CloudWatch agent for EKS
kubectl apply -f https://raw.githubusercontent.com/aws-samples/amazon-cloudwatch-container-insights/latest/k8s-deployment-manifest-templates/deployment-mode/daemonset/container-insights-monitoring/quickstart/cwagent-fluentd-quickstart.yaml
```

---

## Scaling and Management

### Manual Scaling

```bash
# Scale to 3 replicas
kubectl scale deployment tour-management --replicas=3 -n tour-management

# Check scaling status
kubectl get pods -n tour-management
```

### Horizontal Pod Autoscaler (HPA)

```bash
# Create HPA
kubectl autoscale deployment tour-management \
  --cpu-percent=70 \
  --min=2 \
  --max=10 \
  -n tour-management

# Check HPA status
kubectl get hpa -n tour-management
```

### Rolling Updates

```bash
# Update image
kubectl set image deployment/tour-management \
  tour-management=YOUR_REGISTRY/tour-management:v2.0 \
  -n tour-management

# Monitor rollout
kubectl rollout status deployment/tour-management -n tour-management

# View rollout history
kubectl rollout history deployment/tour-management -n tour-management
```

---

## Troubleshooting

### Common Issues

#### 1. Windows Container Not Starting

```bash
# Check pod events
kubectl describe pod -l app=tour-management -n tour-management

# Check if Windows nodes are available
kubectl get nodes -l kubernetes.io/os=windows

# Check node selector in deployment
kubectl get deployment tour-management -n tour-management -o yaml | grep nodeSelector
```

#### 2. IIS Not Starting

```bash
# Get pod logs
kubectl logs -l app=tour-management -n tour-management

# Exec into pod (Windows)
kubectl exec -it <pod-name> -n tour-management -- powershell
# Inside pod:
Get-Service W3SVC
Get-EventLog -LogName Application -Newest 20
```

#### 3. Database Connection Failures

```bash
# Check environment variables
kubectl exec -it <pod-name> -n tour-management -- powershell -Command "Get-ChildItem Env:"

# Test SQL Server connectivity from pod
kubectl exec -it <pod-name> -n tour-management -- powershell -Command "Test-NetConnection -ComputerName your-sql-server -Port 1433"
```

#### 4. Ingress / ALB Not Created

```bash
# Check ALB controller logs
kubectl logs -n kube-system -l app.kubernetes.io/name=aws-load-balancer-controller

# Check ingress events
kubectl describe ingress tour-management-ingress -n tour-management

# Verify ALB controller is running
kubectl get pods -n kube-system | grep aws-load-balancer
```

#### 5. Image Pull Errors

```bash
# Check pod events for image pull errors
kubectl describe pod -l app=tour-management -n tour-management | grep -A 5 "Events:"

# Verify ECR permissions
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin YOUR_ACCOUNT.dkr.ecr.us-east-1.amazonaws.com

# Check node IAM role has ECR access
aws iam list-attached-role-policies --role-name <node-instance-role>
```

#### 6. Health Check Failures

```bash
# Test health endpoint directly
kubectl port-forward svc/tour-management-service 8080:80 -n tour-management
curl -v http://localhost:8080/HealthCheck.ashx

# Check IIS application pool status
kubectl exec -it <pod-name> -n tour-management -- powershell -Command "Get-WebConfiguration system.applicationHost/applicationPools/add | Select-Object name, state"
```

---

## Security Considerations

### 1. Connection String Security
- **Never** hardcode connection strings in `Web.config` for production
- Use Kubernetes Secrets or AWS Secrets Manager
- Rotate database credentials regularly

### 2. Container Security
- The application runs under IIS Application Pool Identity (least privilege)
- Windows containers include Windows security patches - keep base images updated
- Regularly update `mcr.microsoft.com/dotnet/framework/runtime:4.7.2` base image

### 3. Network Security
- Use Network Policies to restrict pod-to-pod communication
- Configure ALB with HTTPS (port 443) for production
- Use AWS WAF with ALB for web application firewall protection

### 4. Secrets Management

```bash
# Use AWS Secrets Manager
aws secretsmanager create-secret \
  --name tour-management/db-connection \
  --secret-string "Data Source=your-server;Initial Catalog=tourdb;User ID=sa;Password=YourPassword123!"

# Install External Secrets Operator for Kubernetes integration
helm repo add external-secrets https://charts.external-secrets.io
helm install external-secrets external-secrets/external-secrets -n kube-system
```

### 5. HTTPS Configuration

Update `ingress.yaml` for HTTPS:

```yaml
annotations:
  alb.ingress.kubernetes.io/listen-ports: '[{"HTTP": 80}, {"HTTPS": 443}]'
  alb.ingress.kubernetes.io/ssl-redirect: '443'
  alb.ingress.kubernetes.io/certificate-arn: arn:aws:acm:us-east-1:ACCOUNT:certificate/CERT-ID
```

---

## Rollback Procedures

### Rollback Deployment

```bash
# Rollback to previous version
kubectl rollout undo deployment/tour-management -n tour-management

# Rollback to specific revision
kubectl rollout history deployment/tour-management -n tour-management
kubectl rollout undo deployment/tour-management --to-revision=2 -n tour-management

# Verify rollback
kubectl rollout status deployment/tour-management -n tour-management
kubectl get pods -n tour-management
```

### Emergency Cleanup

```bash
# Delete all resources in namespace
kubectl delete namespace tour-management

# Or delete individual resources
kubectl delete deployment tour-management -n tour-management
kubectl delete service tour-management-service -n tour-management
kubectl delete ingress tour-management-ingress -n tour-management
```

---

## Technology-Specific Notes

### .NET Framework 4.7.2 Considerations

1. **Windows Containers Only**: .NET Framework 4.7.2 requires Windows containers. Linux containers are not supported.

2. **IIS Hosting**: The application is hosted on IIS inside the Windows container. IIS must be properly configured.

3. **Application Pool**: The DefaultAppPool is configured for .NET Framework 4.0 (which supports 4.7.2).

4. **Session State**: Default in-process session state is not suitable for multi-replica deployments. Consider:
   - SQL Server session state
   - Redis session state (StackExchange.Redis)
   - Sticky sessions via ALB (not recommended for production)

5. **LocalDB Not Supported**: The original `Web.config` uses LocalDB (`(LocalDB)\MSSQLLocalDB`). This must be replaced with a proper SQL Server connection string for containerized deployments.

6. **Static Files**: Tour images in `Tour_pics/` and `pics/` directories are included in the container image. For production, consider using AWS S3 for static file storage.

7. **Windows Container Startup Time**: Windows containers typically take 60-120 seconds to start. The Kubernetes probes are configured with appropriate initial delays.

8. **Node Requirements**: Windows nodes require larger instance types (minimum `m5.xlarge` recommended for .NET Framework applications).

### Upgrading to .NET 8 (Recommended)

Consider migrating to ASP.NET Core on .NET 8 for:
- Linux container support (smaller images, lower cost)
- Better performance and scalability
- Active Microsoft support
- Easier Kubernetes integration

---

*Generated for Tour_Management - ASP.NET Web Forms (.NET Framework 4.7.2)*
*Target Platform: AWS EKS*
