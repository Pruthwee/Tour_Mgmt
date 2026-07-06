# Tour Management - .NET 8 Migration

## Overview
This project has been migrated from ASP.NET Web Forms (.NET 4.7.2) to .NET 8 using clean architecture principles.

## Architecture
The solution follows Clean Architecture with four layers:

```
Tour_Management/
├── src/
│   ├── Tour_Management.Domain/          # Domain entities, interfaces, DTOs, exceptions
│   ├── Tour_Management.Application/     # Business logic, services, validators, AutoMapper
│   ├── Tour_Management.Infrastructure/  # EF Core, repositories, data configurations
│   └── Tour_Management.Web/             # Razor Pages, ViewModels, static files
├── tests/
│   ├── Tour_Management.UnitTests/       # xUnit unit tests with Moq
│   └── Tour_Management.IntegrationTests/ # Integration tests with in-memory DB
└── docs/                                # Documentation
```

## Setup Instructions

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full instance)

### Configuration
1. Update the connection string in `src/Tour_Management.Web/appsettings.json`
2. Run EF Core migrations:
   ```bash
   cd src/Tour_Management.Web
   dotnet ef database update
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

## Default Admin Credentials
- Email: `admin@gmail.com`
- Password: `admin`

## Key Features
- Tour management (CRUD)
- User registration and login
- Tour booking system
- Admin dashboard
- Session-based authentication

## Build Verification
```bash
dotnet build
```
Build Status: ✅ SUCCESS (0 errors)
