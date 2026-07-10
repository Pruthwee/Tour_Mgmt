#!/bin/bash
set -e
set -o pipefail

echo "--- AWS ECS Fargate Deployment ---"

# Prompts
read -p "Enter AWS region (e.g., us-east-1): " AWS_REGION
read -p "Enter ECS cluster name (e.g., my-ecs-cluster): " CLUSTER_NAME
read -p "Enter VPC ID (e.g., vpc-0abc123def456): " VPC_ID
read -p "Enter Subnet IDs comma-separated (e.g., subnet-0abc123,subnet-0def456): " SUBNETS
read -p "Enter Security Group ID (e.g., sg-0abc123def): " SECURITY_GROUP
read -p "Enter Docker image URI (e.g., 123456789.dkr.ecr.us-east-1.amazonaws.com/app:latest): " IMAGE_URI

# Get Account ID
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)

# Check/Create Cluster
aws ecs describe-clusters --clusters $CLUSTER_NAME >/dev/null 2>&1 || aws ecs create-cluster --cluster-name $CLUSTER_NAME

# Load Balancer Handling
read -p "Do you need a load balancer for this service? (y/n): " LB_NEEDED
if [ "$LB_NEEDED" == "y" ]; then
    echo "Creating Application Load Balancer and Target Group..."
    # This is a simplified representation of LB creation
    # In a real script, we would use aws elbv2 create-load-balancer etc.
    # For this artifact, we assume the user provides the ARN or we simulate the capture
    read -p "Enter Target Group ARN: " TARGET_GROUP_ARN
    sed -i 's|{{TARGET_GROUP_ARN}}'${TARGET_GROUP_ARN}'|g' ecs/service-definition.json
else
    # Remove loadBalancers section from service-definition.json
    sed -i '/"loadBalancers": \[.*\],/d' ecs/service-definition.json
fi

# Replace placeholders in JSON files
sed -i "s|{{IMAGE_URI}}|$IMAGE_URI|g" ecs/task-definition.json
sed -i "s|{{AWS_REGION}}|$AWS_REGION|g" ecs/task-definition.json
sed -i "s|{{ACCOUNT_ID}}|$ACCOUNT_ID|g" ecs/task-definition.json
sed -i "s|{{CLUSTER_NAME}}|$CLUSTER_NAME|g" ecs/service-definition.json
sed -i "s|{{SUBNET_1}}|$(echo $SUBNETS | cut -d',' -f1)|g" ecs/service-definition.json
sed -i "s|{{SUBNET_2}}|$(echo $SUBNETS | cut -d',' -f2)|g" ecs/service-definition.json
sed -i "s|{{SECURITY_GROUP}}|$SECURITY_GROUP|g" ecs/service-definition.json

# Register Task Definition
TASK_DEFINITION_ARN=$(aws ecs register-task-definition --cli-input-json file://ecs/task-definition.json --query 'taskDefinition.taskDefinitionArn' --output text)

# Service Existence Check
SERVICE_NAME="tour-management-service"
SERVICE_EXISTS=$(aws ecs describe-services --cluster $CLUSTER_NAME --services $SERVICE_NAME --query 'services[0].status' --output text)

if [ "$SERVICE_EXISTS" == "ACTIVE" ] || [ "$SERVICE_EXISTS" == "STEADY" ]; then
    echo "Updating existing service..."
    aws ecs update-service --cluster $CLUSTER_NAME --service $SERVICE_NAME --task-definition $TASK_DEFINITION_ARN
else
    echo "Creating new service..."
    aws ecs create-service --cluster $CLUSTER_NAME --service $SERVICE_NAME --task-definition $TASK_DEFINITION_ARN --launch-type FARGATE --network-configuration "awsvpcConfiguration={subnets=[$(echo $SUBNETS | sed 's/,/ /g')],securityGroups=[$SECURITY_GROUP],assignPublicIp=ENABLED}"
fi

echo "Waiting for service stability..."
aws ecs wait services-stable --cluster $CLUSTER_NAME --services $SERVICE_NAME

echo "Deployment complete!"
echo "CloudWatch log group: /ecs/tour-management"
