using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(256);
        builder.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.LastName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Gender).HasMaxLength(30);
        builder.Property(c => c.Street).HasMaxLength(200);
        builder.Property(c => c.City).HasMaxLength(100);
        builder.Property(c => c.State).HasMaxLength(100);
        builder.HasIndex(c => c.Email).IsUnique();
    }
}
