# Migration Notes

## Migrated Components

- Web Forms pages were replaced by Razor Pages.
- Code-behind database logic was moved to application services and EF Core repositories.
- `Web.config` settings were migrated to `appsettings.json`.
- System.Web usage was removed from active project compilation.
- Entity Framework Core packages were updated to version 8.0.0.
- ASP.NET Core Identity was configured as the authentication foundation.

## Key Differences

Razor Pages use model binding and dependency injection rather than ViewState and server controls. Temporary state should use TempData, session, or persistent storage.
