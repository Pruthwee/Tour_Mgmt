# Deployment Guide for Tour Management on AWS EKS

This guide provides instructions for containerizing and deploying the Tour Management .NET Framework application to AWS Elastic Kubernetes Service (EKS).

## Prerequisites

### System Requirements
- Docker Desktop (with Windows Containers enabled for .NET Framework)
- AWS CLI configured with appropriate IAM permissions
- kubectl installed
- .NET Framework 4.7.2 SDK (for local builds)

### AWS EKS Requirements
- An existing EKS Cluster
- AWS Load Balancer Controller installed in the cluster (for Ingress)
- ECR Repository (will be created automatically by build scripts)

## Local Development Setup

### Using Docker Compose
1. Ensure Docker is in **Windows Containers** mode.
2. Run the following command from the project root:
   ```bash
   docker-compose up --build
   ```
3. The application will be available at `http://localhost:80`.

## Build and Push Instructions

### Linux/macOS (using Bash)
```bash
chmod +x scripts/build-push.sh
./scripts/build-push.sh
```

### Windows (using Batch)
```cmd
scripts\build-push.bat
```

The script will prompt you to choose between AWS ECR and Docker Hub and will handle the authentication and pushing of the image.

## AWS EKS Deployment Walkthrough

### 1. Configure kubectl
The deployment script handles this, but manually you can run:
```bash
aws eks update-kubeconfig --region <region> --name <cluster-name>
```

### 2. Deploy the Application
Run the deployment script:
- **Linux/macOS**: `./scripts/deploy-image.sh`
- **Windows**: `scripts\deploy-image.bat`

The script will:
1. Prompt for the image URI and environment variables.
2. Update the Kubernetes manifests.
3. Apply the namespace, deployment, service, and ingress.
4. Wait for the rollout to complete.

## Kubernetes Manifest Descriptions

- `namespace.yaml`: Creates a dedicated namespace `tour-management` for isolation.
- `deployment.yaml`: Defines the application pods, resource limits (CPU: 500m, Memory: 1Gi), and health probes.
- `service.yaml`: Exposes the application internally within the cluster via ClusterIP.
- `ingress.yaml`: Configures an AWS Application Load Balancer (ALB) to route external traffic to the service.

## Troubleshooting

### Pod Failures
- Check pod logs: `kubectl logs -l app=tour-management -n tour-management`
- Describe pod: `kubectl describe pod <pod-name> -n tour-management`

### Service/Ingress Issues
- Verify service endpoints: `kubectl get endpoints -n tour-management`
- Check ALB status in AWS Console.

### .NET Framework Specifics
- Ensure the base image `mcr.microsoft.com/dotnet/framework/aspnet:4.8` is compatible with your cluster's node OS (Windows nodes are required for .NET Framework containers).

## Configuration Management
Environment variables are used to manage configuration. The `deployment.yaml` includes placeholders for:
- `IMAGE_URI`: The full path to the Docker image.
- `DB_CONNECTION_STRING`: The connection string for the SQL Server database.

## Security Considerations
- Use AWS Secrets Manager or Kubernetes Secrets for sensitive data like connection strings.
- Ensure the ALB is configured with HTTPS for production environments.
