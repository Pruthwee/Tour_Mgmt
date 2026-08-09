using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations;

public sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.CustomerEmail).IsRequired().HasMaxLength(256);
        builder.Property(b => b.CustomerName).IsRequired().HasMaxLength(200);
        builder.Property(b => b.Status).IsRequired().HasMaxLength(50);
        builder.HasOne(b => b.Tour).WithMany(t => t.Bookings).HasForeignKey(b => b.TourId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(b => b.Customer).WithMany(c => c.Bookings).HasForeignKey(b => b.CustomerId).OnDelete(DeleteBehavior.SetNull);
    }
}
