#!/bin/bash
set -e

PROJECT_NAME="Tour_Management"
IMAGE_TAG=$(read -p "Enter image tag (default: latest): " tag && echo ${tag:-latest})

echo "Select Registry:"
echo "1. AWS ECR"
echo "2. Docker Hub"
read -p "Choice [1-2]: " REGISTRY_CHOICE

# Sanitize image name
IMAGE_NAME=$(echo "$PROJECT_NAME" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9' '-' | sed 's/^-*//;s/-*$//')

if [ "$REGISTRY_CHOICE" == "1" ]; then
    read -p "Enter AWS Region: " AWS_REGION
    read -p "Enter ECR Repository Name: " ECR_REPO
    REGISTRY_URL=$(aws ecr describe-repositories --repository-names $ECR_REPO --region $AWS_REGION --query 'repositories[0].repositoryUri' --output text 2>/dev/null || echo "")
    
    if [ -z "$REGISTRY_URL" ]; then
        echo "Creating ECR repository..."
        aws ecr create-repository --repository-name $ECR_REPO --region $AWS_REGION
        REGISTRY_URL=$(aws ecr describe-repositories --repository-names $ECR_REPO --region $AWS_REGION --query 'repositories[0].repositoryUri' --output text)
    fi
    
    aws ecr get-login-password --region $AWS_REGION | docker login --username AWS --password-stdin $REGISTRY_URL
    FULL_IMAGE_NAME="$REGISTRY_URL:$IMAGE_TAG"
else
    read -p "Enter Docker Hub Username: " DOCKER_USER
    read -p "Enter Docker Hub Password: " DOCKER_PASS
    echo $DOCKER_PASS | docker login --username $DOCKER_USER --password-stdin
    FULL_IMAGE_NAME="$DOCKER_USER/$IMAGE_NAME:$IMAGE_TAG"
fi

echo "Building image $FULL_IMAGE_NAME..."
docker build -t $FULL_IMAGE_NAME .

echo "Pushing image $FULL_IMAGE_NAME..."
docker push $FULL_IMAGE_NAME

echo "Successfully built and pushed $FULL_IMAGE_NAME"
