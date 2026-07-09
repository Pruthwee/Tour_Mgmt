#!/bin/bash
set -e
set -o pipefail

PROJECT_NAME="Tour_Management"

echo "-------------------------------------------------------"
echo " .NET Framework Build and Push Script"
echo "-------------------------------------------------------"

# Prompt for Image Tag
read -p "Enter image tag [latest]: " IMAGE_TAG
IMAGE_TAG=${IMAGE_TAG:-latest}

# Registry Selection
echo "Select Registry:"
echo "1) AWS ECR"
echo "2) Docker Hub"
read -p "Choice [1-2]: " REGISTRY_CHOICE

if [ "$REGISTRY_CHOICE" == "1" ]; then
    read -p "Enter AWS Region [us-east-1]: " AWS_REGION
    AWS_REGION=${AWS_REGION:-us-east-1}
    read -p "Enter ECR Repository Name: " ECR_REPO
    
    # Sanitize Image Name
    IMAGE_NAME=$(echo "$PROJECT_NAME" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9' '-' | sed 's/^-*//;s/-*$//')
    REGISTRY_URL=$(aws ecr describe-repositories --repository-names "$IMAGE_NAME" --region "$AWS_REGION" --query 'repositories[0].repositoryUri' --output text 2>/dev/null || echo "")
    
    if [ -z "$REGISTRY_URL" ]; then
        echo "Creating ECR repository $IMAGE_NAME..."
        aws ecr create-repository --repository-name "$IMAGE_NAME" --region "$AWS_REGION" > /dev/null
        REGISTRY_URL=$(aws ecr describe-repositories --repository-names "$IMAGE_NAME" --region "$AWS_REGION" --query 'repositories[0].repositoryUri' --output text)
    fi
    
    echo "Logging into ECR..."
    aws ecr get-login-password --region "$AWS_REGION" | docker login --username AWS --password-stdin "$REGISTRY_URL"
    
    FULL_IMAGE_NAME="$REGISTRY_URL:$IMAGE_TAG"
else
    read -p "Enter Docker Hub Username: " DOCKER_USERNAME
    read -sp "Enter Docker Hub Password: " DOCKER_PASSWORD
    echo ""
    
    echo "Logging into Docker Hub..."
    echo "$DOCKER_PASSWORD" | docker login --username "$DOCKER_USERNAME" --password-stdin
    
    IMAGE_NAME=$(echo "$PROJECT_NAME" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9' '-' | sed 's/^-*//;s/-*$//')
    FULL_IMAGE_NAME="$DOCKER_USERNAME/$IMAGE_NAME:$IMAGE_TAG"
fi

echo "Building Docker image..."
docker build -t "$FULL_IMAGE_NAME" .

echo "Pushing Docker image..."
docker push "$FULL_IMAGE_NAME"

echo "Successfully built and pushed $FULL_IMAGE_NAME"
