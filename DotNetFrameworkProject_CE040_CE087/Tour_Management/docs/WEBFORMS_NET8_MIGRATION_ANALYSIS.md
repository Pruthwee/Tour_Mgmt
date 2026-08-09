# ASP.NET Web Forms to .NET 8 Migration Analysis

Analysis date: 2026-08-09
Module: Tour_Management
Current framework: ASP.NET Web Forms 4.7.2
Target framework: .NET 8
Rules applied: /studio-app/claude-workspace/versionUpgrade/136/_claude/upgrade-analysis-rules.json

## Inventory

- Web Forms pages: 12 (`AddTour.aspx`, `AdminLogin2.aspx`, `AdminProfile.aspx`, `allbooking.aspx`, `DisplayTours.aspx`, `MainProfilePage.aspx`, `mybooking.aspx`, `Order.aspx`, `SignUpForm.aspx`, `TourCrud.aspx`, `usercrud.aspx`, `userlogin.aspx`)
- Code-behind files: 12
- User controls: 0
- Master pages: 0
- Global.asax files: 0
- Primary configuration: `Web.config`
- Package manifest: `packages.config`

## Migration blockers and recommendations

1. The project file is a non-SDK ASP.NET Web Application targeting .NET Framework 4.7.2 and references `System.Web`. This is a critical blocker for .NET 8. Replace it with SDK-style clean architecture projects targeting `net8.0`.
2. `Web.config` contains ASP.NET Web Forms runtime configuration, `system.web`, CodeDom providers, Chart handlers, and connection strings. Migrate configuration to `appsettings.json` and startup/middleware to `Program.cs`.
3. All `.aspx` pages use Web Forms directives, server controls, `runat="server"`, server-side events, and designer-generated `System.Web.UI` controls. Migrate pages to Razor Pages or MVC views.
4. Code-behind classes inherit from `System.Web.UI.Page` and use the Web Forms lifecycle (`Page_Load`, `Page.IsPostBack`). Migrate lifecycle logic to Razor Page handlers (`OnGetAsync`, `OnPostAsync`) and application services.
5. Several code-behind files use ADO.NET directly (`SqlConnection`, `SqlCommand`, `ConfigurationManager`). Migrate data access to EF Core 8 repositories or Dapper behind infrastructure abstractions.
6. `userlogin.aspx.cs` builds SQL with string concatenation from user input. This is a critical SQL injection issue and must be replaced with ASP.NET Core Identity or parameterized repository queries.
7. Passwords are stored and compared as plaintext, and admin credentials are hard-coded. Migrate to ASP.NET Core Identity with hashed passwords, policies, and roles.
8. `SqlDataSource` and `GridView` controls are used for CRUD and data binding in markup. Replace with Razor Pages, strongly typed view models, EF Core services, and explicit page handlers.
9. `Server.MapPath`, `Response.Redirect`, `Response.Write`, and `Server.Transfer` are Web Forms/System.Web APIs. Replace with `IWebHostEnvironment`, `RedirectToPage`, TempData, validation summaries, and endpoint routing.
10. LocalDB `.mdf` files under `App_Data` and absolute developer paths in connection strings are not production-ready for .NET 8 deployment. Move database configuration to environment-specific connection strings and EF Core migrations.

## Component complexity

- Complex: `AddTour.aspx`, `SignUpForm.aspx`, `userlogin.aspx`, `TourCrud.aspx`, `DisplayTours.aspx`, `mybooking.aspx`, `allbooking.aspx`, `Order.aspx`
- Medium: `AdminLogin2.aspx`, `AdminProfile.aspx`, `MainProfilePage.aspx`, `usercrud.aspx`
- Simple: none

## Target migration outline

Create the following clean architecture structure per the applied rules:

- `src/TourManagement.Domain`: entities (`Tour`, `Booking`, `ApplicationUser`), repository and service interfaces, domain exceptions.
- `src/TourManagement.Application`: DTOs, validators, AutoMapper profiles, service implementations, application DI.
- `src/TourManagement.Infrastructure`: EF Core 8 `DbContext`, entity configurations, repositories, data migrations.
- `src/TourManagement.Web`: Razor Pages, view models, manual view model mapping, static files, authentication, Program.cs.
- `tests/TourManagement.UnitTests` and `tests/TourManagement.IntegrationTests`.

## Notes

This document was added as an analysis artifact because the requested primary objective is a migration compatibility analysis report, not a compilable full migration implementation. No build commands were run.