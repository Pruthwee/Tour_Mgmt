#!/bin/bash
set -e
set -o pipefail

echo "AWS EKS Deployment Script"

read -p "Enter AWS Region: " AWS_REGION
read -p "Enter EKS Cluster Name: " CLUSTER_NAME
read -p "Enter Docker Image URI (full path with tag): " IMAGE_URI

# Prompt for application-specific environment variables
read -p "Enter value for DATABASE_CONNECTION_STRING (or press Enter to skip): " DB_CONN

# Update manifests
sed -i "s|{{IMAGE_URI}}|$IMAGE_URI|g" kubernetes/deployment.yaml
if [ -n "$DB_CONN" ]; then
    sed -i "s|{{DATABASE_CONNECTION_STRING}}|$DB_CONN|g" kubernetes/deployment.yaml
fi

# Configure kubectl
aws eks update-kubeconfig --region $AWS_REGION --name $CLUSTER_NAME

# Verify connectivity
kubectl cluster-info || { echo "Cluster connectivity failed"; exit 1; }

# Apply manifests
echo "Applying Kubernetes manifests..."
kubectl apply -f kubernetes/namespace.yaml
kubectl apply -f kubernetes/deployment.yaml
kubectl apply -f kubernetes/service.yaml
kubectl apply -f kubernetes/ingress.yaml

# Wait for rollout
echo "Waiting for rollout..."
kubectl rollout status deployment/tour-management -n tour-management

# Verify resources
kubectl get pods,svc,ingress -n tour-management

echo "Deployment complete. Please check the ingress URL for access."
