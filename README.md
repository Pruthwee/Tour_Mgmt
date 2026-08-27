# Tour Management - .NET 8 Migration

## Overview
This is the migrated Tour Management application, converted from ASP.NET Web Forms 4.7.2 to .NET 8 using clean architecture principles.

## Architecture
The solution follows Clean Architecture with four layers:

- **Tour_Management.Domain** - Entities, repository interfaces, domain exceptions
- **Tour_Management.Application** - Services, DTOs, AutoMapper profiles, validators, service interfaces
- **Tour_Management.Infrastructure** - EF Core DbContext, repository implementations, entity configurations
- **Tour_Management.Web** - Razor Pages UI, ViewModels, Program.cs

## Setup Instructions

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full instance)

### Configuration
1. Update the connection string in `src/Tour_Management.Web/appsettings.json`
2. Run EF Core migrations:
   ```bash
   cd src/Tour_Management.Web
   dotnet ef migrations add InitialCreate --project ../Tour_Management.Infrastructure
   dotnet ef database update --project ../Tour_Management.Infrastructure
   ```

### Running the Application
```bash
cd src/Tour_Management.Web
dotnet run
```

### Running Tests
```bash
dotnet test
```

## Migration Notes
- Web Forms pages migrated to Razor Pages
- ADO.NET replaced with Entity Framework Core 8.0
- System.Web replaced with ASP.NET Core equivalents
- Session-based authentication (simple, can be upgraded to ASP.NET Core Identity)
- Web.config replaced with appsettings.json
- Global.asax replaced with Program.cs

## Default Admin Credentials
- Email: admin@gmail.com
- Password: admin
