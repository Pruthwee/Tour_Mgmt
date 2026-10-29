@echo off & setlocal enabledelayedexpansion
rem Deploys the pushed TESTING_CON_5 image to an AWS EKS cluster with kubectl.
rem Run from the repository root  scripts\deploy-image.bat  (needs the AWS CLI, kubectl and PowerShell)

set "APP_NAME=testing-con-5"
set "NAMESPACE=testing-con-5"
set "DEPLOYMENT_FILE=kubernetes\deployment.yaml"
set "DEPLOYMENT_BACKUP=%TEMP%\testing-con-5-deployment.yaml.orig"
set "TMP_FILE=%TEMP%\testing-con-5-deploy.tmp"

where aws >nul 2>&1
if !ERRORLEVEL! neq 0 (
  echo The AWS CLI is required
  exit /b 1
)
where kubectl >nul 2>&1
if !ERRORLEVEL! neq 0 (
  echo kubectl is required
  exit /b 1
)

echo === Deployment of !APP_NAME! to AWS EKS ===
set /p AWS_REGION="Enter the AWS region (for example us-east-1) "
set /p CLUSTER_NAME="Enter the EKS cluster name "
set /p IMAGE_URI="Enter the image URI (registry path and tag) "
if "!AWS_REGION!"=="" (
  echo The AWS region is required
  exit /b 1
)
if "!CLUSTER_NAME!"=="" (
  echo The cluster name is required
  exit /b 1
)
if "!IMAGE_URI!"=="" (
  echo The image URI is required
  exit /b 1
)

set /p S3_BUCKET_NAME="Enter value for S3_BUCKET_NAME (or press Enter to skip) "
set /p TOUR_PICS_BASE_URL="Enter value for TOUR_PICS_BASE_URL, the public base URL of the tour pictures ending with a slash (or press Enter to skip) "
set /p IRSA_ROLE_ARN="Enter the IAM role ARN for the S3 access of the pods, IRSA (or press Enter to skip) "

rem The connection string is read without echo through PowerShell
powershell -NoProfile -Command "$s = Read-Host 'Enter value for ConnectionStrings__dbconnection, the SQL Server connection string' -AsSecureString; [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($s))" > "!TMP_FILE!"
set /p DB_CONNECTION_STRING=<"!TMP_FILE!"
del "!TMP_FILE!" >nul 2>&1
if "!DB_CONNECTION_STRING!"=="" (
  echo The database connection string is required
  exit /b 1
)

echo Configuring kubectl for the cluster !CLUSTER_NAME! ...
aws eks update-kubeconfig --region !AWS_REGION! --name !CLUSTER_NAME!
if !ERRORLEVEL! neq 0 (echo aws eks update-kubeconfig failed & exit /b 1)
kubectl cluster-info
if !ERRORLEVEL! neq 0 (echo kubectl cannot reach the cluster & exit /b 1)

echo Checking that the cluster has Windows nodes ...
kubectl get nodes -l kubernetes.io/os=windows
if !ERRORLEVEL! neq 0 (echo Could not list the Windows nodes & exit /b 1)

echo Applying the namespace ...
kubectl apply -f kubernetes\namespace.yaml
if !ERRORLEVEL! neq 0 (echo Namespace failed & exit /b 1)

echo Creating the service account ...
kubectl create serviceaccount !APP_NAME!-sa -n !NAMESPACE! --dry-run=client -o yaml | kubectl apply -f -
if !ERRORLEVEL! neq 0 (echo Service account failed & exit /b 1)
if not "!IRSA_ROLE_ARN!"=="" (
  kubectl annotate serviceaccount !APP_NAME!-sa -n !NAMESPACE! "eks.amazonaws.com/role-arn=!IRSA_ROLE_ARN!" --overwrite
  if !ERRORLEVEL! neq 0 (echo Service account annotation failed & exit /b 1)
)

echo Creating the secret !APP_NAME!-secrets ...
kubectl create secret generic !APP_NAME!-secrets -n !NAMESPACE! "--from-literal=ConnectionStrings__dbconnection=!DB_CONNECTION_STRING!" --dry-run=client -o yaml | kubectl apply -f -
if !ERRORLEVEL! neq 0 (echo Secret failed & exit /b 1)
set "DB_CONNECTION_STRING="

echo Filling in the placeholders of !DEPLOYMENT_FILE! ...
copy /Y "!DEPLOYMENT_FILE!" "!DEPLOYMENT_BACKUP!" >nul
set "NAMESPACE_VALUE=!NAMESPACE!"
powershell -NoProfile -Command "$p = 'kubernetes\deployment.yaml'; $c = [IO.File]::ReadAllText($p); $c = $c.Replace('{{IMAGE_URI}}', $env:IMAGE_URI).Replace('{{NAMESPACE}}', $env:NAMESPACE_VALUE).Replace('{{S3_BUCKET_NAME}}', $env:S3_BUCKET_NAME).Replace('{{AWS_REGION}}', $env:AWS_REGION).Replace('{{TOUR_PICS_BASE_URL}}', $env:TOUR_PICS_BASE_URL); [IO.File]::WriteAllText($p, $c)"
if !ERRORLEVEL! neq 0 (
  echo Filling in the placeholders failed
  copy /Y "!DEPLOYMENT_BACKUP!" "!DEPLOYMENT_FILE!" >nul
  exit /b 1
)

echo Applying the deployment, the service and the ingress ...
kubectl apply -f kubernetes\deployment.yaml
set "DEPLOY_RESULT=!ERRORLEVEL!"
copy /Y "!DEPLOYMENT_BACKUP!" "!DEPLOYMENT_FILE!" >nul
del "!DEPLOYMENT_BACKUP!" >nul 2>&1
if !DEPLOY_RESULT! neq 0 (
  echo Deployment failed
  goto :rollback_help
)
kubectl apply -f kubernetes\service.yaml
if !ERRORLEVEL! neq 0 (
  echo Service failed
  goto :rollback_help
)
kubectl apply -f kubernetes\ingress.yaml
if !ERRORLEVEL! neq 0 (
  echo Ingress failed
  goto :rollback_help
)

echo Waiting for the rollout (Windows images are large, this can take several minutes) ...
kubectl rollout status deployment/!APP_NAME! -n !NAMESPACE! --timeout=900s
if !ERRORLEVEL! neq 0 (
  echo The rollout did not finish
  goto :rollback_help
)

kubectl get pods,svc,ingress -n !NAMESPACE!

kubectl get ingress !APP_NAME!-ingress -n !NAMESPACE! -o "jsonpath={.status.loadBalancer.ingress[0].hostname}" > "!TMP_FILE!" 2>nul
set "INGRESS_HOST="
set /p INGRESS_HOST=<"!TMP_FILE!"
del "!TMP_FILE!" >nul 2>&1
if not "!INGRESS_HOST!"=="" (
  echo Application URL http://!INGRESS_HOST! - the ingress rule answers for the host !APP_NAME!.example.com, send that Host header or create a DNS record to the load balancer
) else (
  echo The load balancer address is not ready yet, run kubectl get ingress -n !NAMESPACE!
)
echo Deployment finished.
endlocal
exit /b 0

:rollback_help
echo.
echo To go back to the previous version
echo   kubectl rollout undo deployment/!APP_NAME! -n !NAMESPACE!
echo   kubectl rollout status deployment/!APP_NAME! -n !NAMESPACE!
echo To look into the failure
echo   kubectl describe pods -n !NAMESPACE! -l app=!APP_NAME!
echo   kubectl logs -n !NAMESPACE! -l app=!APP_NAME! --tail=100
endlocal
exit /b 1
