#!/bin/bash
# Deploys the pushed TESTING_CON_5 image to an AWS EKS cluster with kubectl.
# Run from the repository root:  ./scripts/deploy-image.sh   (needs the AWS CLI and kubectl)
set -e
set -o pipefail

APP_NAME="testing-con-5"
NAMESPACE="testing-con-5"
DEPLOYMENT_FILE="kubernetes/deployment.yaml"
DEPLOYMENT_BACKUP=$(mktemp)

# Keeps the manifest with its placeholders for the next run
cp "$DEPLOYMENT_FILE" "$DEPLOYMENT_BACKUP"
cleanup() {
  cp "$DEPLOYMENT_BACKUP" "$DEPLOYMENT_FILE" 2>/dev/null || true
  rm -f "$DEPLOYMENT_BACKUP" "$DEPLOYMENT_FILE.bak"
}
trap cleanup EXIT

rollback_help() {
  echo ""
  echo "Deployment failed. To go back to the previous version:"
  echo "  kubectl rollout undo deployment/$APP_NAME -n $NAMESPACE"
  echo "  kubectl rollout status deployment/$APP_NAME -n $NAMESPACE"
  echo "To look into the failure:"
  echo "  kubectl describe pods -n $NAMESPACE -l app=$APP_NAME"
  echo "  kubectl logs -n $NAMESPACE -l app=$APP_NAME --tail=100"
}

command -v aws >/dev/null 2>&1 || { echo "The AWS CLI is required"; exit 1; }
command -v kubectl >/dev/null 2>&1 || { echo "kubectl is required"; exit 1; }

echo "=== Deployment of $APP_NAME to AWS EKS ==="
read -r -p "Enter the AWS region (for example us-east-1): " AWS_REGION
read -r -p "Enter the EKS cluster name: " CLUSTER_NAME
read -r -p "Enter the image URI (registry path and tag, for example 123456789012.dkr.ecr.us-east-1.amazonaws.com/$APP_NAME:latest): " IMAGE_URI
if [ -z "$AWS_REGION" ] || [ -z "$CLUSTER_NAME" ] || [ -z "$IMAGE_URI" ]; then
  echo "The AWS region, the cluster name and the image URI are required"
  exit 1
fi

read -r -p "Enter value for S3_BUCKET_NAME (or press Enter to skip): " S3_BUCKET_NAME
read -r -p "Enter value for TOUR_PICS_BASE_URL, the public base URL of the tour pictures ending with / (or press Enter to skip): " TOUR_PICS_BASE_URL
read -r -p "Enter the IAM role ARN for the S3 access of the pods, IRSA (or press Enter to skip): " IRSA_ROLE_ARN
read -r -s -p "Enter value for ConnectionStrings__dbconnection, the SQL Server connection string (hidden): " DB_CONNECTION_STRING
echo
if [ -z "$DB_CONNECTION_STRING" ]; then
  echo "The database connection string is required"
  exit 1
fi

echo "Configuring kubectl for the cluster $CLUSTER_NAME ..."
aws eks update-kubeconfig --region "$AWS_REGION" --name "$CLUSTER_NAME" || { echo "aws eks update-kubeconfig failed"; exit 1; }
kubectl cluster-info || exit 1

echo "Checking that the cluster has Windows nodes ..."
kubectl get nodes -l kubernetes.io/os=windows || { echo "Could not list the Windows nodes"; exit 1; }

echo "Applying the namespace ..."
kubectl apply -f kubernetes/namespace.yaml || { echo "Namespace failed"; exit 1; }

echo "Creating the service account ..."
kubectl create serviceaccount "$APP_NAME-sa" -n "$NAMESPACE" --dry-run=client -o yaml | kubectl apply -f -
if [ -n "$IRSA_ROLE_ARN" ]; then
  kubectl annotate serviceaccount "$APP_NAME-sa" -n "$NAMESPACE" "eks.amazonaws.com/role-arn=$IRSA_ROLE_ARN" --overwrite
fi

echo "Creating the secret $APP_NAME-secrets ..."
kubectl create secret generic "$APP_NAME-secrets" -n "$NAMESPACE" --from-literal=ConnectionStrings__dbconnection="$DB_CONNECTION_STRING" --dry-run=client -o yaml | kubectl apply -f -
unset DB_CONNECTION_STRING

echo "Filling in the placeholders of $DEPLOYMENT_FILE ..."
sed -i.bak 's|{{IMAGE_URI}}|'"$IMAGE_URI"'|g' "$DEPLOYMENT_FILE"
sed -i.bak 's|{{NAMESPACE}}|'"$NAMESPACE"'|g' "$DEPLOYMENT_FILE"
sed -i.bak 's|{{S3_BUCKET_NAME}}|'"$S3_BUCKET_NAME"'|g' "$DEPLOYMENT_FILE"
sed -i.bak 's|{{AWS_REGION}}|'"$AWS_REGION"'|g' "$DEPLOYMENT_FILE"
sed -i.bak 's|{{TOUR_PICS_BASE_URL}}|'"$TOUR_PICS_BASE_URL"'|g' "$DEPLOYMENT_FILE"
rm -f "$DEPLOYMENT_FILE.bak"

echo "Applying the deployment, the service and the ingress ..."
kubectl apply -f kubernetes/deployment.yaml || { echo "Deployment failed"; rollback_help; exit 1; }
kubectl apply -f kubernetes/service.yaml || { echo "Service failed"; rollback_help; exit 1; }
kubectl apply -f kubernetes/ingress.yaml || { echo "Ingress failed"; rollback_help; exit 1; }

echo "Waiting for the rollout (Windows images are large, this can take several minutes) ..."
if ! kubectl rollout status deployment/$APP_NAME -n $NAMESPACE --timeout=900s; then
  echo "The rollout did not finish"
  rollback_help
  exit 1
fi

echo ""
kubectl get pods,svc,ingress -n "$NAMESPACE"

INGRESS_HOST=$(kubectl get ingress "$APP_NAME-ingress" -n "$NAMESPACE" -o jsonpath='{.status.loadBalancer.ingress[0].hostname}' 2>/dev/null || true)
echo ""
if [ -n "$INGRESS_HOST" ]; then
  echo "Application URL: http://$INGRESS_HOST (the ingress rule answers for the host $APP_NAME.example.com, send that Host header or create a DNS record to the load balancer)"
else
  echo "The load balancer address is not ready yet. Run: kubectl get ingress -n $NAMESPACE"
fi
echo "Deployment finished."
