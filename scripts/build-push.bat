@echo off & setlocal enabledelayedexpansion
rem Builds the TESTING_CON_5 Windows container image and pushes it to AWS ECR or Docker Hub.
rem Run from the repository root  scripts\build-push.bat  (needs docker in Windows containers mode, and the AWS CLI for ECR)

set "PROJECT_NAME=TESTING_CON_5"
set "TMP_FILE=%TEMP%\build-push-name.txt"
powershell -NoProfile -Command "('!PROJECT_NAME!').ToLower() -replace '[^a-z0-9]+','-' -replace '^-+|-+$',''" > "!TMP_FILE!"
set /p IMAGE_NAME=<"!TMP_FILE!"

echo === Build and push of !IMAGE_NAME! ===
echo Select the registry
echo   1. AWS ECR (Amazon Elastic Container Registry)
echo   2. Docker Hub
set /p REGISTRY_CHOICE="Enter 1 or 2 "

if "!REGISTRY_CHOICE!"=="1" (
  where aws >nul 2>&1
  if !ERRORLEVEL! neq 0 (
    echo The AWS CLI is required for ECR
    exit /b 1
  )
  set /p AWS_REGION="Enter the AWS region (for example us-east-1) "
  set /p AWS_ACCOUNT_ID="Enter the AWS account ID "
  set /p ECR_REPO="Enter the ECR repository name (press Enter for !IMAGE_NAME!) "
  if "!ECR_REPO!"=="" set "ECR_REPO=!IMAGE_NAME!"
  if "!AWS_REGION!"=="" (
    echo The AWS region is required
    exit /b 1
  )
  if "!AWS_ACCOUNT_ID!"=="" (
    echo The AWS account ID is required
    exit /b 1
  )
  set "REGISTRY_URL=!AWS_ACCOUNT_ID!.dkr.ecr.!AWS_REGION!.amazonaws.com"
  set "REPO_PATH=!ECR_REPO!"
) else if "!REGISTRY_CHOICE!"=="2" (
  set /p DOCKER_USERNAME="Enter the Docker Hub username "
  set /p DOCKER_PASSWORD="Enter the Docker Hub password or access token "
  if "!DOCKER_USERNAME!"=="" (
    echo The Docker Hub username is required
    exit /b 1
  )
  if "!DOCKER_PASSWORD!"=="" (
    echo The Docker Hub password is required
    exit /b 1
  )
  set "REPO_PATH=!DOCKER_USERNAME!/!IMAGE_NAME!"
) else (
  echo Invalid choice, enter 1 or 2
  exit /b 1
)

set /p IMAGE_TAG="Enter the image tag (press Enter for latest) "
set "IMAGE_TAG_IN=!IMAGE_TAG!"
powershell -NoProfile -Command "$t = ('!IMAGE_TAG_IN!').ToLower() -replace '[^a-z0-9._-]+','-' -replace '^-+|-+$',''; if ($t -eq '') { $t = 'latest' }; $t" > "!TMP_FILE!"
set /p IMAGE_TAG=<"!TMP_FILE!"
del "!TMP_FILE!" >nul 2>&1

if "!REGISTRY_CHOICE!"=="1" (
  set "FULL_IMAGE_NAME=!REGISTRY_URL!/!REPO_PATH!:!IMAGE_TAG!"
) else (
  set "FULL_IMAGE_NAME=!REPO_PATH!:!IMAGE_TAG!"
)

echo Building the image !FULL_IMAGE_NAME! ...
docker build -f Dockerfile -t !FULL_IMAGE_NAME! .
if !ERRORLEVEL! neq 0 (echo Docker build failed & exit /b 1)
echo Build finished.

if "!REGISTRY_CHOICE!"=="1" (
  echo Logging in to AWS ECR ...
  aws ecr get-login-password --region !AWS_REGION! | docker login --username AWS --password-stdin !REGISTRY_URL!
  if !ERRORLEVEL! neq 0 (echo ECR login failed & exit /b 1)
  aws ecr describe-repositories --repository-names !ECR_REPO! --region !AWS_REGION! >nul 2>&1
  if !ERRORLEVEL! neq 0 (echo Creating ECR repository... & aws ecr create-repository --repository-name !ECR_REPO! --region !AWS_REGION!)
  if !ERRORLEVEL! neq 0 (echo ECR repository creation failed & exit /b 1)
) else (
  echo Logging in to Docker Hub ...
  echo !DOCKER_PASSWORD! | docker login --username !DOCKER_USERNAME! --password-stdin
  if !ERRORLEVEL! neq 0 (echo Docker Hub login failed & exit /b 1)
)

echo Pushing !FULL_IMAGE_NAME! ...
docker push !FULL_IMAGE_NAME!
if !ERRORLEVEL! neq 0 (echo Docker push failed & exit /b 1)

echo Done. Image pushed !FULL_IMAGE_NAME!
echo Use this image URI with scripts\deploy-image.bat
endlocal
