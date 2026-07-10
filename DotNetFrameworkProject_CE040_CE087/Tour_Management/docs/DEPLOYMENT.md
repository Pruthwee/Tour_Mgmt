# Deployment Guide - Tour Management (.NET Framework)

## Overview
This guide provides instructions for containerizing and deploying the Tour Management application to AWS ECS Fargate.

## Prerequisites
- Docker installed locally
- AWS CLI installed and configured
- Access to an AWS account with appropriate permissions (ECS, ECR, IAM, VPC)
- Windows Container support enabled in Docker Desktop (since this is a .NET Framework app)

## Local Development Setup
1. Clone the repository.
2. Run the application using Docker Compose:
   ```bash
   docker-compose up --build
   ```
3. Access the application at `http://localhost:80`.

## Docker Deployment
### Build and Push
Use the provided scripts to build the image and push it to a registry (AWS ECR or Docker Hub).
- **Linux/macOS**: `./scripts/build-push.sh`
- **Windows**: `scripts\\build-push.bat`

## AWS ECS Fargate Deployment
### Prerequisites
- A VPC with at least two public subnets.
- A Security Group allowing traffic on port 80.
- An IAM Execution Role (`ecsTaskExecutionRole`) for pulling images and writing logs.

### Deployment Steps
1. Run the deployment script:
   - **Linux/macOS**: `./scripts/deploy-image.sh`
   - **Windows**: `scripts\\deploy-image.bat`
2. Follow the interactive prompts to provide:
   - AWS Region
   - ECS Cluster Name
   - VPC and Subnet IDs
   - Security Group ID
   - Docker Image URI

### ECS Configuration Details
- **Launch Type**: FARGATE
- **Network Mode**: awsvpc
- **CPU/Memory**: 512 / 1024 MB
- **Logging**: AWS CloudWatch logs (`/ecs/tour-management`)

## Troubleshooting
- **Task Failures**: Check the CloudWatch log group `/ecs/tour-management` for application errors.
- **Network Issues**: Ensure the Security Group allows inbound traffic on port 80 and the subnets have a route to the internet.
- **CPU/Memory Errors**: If the application crashes due to memory, increase the memory in `ecs/task-definition.json`.

## Configuration Management
Environment-specific settings are managed via `Web.config` and environment variables.
