using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data;

/// <summary>EF Core database context replacing Web Forms ADO.NET access.</summary>
public sealed class TourManagementDbContext(DbContextOptions<TourManagementDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(TourManagementDbContext).Assembly);
    }
}
