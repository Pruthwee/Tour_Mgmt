@echo off
setlocal enabledelayedexpansion

:: =============================================================================
:: deploy-image.bat - Deploy Tour_Management to AWS EKS
:: ASP.NET Web Forms (.NET Framework 4.7.2) - Windows Node Pool Required
:: =============================================================================

set "APP_NAME=tour-management"
set "NAMESPACE=tour-management"

echo ==============================================
echo   Tour_Management - Deploy to AWS EKS
echo ==============================================
echo.

:: ---- Prompt for AWS / EKS configuration ----
set /p "AWS_REGION=Enter AWS Region (e.g., us-east-1): "
if "!AWS_REGION!"=="" (
    echo ERROR: AWS Region is required.
    exit /b 1
)

set /p "CLUSTER_NAME=Enter EKS Cluster Name: "
if "!CLUSTER_NAME!"=="" (
    echo ERROR: EKS Cluster Name is required.
    exit /b 1
)

set /p "IMAGE_URI=Enter full Docker Image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/tour-management:latest): "
if "!IMAGE_URI!"=="" (
    echo ERROR: Docker Image URI is required.
    exit /b 1
)

echo.
echo --- Application Environment Variables ---
echo The Tour_Management application requires a SQL Server connection string.
echo.
set /p "DB_CONNECTION_STRING=Enter DB_CONNECTION_STRING (or press Enter to skip): "

echo.
echo --- Configuring kubectl for EKS cluster ---
aws eks update-kubeconfig --region !AWS_REGION! --name !CLUSTER_NAME!
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to configure kubectl. Check your AWS credentials and cluster name.
    exit /b 1
)

echo Verifying cluster connectivity...
kubectl cluster-info
if !ERRORLEVEL! neq 0 (
    echo ERROR: Cannot connect to EKS cluster.
    exit /b 1
)

echo.
echo --- Copying manifests for modification ---
if exist "%TEMP%\tour-management-k8s" rmdir /s /q "%TEMP%\tour-management-k8s"
xcopy /s /e /i /q "..\kubernetes" "%TEMP%\tour-management-k8s"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to copy Kubernetes manifests.
    exit /b 1
)

echo --- Updating Kubernetes manifests ---

:: Replace IMAGE_URI placeholder using PowerShell
powershell -Command "(Get-Content '%TEMP%\tour-management-k8s\deployment.yaml') -replace '{{IMAGE_URI}}', '!IMAGE_URI!' | Set-Content '%TEMP%\tour-management-k8s\deployment.yaml'"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to update IMAGE_URI in deployment.yaml.
    exit /b 1
)

:: Replace DB_CONNECTION_STRING placeholder
if "!DB_CONNECTION_STRING!"=="" (
    echo WARNING: DB_CONNECTION_STRING not provided. Update deployment.yaml manually.
    powershell -Command "(Get-Content '%TEMP%\tour-management-k8s\deployment.yaml') -replace '{{DB_CONNECTION_STRING}}', 'PLACEHOLDER_UPDATE_REQUIRED' | Set-Content '%TEMP%\tour-management-k8s\deployment.yaml'"
) else (
    powershell -Command "(Get-Content '%TEMP%\tour-management-k8s\deployment.yaml') -replace '{{DB_CONNECTION_STRING}}', '!DB_CONNECTION_STRING!' | Set-Content '%TEMP%\tour-management-k8s\deployment.yaml'"
)

echo.
echo --- Applying Kubernetes manifests ---

echo Applying namespace...
kubectl apply -f "%TEMP%\tour-management-k8s\namespace.yaml"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to apply namespace.
    exit /b 1
)

echo Applying deployment...
kubectl apply -f "%TEMP%\tour-management-k8s\deployment.yaml"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to apply deployment.
    exit /b 1
)

echo Applying service...
kubectl apply -f "%TEMP%\tour-management-k8s\service.yaml"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to apply service.
    exit /b 1
)

echo Applying ingress...
kubectl apply -f "%TEMP%\tour-management-k8s\ingress.yaml"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to apply ingress.
    exit /b 1
)

echo.
echo --- Waiting for deployment rollout ---
echo NOTE: Windows containers may take longer to start (60-120 seconds).
kubectl rollout status deployment/!APP_NAME! -n !NAMESPACE! --timeout=300s
if !ERRORLEVEL! neq 0 (
    echo ERROR: Deployment rollout failed or timed out.
    echo Run: kubectl describe pods -n !NAMESPACE!
    echo Run: kubectl logs -l app=!APP_NAME! -n !NAMESPACE!
    exit /b 1
)

echo.
echo --- Verifying deployed resources ---
kubectl get pods,svc,ingress -n !NAMESPACE!

echo.
echo --- Application Access ---
echo Health Check: http://tour-management.example.com/HealthCheck.ashx
echo Update the ingress host in kubernetes/ingress.yaml with your actual domain.
echo.

:: Cleanup temp files
rmdir /s /q "%TEMP%\tour-management-k8s"

echo ==============================================
echo   SUCCESS: Tour_Management deployed to EKS!
echo ==============================================
echo.
echo Rollback command (if needed):
echo   kubectl rollout undo deployment/!APP_NAME! -n !NAMESPACE!

endlocal
exit /b 0
