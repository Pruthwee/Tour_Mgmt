# TESTING_CON_5 - Deployment guide (Docker, AWS ECR, AWS EKS)

TESTING_CON_5 is the Tour Management application: an ASP.NET Web Forms site on the .NET Framework 4.7.2 (IIS), with SQL Server as its database. It runs in a **Windows container** built on `mcr.microsoft.com/dotnet/framework/aspnet:4.7.2` (Windows Server Core LTSC 2019 family) and is deployed to **Windows nodes** of an AWS EKS cluster.

## 1. Prerequisites and system requirements

- Windows 10/11 or Windows Server with Docker running in **Windows containers** mode. The host OS build must match the image (the 4.7.2 image is Windows Server LTSC 2019 based; use LTSC 2019 hosts or Hyper-V isolation). Image sizes are several GB.
- Windows Server container images are subject to Microsoft licensing: the host must be licensed for Windows Server containers (EKS Windows nodes include the license through the Windows AMI).
- AWS CLI v2, `kubectl`, `eksctl` (cluster creation), Docker.
- An SQL Server database reachable from the cluster (external, not part of this repository; schema in `Database.txt`).
- An S3 bucket for the tour pictures (uploads no longer go to the container disk).
- IAM permissions: `ecr:*` on the repository (create, push), `eks:DescribeCluster`, `iam` permissions to create the IRSA role, and `s3:PutObject`/`s3:GetObject` on the bucket for the pods' role.

## 2. Local setup with Docker Compose

`docker-compose.yml` has one service, the application. It starts no database; SQL Server and S3 are reached through environment variables.

```powershell
$env:ConnectionStrings__dbconnection = "<SQL Server connection string>"   # credential, never written in files
$env:S3_BUCKET_NAME = "<bucket>"          # optional, upload target
$env:AWS_REGION = "us-east-1"
$env:TOUR_PICS_BASE_URL = ""              # optional base URL of the pictures, ending with /
docker compose up --build
```

The site answers on http://localhost:8080 and the health endpoint on http://localhost:8080/health. Logs of IIS go to `./logs`. AWS credentials for S3 outside the cluster come from the standard AWS SDK sources (environment variables or profile) that you add to the container; on EKS the pods use IRSA.

## 3. Build and push

Run from the repository root.

- Linux/macOS/Git Bash: `./scripts/build-push.sh`
- Windows: `scripts\build-push.bat`

The scripts ask for the registry (1 = AWS ECR, 2 = Docker Hub), then its details, then the image tag (empty = `latest`). For ECR they log in with `aws ecr get-login-password`, create the repository when missing and push. The image name is `testing-con-5`. The build always uses `docker build -f Dockerfile -t <registry>/<repo>:<tag> .`.

### The Dockerfile

- Build stage: `mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019` (NuGet restore of `Tour_Management.sln`, then MSBuild web publish to `C:\publish` with `DebugSymbols=false`).
- Runtime stage: `mcr.microsoft.com/dotnet/framework/aspnet:4.7.2`, files copied to `C:\inetpub\wwwroot`, `*.pdb`, source and local database files removed. The base image's `ServiceMonitor.exe` entry point runs IIS and handles shutdown. No HEALTHCHECK instruction: Kubernetes probes `/health`.
- `.dockerignore` removes bin/obj, `.git`, `*.md`, `.vs`, `*.user`, `*.suo`, `launchSettings.json` and the deployment folders from the build context.

## 4. AWS EKS deployment

### Prerequisites and cluster setup

```bash
aws configure
eksctl create cluster --name testing-con-5-cluster --region us-east-1 --managed=false --nodes 0   # or use an existing cluster
eksctl create nodegroup --cluster testing-con-5-cluster --region us-east-1 --name win-ng --node-ami-family WindowsServer2019FullContainer --nodes 2 --node-type t3.large
eksctl utils associate-iam-oidc-provider --cluster testing-con-5-cluster --region us-east-1 --approve
```

Windows support must be enabled on the cluster (VPC resource controller, `enable-windows-ipam` in `amazon-vpc-cni`), the Linux nodes run the cluster add-ons, and the **AWS Load Balancer Controller** must be installed for the ingress. Windows containers run only on Windows nodes, so a **Windows node group matching the image (Windows Server 2019 / LTSC 2019)** is required.

IRSA for S3: create an IAM policy for `s3:PutObject`, `s3:GetObject`, `s3:DeleteObject` on the bucket and a role trusted by the service account `testing-con-5-sa` in the namespace `testing-con-5`; give its ARN to the deploy script.

### kubectl configuration

```bash
aws eks update-kubeconfig --region <region> --name <cluster>
kubectl cluster-info
kubectl get nodes -l kubernetes.io/os=windows
```

### Deploy

`./scripts/deploy-image.sh` (or `scripts\deploy-image.bat`) asks for the region, the cluster name, the image URI, `S3_BUCKET_NAME`, `TOUR_PICS_BASE_URL`, the IRSA role ARN and (hidden) the SQL Server connection string. It configures kubectl, applies the namespace, creates the service account and the Secret `testing-con-5-secrets`, fills the `{{...}}` placeholders of `kubernetes/deployment.yaml`, applies the manifests in order, waits for `kubectl rollout status deployment/testing-con-5 -n testing-con-5` and shows the URL from the ingress. The manifest template is restored afterwards.

### The manifests

- `kubernetes/namespace.yaml`: namespace `testing-con-5`.
- `kubernetes/deployment.yaml`: 2 replicas, `nodeSelector kubernetes.io/os: windows`, container `testing-con-5` on port 80 with the image `{{IMAGE_URI}}`, environment placeholders (`S3_BUCKET_NAME`, `AWS_REGION`, `TOUR_PICS_BASE_URL`), the Secret key `ConnectionStrings__dbconnection`, requests 250m/512Mi, limits 500m/1Gi, liveness and readiness probes on `GET /health` (long initial delays, Windows containers start slowly).
- `kubernetes/service.yaml`: ClusterIP `testing-con-5-service`, port 80 to 80.
- `kubernetes/ingress.yaml`: ALB ingress (`kubernetes.io/ingress.class: alb`, `alb.ingress.kubernetes.io/scheme: internet-facing`), host `testing-con-5.example.com`, path `/` to the service.

### Health check endpoint

`GET /health` answers `{"status":"healthy"}` from the application's `HealthHandler` (registered in `Web.config`, system.webServer handlers). It does not touch the database. It is served by IIS for as long as the container runs.

### Troubleshooting

- Pod failures: `kubectl describe pod -n testing-con-5 -l app=testing-con-5`, `kubectl logs ...`. `Pending` with "no nodes available" means no Windows node matches (`nodeSelector`) or the Windows node group is missing. `ImagePullBackOff`: check the ECR permissions of the node role and the image URI. Image/host version mismatch shows as "container operating system does not match the host".
- Service problems: `kubectl get endpoints -n testing-con-5`; empty endpoints mean the pods are not Ready (probe `/health`).
- Ingress problems: `kubectl describe ingress -n testing-con-5`, the AWS Load Balancer Controller logs in `kube-system`; the ALB needs public subnets tagged `kubernetes.io/role/elb`. The rule answers for the host `testing-con-5.example.com`, so create a DNS record or send that Host header.
- Database errors: `ConnectionStrings__dbconnection` must reach the SQL Server (security groups); an empty value fails at `SqlConnection.Open`.
- Upload errors (`AccessDenied`): the OIDC provider, the role trust policy and the service account annotation.

### Scaling and management

```bash
kubectl scale deployment/testing-con-5 -n testing-con-5 --replicas=4
kubectl autoscale deployment testing-con-5 -n testing-con-5 --cpu-percent=60 --min=2 --max=10   # HPA, needs metrics-server
kubectl set image deployment/testing-con-5 testing-con-5=<new image uri> -n testing-con-5       # rolling update
kubectl rollout status deployment/testing-con-5 -n testing-con-5
kubectl rollout undo deployment/testing-con-5 -n testing-con-5                                  # rollback
eksctl scale nodegroup --cluster <cluster> --name win-ng --nodes 4
```

### Limits of Windows containers on Kubernetes

Windows pods only run on Windows nodes, images are large (slow pulls, so keep `imagePullPolicy: IfNotPresent`), the node OS version must match the image, no privileged containers, and `runAsNonRoot` is not available. ASP.NET in-process session state does not work across replicas.

## 5. Configuration management and environment variables

| Variable | Purpose | Where it comes from |
|---|---|---|
| `ConnectionStrings__dbconnection` | SQL Server connection string used by the code-behind pages | Kubernetes Secret `testing-con-5-secrets` (credential) |
| `S3_BUCKET_NAME` | bucket receiving the tour pictures | deployment placeholder |
| `AWS_REGION` | region of the S3 client | deployment placeholder |
| `TOUR_PICS_BASE_URL` | base URL (ending with `/`) from which the pages load the pictures; empty = `Tour_pics/` in the site | deployment placeholder |
| `ASPNETCORE_ENVIRONMENT` | `Production` | Dockerfile / manifest |

Pictures are stored in S3 under `Tour_pics/<file name>`; make them readable through a public bucket policy or CloudFront and set `TOUR_PICS_BASE_URL` to that base URL (for example `https://<distribution>/Tour_pics/`).

Known limit: the pages that use `SqlDataSource` controls (`DisplayTours`, `mybooking`, `usercrud`, `allbooking` and others) still read `ConnectionStrings:dbconnection` from `Web.config` (a LocalDB path of the original developer). They were not part of the blockers; to run them in the cluster, replace that connection string in `Web.config` or migrate them to the environment variable too.

## 6. Security considerations and best practices

- The connection string is a Kubernetes Secret, asked without echo by the scripts; never commit it. Consider AWS Secrets Manager with the External Secrets Operator.
- S3 access uses IRSA: no AWS keys in the image or the manifests. Keep the bucket private except for the picture path.
- `userlogin.aspx.cs` builds its SQL by string concatenation (SQL injection) and passwords are stored in clear text; these were not blockers and are not fixed here. Fix them before exposing the site.
- Terminate TLS at the ALB (add an ACM certificate annotation and a 443 listener); the ingress is internet-facing.
- Keep the Windows base image patched, scan it in ECR, limit the node security groups to the ALB.
- The container runs IIS with the application pool identity (Windows containers have no `USER` non-root switch like Linux).

## 7. .NET notes

- Runtime settings: IIS settings (application pool, request limits) are in `Web.config` and the IIS defaults of the base image; `compilation debug="true"` is set in `Web.config`, set it to `false` for production (`Web.Release.config` removes it only for a Visual Studio publish, which the container build does not run). Garbage collection: add `<gcServer enabled="true"/>` / `<gcConcurrent>` in `Web.config` runtime settings if needed; the container memory limit is 1Gi.
- Monitoring: Application Insights or CloudWatch Container Insights for Windows nodes; IIS logs go to `C:\inetpub\logs\LogFiles`.
- Security headers, HTTPS redirection and CORS are not configured in the application; add them in `Web.config` (`system.webServer/httpProtocol/customHeaders`, URL Rewrite) or at the ALB. Authentication is custom (the login pages); data protection (machine keys) must be identical on all replicas if view state or forms authentication is used: set a shared `machineKey`.
- The solution is a classic Web Forms project: it needs Windows MSBuild with the web publishing targets, so it was not compiled on the Linux workstation used to prepare these files; the Docker build on a Windows host is the first real build.
