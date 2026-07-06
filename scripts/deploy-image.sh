#!/bin/bash
# =============================================================================
# deploy-image.sh - Deploy Tour_Management to AWS EKS
# ASP.NET Web Forms (.NET Framework 4.7.2) - Windows Node Pool Required
# =============================================================================
set -e
set -o pipefail

APP_NAME="tour-management"
NAMESPACE="tour-management"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"

echo "=============================================="
echo "  Tour_Management - Deploy to AWS EKS"
echo "=============================================="
echo ""

# ---- Prompt for AWS / EKS configuration ----
read -p "Enter AWS Region (e.g., us-east-1): " AWS_REGION
if [ -z "$AWS_REGION" ]; then
  echo "ERROR: AWS Region is required."
  exit 1
fi

read -p "Enter EKS Cluster Name: " CLUSTER_NAME
if [ -z "$CLUSTER_NAME" ]; then
  echo "ERROR: EKS Cluster Name is required."
  exit 1
fi

read -p "Enter full Docker Image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/tour-management:latest): " IMAGE_URI
if [ -z "$IMAGE_URI" ]; then
  echo "ERROR: Docker Image URI is required."
  exit 1
fi

echo ""
echo "--- Application Environment Variables ---"
echo "The Tour_Management application requires a SQL Server connection string."
echo ""
read -p "Enter DB_CONNECTION_STRING (or press Enter to skip - update deployment.yaml manually): " DB_CONNECTION_STRING

echo ""
echo "--- Configuring kubectl for EKS cluster ---"
aws eks update-kubeconfig --region "$AWS_REGION" --name "$CLUSTER_NAME"
if [ $? -ne 0 ]; then
  echo "ERROR: Failed to configure kubectl. Check your AWS credentials and cluster name."
  exit 1
fi

echo "Verifying cluster connectivity..."
kubectl cluster-info || { echo "ERROR: Cannot connect to EKS cluster."; exit 1; }

echo ""
echo "--- Updating Kubernetes manifests ---"

# Work on copies to avoid modifying originals
cp -r "$ROOT_DIR/kubernetes" /tmp/tour-management-k8s

# Replace IMAGE_URI placeholder
sed -i 's|{{IMAGE_URI}}|'"$IMAGE_URI"'|g' /tmp/tour-management-k8s/deployment.yaml

# Replace DB_CONNECTION_STRING placeholder if provided
if [ -n "$DB_CONNECTION_STRING" ]; then
  sed -i 's|{{DB_CONNECTION_STRING}}|'"$DB_CONNECTION_STRING"'|g' /tmp/tour-management-k8s/deployment.yaml
else
  echo "WARNING: DB_CONNECTION_STRING not provided. Update deployment.yaml manually before applying."
  sed -i 's|{{DB_CONNECTION_STRING}}|PLACEHOLDER_UPDATE_REQUIRED|g' /tmp/tour-management-k8s/deployment.yaml
fi

echo ""
echo "--- Applying Kubernetes manifests ---"

echo "Applying namespace..."
kubectl apply -f /tmp/tour-management-k8s/namespace.yaml

echo "Applying deployment..."
kubectl apply -f /tmp/tour-management-k8s/deployment.yaml

echo "Applying service..."
kubectl apply -f /tmp/tour-management-k8s/service.yaml

echo "Applying ingress..."
kubectl apply -f /tmp/tour-management-k8s/ingress.yaml

echo ""
echo "--- Waiting for deployment rollout ---"
echo "NOTE: Windows containers may take longer to start (60-120 seconds)."
kubectl rollout status deployment/$APP_NAME -n $NAMESPACE --timeout=300s
if [ $? -ne 0 ]; then
  echo "ERROR: Deployment rollout failed or timed out."
  echo "Run: kubectl describe pods -n $NAMESPACE"
  echo "Run: kubectl logs -l app=$APP_NAME -n $NAMESPACE"
  exit 1
fi

echo ""
echo "--- Verifying deployed resources ---"
kubectl get pods,svc,ingress -n $NAMESPACE

echo ""
echo "--- Application Access ---"
INGRESS_HOST=$(kubectl get ingress ${APP_NAME}-ingress -n $NAMESPACE -o jsonpath='{.spec.rules[0].host}' 2>/dev/null || echo "tour-management.example.com")
ALB_ADDRESS=$(kubectl get ingress ${APP_NAME}-ingress -n $NAMESPACE -o jsonpath='{.status.loadBalancer.ingress[0].hostname}' 2>/dev/null || echo "pending")

echo "Ingress Host:    http://$INGRESS_HOST"
echo "ALB Address:     $ALB_ADDRESS"
echo "Health Check:    http://$INGRESS_HOST/HealthCheck.ashx"
echo ""
echo "If ALB address is 'pending', wait a few minutes for AWS to provision the load balancer."
echo ""

# Cleanup temp files
rm -rf /tmp/tour-management-k8s

echo "=============================================="
echo "  SUCCESS: Tour_Management deployed to EKS!"
echo "=============================================="
echo ""
echo "Rollback command (if needed):"
echo "  kubectl rollout undo deployment/$APP_NAME -n $NAMESPACE"
