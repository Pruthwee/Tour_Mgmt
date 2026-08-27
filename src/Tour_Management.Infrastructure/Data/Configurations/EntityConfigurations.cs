using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tour_Management.Domain.Entities;

namespace Tour_Management.Infrastructure.Data.Configurations;

/// <summary>EF Core configuration for UserInfo entity.</summary>
public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfo>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<UserInfo> builder)
    {
        builder.ToTable("UserInfo");
        builder.HasKey(u => u.Email);
        builder.Property(u => u.Email).HasMaxLength(50).IsRequired();
        builder.Property(u => u.FirstName).HasMaxLength(50).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(50).IsRequired();
        builder.Property(u => u.Gender).HasMaxLength(10).IsRequired();
        builder.Property(u => u.Password).HasMaxLength(100).IsRequired();
        builder.Property(u => u.Dob).IsRequired();
        builder.Property(u => u.Street).HasMaxLength(50).IsRequired();
        builder.Property(u => u.City).HasMaxLength(50).IsRequired();
        builder.Property(u => u.State).HasMaxLength(50).IsRequired();
        builder.Property(u => u.Role).HasMaxLength(20).HasDefaultValue("User");
        builder.Property(u => u.CreatedDate).HasDefaultValueSql("GETUTCDATE()");

        builder.HasCheckConstraint("CK_Gender", "[Gender] = 'Male' OR [Gender] = 'Female'");

        builder.HasMany(u => u.Bookings)
               .WithOne(b => b.User)
               .HasForeignKey(b => b.Email)
               .HasPrincipalKey(u => u.Email)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>EF Core configuration for Tour entity.</summary>
public class TourConfiguration : IEntityTypeConfiguration<Tour>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Tour> builder)
    {
        builder.ToTable("Tour");
        builder.HasKey(t => t.TourId);
        builder.Property(t => t.TourId).HasColumnName("TOUR_ID").UseIdentityColumn();
        builder.Property(t => t.TourName).HasColumnName("TOUR_NAME").HasMaxLength(20).IsRequired();
        builder.Property(t => t.Place).HasColumnName("PLACE").HasMaxLength(20).IsRequired();
        builder.Property(t => t.Days).HasColumnName("DAYS").IsRequired();
        builder.Property(t => t.Price).HasColumnName("PRICE").HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(t => t.Locations).HasColumnName("LOCATIONS").HasMaxLength(100).IsRequired();
        builder.Property(t => t.TourInfo).HasColumnName("TOUR_INFO").HasMaxLength(200).IsRequired();
        builder.Property(t => t.Pic).HasColumnName("pic").HasMaxLength(200);
        builder.Property(t => t.IsActive).HasDefaultValue(true);
        builder.Property(t => t.CreatedDate).HasDefaultValueSql("GETUTCDATE()");

        builder.HasMany(t => t.Bookings)
               .WithOne(b => b.Tour)
               .HasForeignKey(b => b.TourId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}

/// <summary>EF Core configuration for Booking entity.</summary>
public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("booking");
        builder.HasKey(b => b.BookingId);
        builder.Property(b => b.BookingId).HasColumnName("TOUR_ID").UseIdentityColumn();
        builder.Property(b => b.TourName).HasColumnName("TOUR_NAME").HasMaxLength(50);
        builder.Property(b => b.Place).HasColumnName("PLACE").HasMaxLength(50);
        builder.Property(b => b.Email).HasColumnName("Email").HasMaxLength(50);
        builder.Property(b => b.FirstName).HasColumnName("FirstName").HasMaxLength(50);
        builder.Property(b => b.TourId).HasColumnName("TourId");
        builder.Property(b => b.BookingDate).HasDefaultValueSql("GETUTCDATE()");
        builder.Property(b => b.IsActive).HasDefaultValue(true);
    }
}
