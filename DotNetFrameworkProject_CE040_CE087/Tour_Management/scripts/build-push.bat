@echo off
setlocal enabledelayedexpansion

set PROJECT_NAME=Tour_Management

set /p IMAGE_TAG="Enter image tag (default: latest): "
if "!IMAGE_TAG!"=="" set IMAGE_TAG=latest

echo Select Registry:
echo 1. AWS ECR
echo 2. Docker Hub
set /p REGISTRY_CHOICE="Choice [1-2]: "

set "IMAGE_NAME=%PROJECT_NAME%"
set "IMAGE_NAME=%IMAGE_NAME: =-%"
:: Simple lowercase conversion for Windows
for /f "tokens=*" %%i in ('echo !IMAGE_NAME! ^| powershell -Command "$input.ToLower()"') do set IMAGE_NAME=%%i

if "!REGISTRY_CHOICE!"=="1" (
    set /p AWS_REGION="Enter AWS Region: "
    set /p ECR_REPO="Enter ECR Repository Name: "
    
    aws ecr describe-repositories --repository-names !ECR_REPO! --region !AWS_REGION! >nul 2>&1
    if !ERRORLEVEL! neq 0 (
        echo Creating ECR repository...
        aws ecr create-repository --repository-name !ECR_REPO! --region !AWS_REGION!
    )
    
    for /f "tokens=*" %%i in ('aws ecr describe-repositories --repository-names !ECR_REPO! --region !AWS_REGION! --query "repositories[0].repositoryUri" --output text') do set REGISTRY_URL=%%i
    
    aws ecr get-login-password --region !AWS_REGION! | docker login --username AWS --password-stdin !REGISTRY_URL!
    if !ERRORLEVEL! neq 0 (
        echo ECR login failed & exit /b 1
    )
    set FULL_IMAGE_NAME=!REGISTRY_URL!:!IMAGE_TAG!
) else (
    set /p DOCKER_USER="Enter Docker Hub Username: "
    set /p DOCKER_PASS="Enter Docker Hub Password: "
    echo !DOCKER_PASS! | docker login --username !DOCKER_USER! --password-stdin
    set FULL_IMAGE_NAME=!DOCKER_USER!/!IMAGE_NAME!:!IMAGE_TAG!
)

echo Building image !FULL_IMAGE_NAME!...
docker build -t !FULL_IMAGE_NAME! .
if !ERRORLEVEL! neq 0 (
    echo Docker build failed & exit /b 1
)

echo Pushing image !FULL_IMAGE_NAME!...
docker push !FULL_IMAGE_NAME!
if !ERRORLEVEL! neq 0 (
    echo Docker push failed & exit /b 1
)

echo Successfully built and pushed !FULL_IMAGE_NAME!
