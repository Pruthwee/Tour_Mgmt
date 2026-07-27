@echo off
setlocal enabledelayedexpansion

echo AWS EKS Deployment Script

set /p AWS_REGION="Enter AWS Region: "
set /p CLUSTER_NAME="Enter EKS Cluster Name: "
set /p IMAGE_URI="Enter Docker Image URI (full path with tag): "

set /p DB_CONN="Enter value for DATABASE_CONNECTION_STRING (or press Enter to skip): "

:: Use PowerShell to replace placeholders in YAML files
powershell -Command "(Get-Content kubernetes/deployment.yaml) -replace '{{IMAGE_URI}}', '%IMAGE_URI%' -replace '{{DATABASE_CONNECTION_STRING}}', '%DB_CONN%' | Set-Content kubernetes/deployment.yaml"

aws eks update-kubeconfig --region %AWS_REGION% --name %CLUSTER_NAME%

kubectl cluster-info
if %ERRORLEVEL% neq 0 (
    echo Cluster connectivity failed & exit /b 1
)

echo Applying Kubernetes manifests...
kubectl apply -f kubernetes/namespace.yaml
kubectl apply -f kubernetes/deployment.yaml
kubectl apply -f kubernetes/service.yaml
kubectl apply -f kubernetes/ingress.yaml

echo Waiting for rollout...
kubectl rollout status deployment/tour-management -n tour-management

kubectl get pods,svc,ingress -n tour-management

echo Deployment complete.
