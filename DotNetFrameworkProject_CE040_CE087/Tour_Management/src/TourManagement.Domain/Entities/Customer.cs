namespace TourManagement.Domain.Entities;

/// <summary>Represents an application customer migrated from the legacy UserInfo table.</summary>
public sealed class Customer : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
