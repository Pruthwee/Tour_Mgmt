using System.ComponentModel.DataAnnotations;

namespace Tour_Management.Web.ViewModels;

/// <summary>ViewModel for displaying a booking in a list.</summary>
public class BookingListViewModel
{
    public int BookingId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}

/// <summary>ViewModel for displaying booking details.</summary>
public class BookingDetailsViewModel
{
    public int BookingId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>ViewModel for creating a new booking (Order page).</summary>
public class BookingCreateViewModel
{
    [Required(ErrorMessage = "Your name is required.")]
    [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
    [Display(Name = "Your Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Your city is required.")]
    [StringLength(200, ErrorMessage = "City must not exceed 200 characters.")]
    [Display(Name = "Your City")]
    public string Place { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tour name is required.")]
    [StringLength(200, ErrorMessage = "Tour name must not exceed 200 characters.")]
    [Display(Name = "Tour Name")]
    public string TourName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mobile number is required.")]
    [StringLength(100, ErrorMessage = "Mobile number must not exceed 100 characters.")]
    [Display(Name = "Mobile Number")]
    public string Email { get; set; } = string.Empty;
}

/// <summary>ViewModel for editing an existing booking.</summary>
public class BookingEditViewModel
{
    public int BookingId { get; set; }

    [Required(ErrorMessage = "Your name is required.")]
    [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
    [Display(Name = "Your Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Your city is required.")]
    [StringLength(200, ErrorMessage = "City must not exceed 200 characters.")]
    [Display(Name = "Your City")]
    public string Place { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tour name is required.")]
    [StringLength(200, ErrorMessage = "Tour name must not exceed 200 characters.")]
    [Display(Name = "Tour Name")]
    public string TourName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mobile number is required.")]
    [StringLength(100, ErrorMessage = "Mobile number must not exceed 100 characters.")]
    [Display(Name = "Mobile Number")]
    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}

/// <summary>ViewModel for deleting a booking (confirmation).</summary>
public class BookingDeleteViewModel
{
    public int BookingId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
