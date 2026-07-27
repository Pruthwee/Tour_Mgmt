# Deployment Guide for Tour Management Application on AWS EKS

## Prerequisites
- Docker installed locally
- AWS CLI installed and configured
- kubectl installed
- Access to an AWS EKS Cluster
- .NET Framework 4.7.2 SDK (for local builds)

## Local Development Setup
1. Clone the repository.
2. Use Docker Compose for local testing:
   ```bash
   docker-compose up --build
   ```
3. Access the application at `http://localhost`.

## Build and Push Instructions
### Linux/macOS
```bash
chmod +x scripts/build-push.sh
./scripts/build-push.sh
```
### Windows
```cmd
scripts\build-push.bat
```
The script will prompt you to choose between AWS ECR and Docker Hub and will handle the image tagging and pushing.

## AWS EKS Deployment
### Prerequisites
- Ensure your EKS cluster is running.
- Ensure the AWS Load Balancer Controller is installed in your cluster for Ingress to work.

### Deployment Walkthrough
1. Run the deployment script:
   - Linux/macOS: `./scripts/deploy-image.sh`
   - Windows: `scripts\deploy-image.bat`
2. Provide the required AWS region, cluster name, and the image URI pushed in the previous step.
3. The script will:
   - Update Kubernetes manifests with the image URI and environment variables.
   - Configure `kubectl` to point to your EKS cluster.
   - Apply the namespace, deployment, service, and ingress manifests.
   - Wait for the rollout to complete.

## Troubleshooting
- **Pod Failures**: Check logs using `kubectl logs -n tour-management <pod-name>`.
- **Service Issues**: Verify the service is targeting the correct pods using `kubectl get endpoints -n tour-management`.
- **Ingress Problems**: Check the AWS Load Balancer Controller logs and ensure the ingress class is set to `alb`.

## Configuration Management
Environment variables are managed via the `deployment.yaml` file. The deployment scripts prompt for critical variables like `DATABASE_CONNECTION_STRING`.

## Security Considerations
- Use AWS Secrets Manager or Kubernetes Secrets for sensitive data instead of plain text environment variables.
- Ensure the EKS cluster has appropriate IAM roles for the pods.
- Use a non-root user in the Dockerfile for better security (though limited in Windows containers).

## .NET Framework Specific Notes
- This application uses Windows Containers. Ensure your EKS cluster has Windows worker nodes configured.
- The base image used is `mcr.microsoft.com/dotnet/framework/aspnet:4.7.2`.
