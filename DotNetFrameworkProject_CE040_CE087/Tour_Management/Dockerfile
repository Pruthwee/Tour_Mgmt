# Stage 1: Build
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 AS build
WORKDIR /app

# Copy solution and project files for dependency restoration
COPY *.sln .
COPY Tour_Management/*.csproj ./Tour_Management/
COPY Tour_Management/packages.config ./Tour_Management/

# Restore NuGet packages
RUN nuget restore Tour_Management/Tour_Management.csproj

# Copy the rest of the source code
COPY . .

# Build the project in Release mode
WORKDIR /app/Tour_Management
RUN msbuild Tour_Management.csproj /p:Configuration=Release /p:OutputPath=c:\app\publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8
WORKDIR /inetpub/wwwroot

# Copy published artifacts from build stage
COPY --from=build /app/publish .

# Set environment variables for .NET Framework
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port 80 for IIS
EXPOSE 80
