# Build Stage
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 AS build
WORKDIR /app

# Copy solution and project files for NuGet restore
COPY *.sln .
COPY Tour_Management/Tour_Management.csproj .
COPY Tour_Management/packages.config .

# Restore NuGet packages
RUN nuget restore Tour_Management.csproj

# Copy the rest of the source code
COPY . .

# Build the project in Release mode
RUN msbuild Tour_Management.csproj /p:Configuration=Release /p:OutputPath=bin\Release

# Runtime Stage
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.7.2 AS runtime
WORKDIR /inetpub/wwwroot

# Copy build artifacts from the build stage
COPY --from=build /app/bin/Release .
COPY --from=build /app/Web.config .
COPY --from=build /app/*.aspx .
COPY --from=build /app/*.aspx.cs .
COPY --from=build /app/*.aspx.designer.cs .
COPY --from=build /app/App_Data ./App_Data
COPY --from=build /app/pics ./pics
COPY --from=build /app/Tour_pics ./Tour_pics

# Expose port 80 for IIS
EXPOSE 80

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
