# Tour Management

This project has been modernized from ASP.NET Web Forms 4.7.2 to ASP.NET Core Razor Pages on .NET 8.

## Architecture

The application follows clean architecture principles with Domain, Application, Infrastructure, and Web folders under `src`.

## Running

Use a .NET 8 SDK and run the web project from this directory. Configuration is supplied by `src/TourManagement.Web/appsettings.json` and defaults to an in-memory database for portable execution.

## Migration Notes

Legacy Web Forms pages, Web.config, packages.config, System.Web dependencies, and ADO.NET page-level SQL access were replaced with Razor Pages, EF Core 8, dependency injection, ASP.NET Core Identity, and structured logging.
