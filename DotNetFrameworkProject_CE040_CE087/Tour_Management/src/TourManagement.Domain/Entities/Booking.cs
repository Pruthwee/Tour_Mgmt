namespace TourManagement.Domain.Entities;

/// <summary>Represents a tour booking made by a customer.</summary>
public sealed class Booking : BaseEntity
{
    public int TourId { get; set; }
    public Tour? Tour { get; set; }
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending";
}
