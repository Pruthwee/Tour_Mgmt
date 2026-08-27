using Microsoft.EntityFrameworkCore;
using Tour_Management.Domain.Entities;
using Tour_Management.Infrastructure.Data.Configurations;

namespace Tour_Management.Infrastructure.Data;

/// <summary>
/// Entity Framework Core database context for Tour Management.
/// </summary>
public class TourManagementDbContext : DbContext
{
    /// <summary>Initializes a new instance of <see cref="TourManagementDbContext"/>.</summary>
    public TourManagementDbContext(DbContextOptions<TourManagementDbContext> options) : base(options) { }

    /// <summary>Gets or sets the UserInfo DbSet.</summary>
    public DbSet<UserInfo> UserInfos { get; set; } = null!;

    /// <summary>Gets or sets the Tour DbSet.</summary>
    public DbSet<Tour> Tours { get; set; } = null!;

    /// <summary>Gets or sets the Booking DbSet.</summary>
    public DbSet<Booking> Bookings { get; set; } = null!;

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserInfoConfiguration());
        modelBuilder.ApplyConfiguration(new TourConfiguration());
        modelBuilder.ApplyConfiguration(new BookingConfiguration());
    }
}
