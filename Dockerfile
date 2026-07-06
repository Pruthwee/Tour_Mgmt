# =============================================================================
# Tour_Management - ASP.NET Web Forms Application (.NET Framework 4.7.2)
# Multi-stage Dockerfile for Windows Container (IIS)
# Runtime Base Image: mcr.microsoft.com/dotnet/framework/runtime:4.7.2
# =============================================================================

# ---- Build Stage ----
FROM mcr.microsoft.com/dotnet/framework/sdk:4.7.2 AS builder

WORKDIR /src

# Copy solution and project files first for layer caching
COPY Tour_Management.sln ./
COPY DotNetFrameworkProject_CE040_CE087/Tour_Management/Tour_Management.csproj ./DotNetFrameworkProject_CE040_CE087/Tour_Management/
COPY DotNetFrameworkProject_CE040_CE087/Tour_Management/packages.config ./DotNetFrameworkProject_CE040_CE087/Tour_Management/

# Restore NuGet packages
RUN nuget restore Tour_Management.sln

# Copy remaining source files
COPY . .

# Build and publish in Release configuration
RUN msbuild DotNetFrameworkProject_CE040_CE087/Tour_Management/Tour_Management.csproj \
    /p:Configuration=Release \
    /p:DeployOnBuild=true \
    /p:WebPublishMethod=FileSystem \
    /p:PublishUrl=C:\publish \
    /p:DeleteExistingFiles=True \
    /p:PrecompileBeforePublish=true \
    /p:EnableUpdateable=false

# ---- Runtime Stage ----
FROM mcr.microsoft.com/dotnet/framework/runtime:4.7.2 AS runtime

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

# Install IIS and required features
RUN Install-WindowsFeature -Name Web-Server, Web-Asp-Net45, NET-Framework-45-ASPNET, Web-Net-Ext45, Web-ISAPI-Ext, Web-ISAPI-Filter, Web-Default-Doc, Web-Static-Content, Web-Http-Errors, Web-Http-Logging -IncludeManagementTools

WORKDIR C:\inetpub\wwwroot

# Remove default IIS content
RUN Remove-Item -Recurse -Force C:\inetpub\wwwroot\* -ErrorAction SilentlyContinue

# Copy published application from build stage
COPY --from=builder C:\publish .

# Configure IIS application pool and site
RUN Import-Module WebAdministration; \
    Set-ItemProperty 'IIS:\AppPools\DefaultAppPool' -Name processModel.identityType -Value ApplicationPoolIdentity; \
    Set-ItemProperty 'IIS:\AppPools\DefaultAppPool' -Name managedRuntimeVersion -Value 'v4.0'; \
    Set-ItemProperty 'IIS:\Sites\Default Web Site' -Name physicalPath -Value 'C:\inetpub\wwwroot'

# Set environment variable for application environment
ENV ASPNET_ENV=Production
ENV TOUR_MANAGEMENT_ENV=Production

# Expose HTTP port
EXPOSE 80

# Start IIS service
ENTRYPOINT ["C:\\ServiceMonitor.exe", "w3svc"]
