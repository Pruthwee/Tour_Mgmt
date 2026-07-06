namespace Tour_Management.Domain.Entities;

/// <summary>
/// Represents a user booking for a tour.
/// </summary>
public class Booking
{
    /// <summary>Gets or sets the unique identifier for the booking.</summary>
    public int BookingId { get; set; }

    /// <summary>Gets or sets the name of the tour booked.</summary>
    public string TourName { get; set; } = string.Empty;

    /// <summary>Gets or sets the city/place of the customer.</summary>
    public string Place { get; set; } = string.Empty;

    /// <summary>Gets or sets the email/mobile number of the customer.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the first name of the customer.</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Gets or sets the date the booking was created.</summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets whether the booking is active.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Gets or sets the optional foreign key to the Tour entity.</summary>
    public int? TourId { get; set; }

    /// <summary>Navigation property for the associated tour.</summary>
    public Tour? Tour { get; set; }

    /// <summary>Gets or sets the optional foreign key to the UserInfo entity.</summary>
    public int? UserInfoId { get; set; }

    /// <summary>Navigation property for the associated user.</summary>
    public UserInfo? UserInfo { get; set; }
}
