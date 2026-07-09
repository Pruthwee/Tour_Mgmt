@echo off
setlocal enabledelayedexpansion

set PROJECT_NAME=tour-management

echo -------------------------------------------------------
echo AWS EKS Deployment Script
echo -------------------------------------------------------

set /p AWS_REGION="Enter AWS Region [us-east-1]: "
if "!AWS_REGION!"=="" set AWS_REGION=us-east-1

set /p CLUSTER_NAME="Enter EKS Cluster Name: "
if "!CLUSTER_NAME!"=="" (
    echo Cluster name is required.
    exit /b 1
)

set /p IMAGE_URI="Enter Docker Image URI: "
if "!IMAGE_URI!"=="" (
    echo Image URI is required.
    exit /b 1
)

set /p DB_CONNECTION_STRING="Enter DB Connection String (or press Enter to skip): "

echo Configuring kubectl...
aws eks update-kubeconfig --region !AWS_REGION! --name !CLUSTER_NAME!
if !ERRORLEVEL! neq 0 (
    echo Failed to update kubeconfig
    exit /b 1
)

echo Verifying cluster connectivity...
kubectl cluster-info
if !ERRORLEVEL! neq 0 (
    echo Cluster connectivity failed
    exit /b 1
)

echo Updating manifests...
powershell -Command "(Get-Content kubernetes/deployment.yaml) -replace '{{IMAGE_URI}}', '!IMAGE_URI!' -replace '{{DB_CONNECTION_STRING}}', '!DB_CONNECTION_STRING!' | Set-Content kubernetes/deployment.yaml"

echo Applying manifests...
kubectl apply -f kubernetes/namespace.yaml
kubectl apply -f kubernetes/deployment.yaml
kubectl apply -f kubernetes/service.yaml
kubectl apply -f kubernetes/ingress.yaml

echo Waiting for rollout...
kubectl rollout status deployment/!PROJECT_NAME! -n !PROJECT_NAME!

echo Verifying resources...
kubectl get pods,svc,ingress -n !PROJECT_NAME!

echo -------------------------------------------------------
echo Deployment Complete!
echo Application URL: http://tour-management.example.com
echo -------------------------------------------------------
