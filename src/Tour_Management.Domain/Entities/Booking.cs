namespace Tour_Management.Domain.Entities;

/// <summary>
/// Represents a tour booking in the Tour Management system.
/// </summary>
public class Booking
{
    /// <summary>Gets or sets the booking identifier.</summary>
    public int BookingId { get; set; }

    /// <summary>Gets or sets the tour name at time of booking.</summary>
    public string TourName { get; set; } = string.Empty;

    /// <summary>Gets or sets the place/destination at time of booking.</summary>
    public string Place { get; set; } = string.Empty;

    /// <summary>Gets or sets the email of the user who booked.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the first name of the user who booked.</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Gets or sets the tour identifier (foreign key).</summary>
    public int? TourId { get; set; }

    /// <summary>Gets or sets the booking date.</summary>
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets whether the booking is active.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Navigation property: the tour that was booked.</summary>
    public Tour? Tour { get; set; }

    /// <summary>Navigation property: the user who made the booking.</summary>
    public UserInfo? User { get; set; }
}
