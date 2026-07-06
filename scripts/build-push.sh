#!/bin/bash
# =============================================================================
# build-push.sh - Build and Push Docker Image for Tour_Management
# ASP.NET Web Forms (.NET Framework 4.7.2) - Windows Container
# =============================================================================
set -e

PROJECT_NAME="tour-management"
IMAGE_NAME=$(echo "$PROJECT_NAME" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9' '-' | sed 's/^-*//;s/-*$//')

echo "=============================================="
echo "  Tour_Management - Docker Build & Push"
echo "=============================================="
echo ""

# Prompt for image tag
read -p "Enter image tag (press Enter for 'latest'): " IMAGE_TAG_INPUT
IMAGE_TAG=$(echo "$IMAGE_TAG_INPUT" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9._-' '-' | sed 's/^-*//;s/-*$//')
if [ -z "$IMAGE_TAG" ]; then
  IMAGE_TAG="latest"
fi
echo "Using image tag: $IMAGE_TAG"
echo ""

# Prompt for registry type
echo "Select container registry:"
echo "  1. AWS ECR (Elastic Container Registry)"
echo "  2. Docker Hub"
read -p "Enter choice (1 or 2): " REGISTRY_CHOICE
echo ""

if [ "$REGISTRY_CHOICE" = "1" ]; then
  # ---- AWS ECR ----
  echo "--- AWS ECR Configuration ---"
  read -p "Enter AWS Region (e.g., us-east-1): " AWS_REGION
  read -p "Enter AWS Account ID: " AWS_ACCOUNT_ID
  read -p "Enter ECR Repository name (default: $IMAGE_NAME): " ECR_REPO_INPUT
  ECR_REPO="${ECR_REPO_INPUT:-$IMAGE_NAME}"
  ECR_REPO=$(echo "$ECR_REPO" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9/_-' '-' | sed 's/^-*//;s/-*$//')

  REGISTRY_URL="${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com"
  FULL_IMAGE_NAME="${REGISTRY_URL}/${ECR_REPO}:${IMAGE_TAG}"

  echo ""
  echo "Logging in to AWS ECR..."
  aws ecr get-login-password --region "$AWS_REGION" | docker login --username AWS --password-stdin "$REGISTRY_URL"
  if [ $? -ne 0 ]; then
    echo "ERROR: ECR login failed. Check your AWS credentials and region."
    exit 1
  fi
  echo "ECR login successful."

  # Auto-create ECR repository if it does not exist
  echo "Checking if ECR repository '$ECR_REPO' exists..."
  aws ecr describe-repositories --repository-names "$ECR_REPO" --region "$AWS_REGION" >/dev/null 2>&1 || \
    aws ecr create-repository --repository-name "$ECR_REPO" --region "$AWS_REGION"
  echo "ECR repository ready."

elif [ "$REGISTRY_CHOICE" = "2" ]; then
  # ---- Docker Hub ----
  echo "--- Docker Hub Configuration ---"
  read -p "Enter Docker Hub username: " DOCKER_USERNAME
  read -s -p "Enter Docker Hub password or access token: " DOCKER_PASSWORD
  echo ""
  read -p "Enter Docker Hub repository name (default: $IMAGE_NAME): " DOCKER_REPO_INPUT
  DOCKER_REPO="${DOCKER_REPO_INPUT:-$IMAGE_NAME}"
  DOCKER_REPO=$(echo "$DOCKER_REPO" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9/_-' '-' | sed 's/^-*//;s/-*$//')

  FULL_IMAGE_NAME="${DOCKER_USERNAME}/${DOCKER_REPO}:${IMAGE_TAG}"

  echo ""
  echo "Logging in to Docker Hub..."
  echo "$DOCKER_PASSWORD" | docker login --username "$DOCKER_USERNAME" --password-stdin
  if [ $? -ne 0 ]; then
    echo "ERROR: Docker Hub login failed. Check your credentials."
    exit 1
  fi
  echo "Docker Hub login successful."

else
  echo "ERROR: Invalid registry choice. Please enter 1 or 2."
  exit 1
fi

echo ""
echo "Building Docker image: $FULL_IMAGE_NAME"
echo "NOTE: This is a Windows container image. Ensure Docker is configured for Windows containers."
echo ""

# Build from repository root - Dockerfile is at the root of TourMgmt-container
docker build -f Dockerfile -t "$FULL_IMAGE_NAME" .
if [ $? -ne 0 ]; then
  echo "ERROR: Docker build failed."
  exit 1
fi
echo "Docker build successful."

echo ""
echo "Pushing image: $FULL_IMAGE_NAME"
docker push "$FULL_IMAGE_NAME"
if [ $? -ne 0 ]; then
  echo "ERROR: Docker push failed."
  exit 1
fi

echo ""
echo "=============================================="
echo "  SUCCESS: Image pushed successfully!"
echo "  Image: $FULL_IMAGE_NAME"
echo "=============================================="
