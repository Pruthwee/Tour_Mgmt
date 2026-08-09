using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;

namespace TourManagement.Infrastructure.Extensions;

/// <summary>Registers infrastructure services and EF Core dependencies.</summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString) || connectionString.Contains("InMemory", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<TourManagementDbContext>(options => options.UseInMemoryDatabase("TourManagement"));
        }
        else
        {
            services.AddDbContext<TourManagementDbContext>(options => options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));
        }
        services.AddScoped<ITourRepository, TourRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        return services;
    }
}
