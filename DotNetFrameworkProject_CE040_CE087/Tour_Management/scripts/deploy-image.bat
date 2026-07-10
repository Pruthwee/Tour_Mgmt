@echo off
setlocal enabledelayedexpansion

echo --- AWS ECS Fargate Deployment ---

set /p AWS_REGION="Enter AWS region (e.g., us-east-1): "
set /p CLUSTER_NAME="Enter ECS cluster name (e.g., my-ecs-cluster): "
set /p VPC_ID="Enter VPC ID (e.g., vpc-0abc123def456): "
set /p SUBNETS="Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): "
set /p SECURITY_GROUP="Enter Security Group ID (e.g., sg-0abc123def): "
set /p IMAGE_URI="Enter Docker image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/app:latest): "

for /f "tokens=*" %%i in ('aws sts get-caller-identity --query Account --output text') do set ACCOUNT_ID=%%i

aws ecs describe-clusters --clusters !CLUSTER_NAME! >nul 2>&1
if !ERRORLEVEL! neq 0 (
    aws ecs create-cluster --cluster-name !CLUSTER_NAME!
)

set /p LB_NEEDED="Do you need a load balancer for this service? (y/n): "
if "!LB_NEEDED!"=="y" (
    set /p TARGET_GROUP_ARN="Enter Target Group ARN: "
    :: Use PowerShell for sed-like replacement
    powershell -Command "(Get-Content ecs/service-definition.json) -replace '{{TARGET_GROUP_ARN}}', '!TARGET_GROUP_ARN!' | Set-Content ecs/service-definition.json"
) else (
    powershell -Command "(Get-Content ecs/service-definition.json) -replace '\"loadBalancers\": \[.*\],', '' | Set-Content ecs/service-definition.json"
)

powershell -Command "(Get-Content ecs/task-definition.json) -replace '{{IMAGE_URI}}', '!IMAGE_URI!' | Set-Content ecs/task-definition.json"
powershell -Command "(Get-Content ecs/task-definition.json) -replace '{{AWS_REGION}}', '!AWS_REGION!' | Set-Content ecs/task-definition.json"
powershell -Command "(Get-Content ecs/task-definition.json) -replace '{{ACCOUNT_ID}}', '!ACCOUNT_ID!' | Set-Content ecs/task-definition.json"
powershell -Command "(Get-Content ecs/service-definition.json) -replace '{{CLUSTER_NAME}}', '!CLUSTER_NAME!' | Set-Content ecs/service-definition.json"
powershell -Command "(Get-Content ecs/service-definition.json) -replace '{{SUBNET_1}}', '!SUBNETS!' | Set-Content ecs/service-definition.json"
powershell -Command "(Get-Content ecs/service-definition.json) -replace '{{SECURITY_GROUP}}', '!SECURITY_GROUP!' | Set-Content ecs/service-definition.json"

for /f "tokens=*" %%i in ('aws ecs register-task-definition --cli-input-json file://ecs/task-definition.json --query "taskDefinition.taskDefinitionArn" --output text') do set TASK_DEFINITION_ARN=%%i

set SERVICE_NAME=tour-management-service
aws ecs describe-services --cluster !CLUSTER_NAME! --services !SERVICE_NAME! --query "services[0].status" --output text > temp_status.txt
set /p SERVICE_STATUS=<temp_status.txt
del temp_status.txt

if "!SERVICE_STATUS!"=="ACTIVE" (
    echo Updating existing service...
    aws ecs update-service --cluster !CLUSTER_NAME! --service !SERVICE_NAME! --task-definition !TASK_DEFINITION_ARN!
) else (
    echo Creating new service...
    aws ecs create-service --cluster !CLUSTER_NAME! --service !SERVICE_NAME! --task-definition !TASK_DEFINITION_ARN! --launch-type FARGATE --network-configuration "awsvpcConfiguration={subnets=[!SUBNETS!],securityGroups=[!SECURITY_GROUP!],assignPublicIp=ENABLED}"
)

echo Waiting for service stability...
aws ecs wait services-stable --cluster !CLUSTER_NAME! --services !SERVICE_NAME!

echo Deployment complete!
echo CloudWatch log group: /ecs/tour-management
