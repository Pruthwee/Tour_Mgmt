using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations;

public sealed class TourConfiguration : IEntityTypeConfiguration<Tour>
{
    public void Configure(EntityTypeBuilder<Tour> builder)
    {
        builder.ToTable("Tours");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired().HasMaxLength(150);
        builder.Property(t => t.Place).IsRequired().HasMaxLength(150);
        builder.Property(t => t.Price).HasPrecision(18, 2);
        builder.Property(t => t.Locations).IsRequired().HasMaxLength(500);
        builder.Property(t => t.Description).IsRequired().HasMaxLength(4000);
        builder.Property(t => t.PictureFileName).HasMaxLength(255);
        builder.HasIndex(t => t.Name);
        builder.HasData(new Tour { Id = 1, Name = "Goa Beach Escape", Place = "Goa", Days = 5, Price = 29999, Locations = "North Goa, South Goa", Description = "A relaxing beach tour with curated local experiences.", PictureFileName = "goa.jpg", CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "migration" });
    }
}
