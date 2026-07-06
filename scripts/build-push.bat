@echo off
setlocal enabledelayedexpansion

:: =============================================================================
:: build-push.bat - Build and Push Docker Image for Tour_Management
:: ASP.NET Web Forms (.NET Framework 4.7.2) - Windows Container
:: =============================================================================

set "PROJECT_NAME=tour-management"

echo ==============================================
echo   Tour_Management - Docker Build ^& Push
echo ==============================================
echo.

:: Prompt for image tag
set /p "IMAGE_TAG_INPUT=Enter image tag (press Enter for 'latest'): "
if "!IMAGE_TAG_INPUT!"=="" (
    set "IMAGE_TAG=latest"
) else (
    set "IMAGE_TAG=!IMAGE_TAG_INPUT!"
)
echo Using image tag: !IMAGE_TAG!
echo.

:: Prompt for registry type
echo Select container registry:
echo   1. AWS ECR (Elastic Container Registry)
echo   2. Docker Hub
set /p "REGISTRY_CHOICE=Enter choice (1 or 2): "
echo.

if "!REGISTRY_CHOICE!"=="1" goto :ecr_setup
if "!REGISTRY_CHOICE!"=="2" goto :dockerhub_setup
echo ERROR: Invalid registry choice. Please enter 1 or 2.
exit /b 1

:ecr_setup
echo --- AWS ECR Configuration ---
set /p "AWS_REGION=Enter AWS Region (e.g., us-east-1): "
set /p "AWS_ACCOUNT_ID=Enter AWS Account ID: "
set /p "ECR_REPO_INPUT=Enter ECR Repository name (default: tour-management): "
if "!ECR_REPO_INPUT!"=="" (
    set "ECR_REPO=tour-management"
) else (
    set "ECR_REPO=!ECR_REPO_INPUT!"
)

set "REGISTRY_URL=!AWS_ACCOUNT_ID!.dkr.ecr.!AWS_REGION!.amazonaws.com"
set "FULL_IMAGE_NAME=!REGISTRY_URL!/!ECR_REPO!:!IMAGE_TAG!"

echo.
echo Logging in to AWS ECR...
aws ecr get-login-password --region !AWS_REGION! | docker login --username AWS --password-stdin !REGISTRY_URL!
if !ERRORLEVEL! neq 0 (
    echo ERROR: ECR login failed. Check your AWS credentials and region.
    exit /b 1
)
echo ECR login successful.

echo Checking if ECR repository '!ECR_REPO!' exists...
aws ecr describe-repositories --repository-names !ECR_REPO! --region !AWS_REGION! >nul 2>&1
if !ERRORLEVEL! neq 0 (
    echo Creating ECR repository...
    aws ecr create-repository --repository-name !ECR_REPO! --region !AWS_REGION!
    if !ERRORLEVEL! neq 0 (
        echo ERROR: Failed to create ECR repository.
        exit /b 1
    )
)
echo ECR repository ready.
goto :build_image

:dockerhub_setup
echo --- Docker Hub Configuration ---
set /p "DOCKER_USERNAME=Enter Docker Hub username: "
set /p "DOCKER_PASSWORD=Enter Docker Hub password or access token: "
set /p "DOCKER_REPO_INPUT=Enter Docker Hub repository name (default: tour-management): "
if "!DOCKER_REPO_INPUT!"=="" (
    set "DOCKER_REPO=tour-management"
) else (
    set "DOCKER_REPO=!DOCKER_REPO_INPUT!"
)

set "FULL_IMAGE_NAME=!DOCKER_USERNAME!/!DOCKER_REPO!:!IMAGE_TAG!"

echo.
echo Logging in to Docker Hub...
echo !DOCKER_PASSWORD! | docker login --username !DOCKER_USERNAME! --password-stdin
if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker Hub login failed. Check your credentials.
    exit /b 1
)
echo Docker Hub login successful.
goto :build_image

:build_image
echo.
echo Building Docker image: !FULL_IMAGE_NAME!
echo NOTE: This is a Windows container image. Ensure Docker is configured for Windows containers.
echo.

docker build -f Dockerfile -t "!FULL_IMAGE_NAME!" .
if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker build failed.
    exit /b 1
)
echo Docker build successful.

echo.
echo Pushing image: !FULL_IMAGE_NAME!
docker push "!FULL_IMAGE_NAME!"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Docker push failed.
    exit /b 1
)

echo.
echo ==============================================
echo   SUCCESS: Image pushed successfully!
echo   Image: !FULL_IMAGE_NAME!
echo ==============================================

endlocal
exit /b 0
