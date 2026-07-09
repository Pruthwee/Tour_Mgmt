#!/bin/bash
set -e
set -o pipefail

PROJECT_NAME="tour-management"

echo "-------------------------------------------------------"
echo " AWS EKS Deployment Script"
echo "-------------------------------------------------------"

read -p "Enter AWS Region [us-east-1]: " AWS_REGION
AWS_REGION=${AWS_REGION:-us-east-1}

read -p "Enter EKS Cluster Name: " CLUSTER_NAME
if [ -z "$CLUSTER_NAME" ]; then
    echo "Cluster name is required."
    exit 1
fi

read -p "Enter Docker Image URI (e.g., 123456789012.dkr.ecr.us-east-1.amazonaws.com/tour-management:latest): " IMAGE_URI
if [ -z "$IMAGE_URI" ]; then
    echo "Image URI is required."
    exit 1
fi

read -p "Enter DB Connection String (or press Enter to skip): " DB_CONNECTION_STRING

echo "Configuring kubectl..."
aws eks update-kubeconfig --region "$AWS_REGION" --name "$CLUSTER_NAME"

echo "Verifying cluster connectivity..."
kubectl cluster-info || { echo "Cluster connectivity failed"; exit 1; }

echo "Updating manifests..."
sed -i "s|{{IMAGE_URI}}|$IMAGE_URI|g" kubernetes/deployment.yaml
if [ -n "$DB_CONNECTION_STRING" ]; then
    sed -i "s|{{DB_CONNECTION_STRING}}|$DB_CONNECTION_STRING|g" kubernetes/deployment.yaml
fi

echo "Applying manifests..."
kubectl apply -f kubernetes/namespace.yaml
kubectl apply -f kubernetes/deployment.yaml
kubectl apply -f kubernetes/service.yaml
kubectl apply -f kubernetes/ingress.yaml

echo "Waiting for rollout..."
kubectl rollout status deployment/$PROJECT_NAME -n $PROJECT_NAME

echo "Verifying resources..."
kubectl get pods,svc,ingress -n $PROJECT_NAME

echo "-------------------------------------------------------"
echo "Deployment Complete!"
echo "Application URL: http://tour-management.example.com"
echo "-------------------------------------------------------"
