# escape=`
# TESTING_CON_5 - ASP.NET Web Forms (.NET Framework 4.7.2) application on IIS, Windows container.
# Build stage: Windows Server Core LTSC 2019 SDK image with MSBuild, NuGet and the web publishing targets.
# Runtime stage: mcr.microsoft.com/dotnet/framework/aspnet:4.7.2 (IIS + ServiceMonitor). The host OS version must match the image.

FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019 AS build
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]
WORKDIR /src

# Restore first (cached while the solution, the project and packages.config do not change)
COPY DotNetFrameworkProject_CE040_CE087/Tour_Management.sln DotNetFrameworkProject_CE040_CE087/
COPY DotNetFrameworkProject_CE040_CE087/Tour_Management/Tour_Management.csproj DotNetFrameworkProject_CE040_CE087/Tour_Management/
COPY DotNetFrameworkProject_CE040_CE087/Tour_Management/packages.config DotNetFrameworkProject_CE040_CE087/Tour_Management/
WORKDIR /src/DotNetFrameworkProject_CE040_CE087
RUN nuget restore Tour_Management.sln

# Source, then one publish to a folder, without debug symbols
WORKDIR /src
COPY DotNetFrameworkProject_CE040_CE087/ DotNetFrameworkProject_CE040_CE087/
WORKDIR /src/DotNetFrameworkProject_CE040_CE087/Tour_Management
RUN msbuild Tour_Management.csproj /p:Configuration=Release /p:DebugSymbols=false /p:DebugType=none /p:DeployOnBuild=true /p:WebPublishMethod=FileSystem /p:DeleteExistingFiles=true /p:publishUrl=C:\publish

FROM mcr.microsoft.com/dotnet/framework/aspnet:4.7.2 AS runtime
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]
WORKDIR /inetpub/wwwroot

# Only the published application, the static pictures and no source, symbols or local database files
COPY --from=build C:/publish/ ./
COPY --from=build C:/src/DotNetFrameworkProject_CE040_CE087/Tour_Management/pics/ ./pics/
COPY --from=build C:/src/DotNetFrameworkProject_CE040_CE087/Tour_Management/Tour_pics/ ./Tour_pics/
RUN Get-ChildItem -Path C:\inetpub\wwwroot -Recurse -Include *.pdb,*.cs,*.csproj,*.mdf,*.ldf | Remove-Item -Force

# Configuration comes from the environment (ServiceMonitor hands the container variables to the IIS application pool)
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 80

# The base image's ENTRYPOINT (ServiceMonitor.exe w3svc) keeps IIS in the foreground and stops it cleanly on docker stop
