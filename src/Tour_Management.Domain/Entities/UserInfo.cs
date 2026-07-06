namespace Tour_Management.Domain.Entities;

/// <summary>
/// Represents a registered user in the system.
/// </summary>
public class UserInfo
{
    /// <summary>Gets or sets the unique identifier for the user.</summary>
    public int UserInfoId { get; set; }

    /// <summary>Gets or sets the email address of the user.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the first name of the user.</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Gets or sets the last name of the user.</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>Gets or sets the gender of the user.</summary>
    public string Gender { get; set; } = string.Empty;

    /// <summary>Gets or sets the hashed password of the user.</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>Gets or sets the date of birth of the user.</summary>
    public DateTime? Dob { get; set; }

    /// <summary>Gets or sets the street address of the user.</summary>
    public string Street { get; set; } = string.Empty;

    /// <summary>Gets or sets the city of the user.</summary>
    public string City { get; set; } = string.Empty;

    /// <summary>Gets or sets the state of the user.</summary>
    public string State { get; set; } = string.Empty;

    /// <summary>Gets or sets the date the user was created.</summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets whether the user account is active.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Navigation property for bookings made by this user.</summary>
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
