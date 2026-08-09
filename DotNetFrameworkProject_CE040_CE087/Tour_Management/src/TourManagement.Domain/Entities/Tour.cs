namespace TourManagement.Domain.Entities;

/// <summary>Represents a tour package offered to customers.</summary>
public sealed class Tour : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public int Days { get; set; }
    public decimal Price { get; set; }
    public string Locations { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? PictureFileName { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
