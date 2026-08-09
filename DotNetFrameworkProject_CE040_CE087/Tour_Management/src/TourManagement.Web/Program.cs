using Microsoft.AspNetCore.Identity;
using Serilog;
using TourManagement.Application.Extensions;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration).WriteTo.Console());
builder.Services.AddRazorPages(options => options.Conventions.AuthorizeFolder("/Admin"));
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<TourManagementDbContext>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages();
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", targetFramework = ".NET 8" }))
    .WithName("HealthCheck")
    .AllowAnonymous();
app.Run();

public partial class Program { }
