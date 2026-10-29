#!/bin/bash
# Builds the TESTING_CON_5 Windows container image and pushes it to AWS ECR or Docker Hub.
# Run from the repository root:  ./scripts/build-push.sh   (needs docker in Windows containers mode, and the AWS CLI for ECR)
set -e
set -o pipefail

PROJECT_NAME="TESTING_CON_5"
IMAGE_NAME=$(echo "$PROJECT_NAME" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9' '-' | sed 's/^-*//;s/-*$//')

echo "=== Build and push of $IMAGE_NAME ==="
echo "Select the registry:"
echo "  1. AWS ECR (Amazon Elastic Container Registry)"
echo "  2. Docker Hub"
read -r -p "Enter 1 or 2: " REGISTRY_CHOICE

if [ "$REGISTRY_CHOICE" = "1" ]; then
  command -v aws >/dev/null 2>&1 || { echo "The AWS CLI is required for ECR"; exit 1; }
  read -r -p "Enter the AWS region (for example us-east-1): " AWS_REGION
  read -r -p "Enter the AWS account ID: " AWS_ACCOUNT_ID
  read -r -p "Enter the ECR repository name [$IMAGE_NAME]: " ECR_REPO
  ECR_REPO=${ECR_REPO:-$IMAGE_NAME}
  if [ -z "$AWS_REGION" ] || [ -z "$AWS_ACCOUNT_ID" ]; then
    echo "The AWS region and the account ID are required"
    exit 1
  fi
  REGISTRY_URL="$AWS_ACCOUNT_ID.dkr.ecr.$AWS_REGION.amazonaws.com"
  REPO_PATH="$ECR_REPO"
elif [ "$REGISTRY_CHOICE" = "2" ]; then
  read -r -p "Enter the Docker Hub username: " DOCKER_USERNAME
  read -r -s -p "Enter the Docker Hub password or access token: " DOCKER_PASSWORD
  echo
  if [ -z "$DOCKER_USERNAME" ] || [ -z "$DOCKER_PASSWORD" ]; then
    echo "The Docker Hub username and password are required"
    exit 1
  fi
  REPO_PATH="$(echo "$DOCKER_USERNAME" | tr '[:upper:]' '[:lower:]')/$IMAGE_NAME"
else
  echo "Invalid choice: enter 1 or 2"
  exit 1
fi

read -r -p "Enter the image tag [latest]: " IMAGE_TAG
IMAGE_TAG=$(echo "$IMAGE_TAG" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9._-' '-' | sed 's/^-*//;s/-*$//')
IMAGE_TAG=${IMAGE_TAG:-latest}

if [ "$REGISTRY_CHOICE" = "1" ]; then
  FULL_IMAGE_NAME="$REGISTRY_URL/$REPO_PATH:$IMAGE_TAG"
else
  FULL_IMAGE_NAME="$REPO_PATH:$IMAGE_TAG"
fi

echo "Building the image $FULL_IMAGE_NAME ..."
docker build -f Dockerfile -t "$FULL_IMAGE_NAME" .
echo "Build finished."

if [ "$REGISTRY_CHOICE" = "1" ]; then
  echo "Logging in to AWS ECR ($REGISTRY_URL) ..."
  aws ecr get-login-password --region "$AWS_REGION" | docker login --username AWS --password-stdin "$REGISTRY_URL"
  echo "Checking the ECR repository $ECR_REPO ..."
  aws ecr describe-repositories --repository-names "$ECR_REPO" --region "$AWS_REGION" >/dev/null 2>&1 || aws ecr create-repository --repository-name "$ECR_REPO" --region "$AWS_REGION"
else
  echo "Logging in to Docker Hub ..."
  echo "$DOCKER_PASSWORD" | docker login --username "$DOCKER_USERNAME" --password-stdin
fi

echo "Pushing $FULL_IMAGE_NAME ..."
docker push "$FULL_IMAGE_NAME"

echo "Done. Image pushed: $FULL_IMAGE_NAME"
echo "Use this image URI with scripts/deploy-image.sh"
