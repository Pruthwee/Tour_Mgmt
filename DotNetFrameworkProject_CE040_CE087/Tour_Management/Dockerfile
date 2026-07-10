# Stage 1: Build
FROM mcr.microsoft.com/dotnet/framework/sdk:4.7.2-windowsservercore-ltsc2019 AS build
WORKDIR /app

# Copy solution and project files
COPY *.sln .
COPY Tour_Management/*.csproj ./Tour_Management/
COPY Tour_Management/packages.config ./Tour_Management/

# Restore NuGet packages
RUN nuget restore Tour_Management/Tour_Management.csproj

# Copy the rest of the source code
COPY Tour_Management/ ./Tour_Management/

# Build the project in Release mode
WORKDIR /app/Tour_Management
RUN msbuild Tour_Management.csproj /p:Configuration=Release /p:OutputPath=publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.7.2-windowsservercore-ltsc2019
WORKDIR /inetpub/wwwroot

# Copy published artifacts from build stage
COPY --from=build /app/Tour_Management/publish .

# Expose port 80 for IIS
EXPOSE 80

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
```