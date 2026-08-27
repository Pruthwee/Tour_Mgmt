using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Application.DTOs;
using Tour_Management.Application.Interfaces;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Pages.Bookings;

/// <summary>Page model for creating a new booking.</summary>
public class CreateModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ITourService _tourService;
    private readonly ILogger<CreateModel> _logger;

    /// <summary>Gets or sets the booking form input.</summary>
    [BindProperty]
    public BookingFormViewModel Input { get; set; } = new();

    /// <summary>Gets or sets the error message to display.</summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>Initializes a new instance of <see cref="CreateModel"/>.</summary>
    public CreateModel(IBookingService bookingService, ITourService tourService, ILogger<CreateModel> logger)
    {
        _bookingService = bookingService;
        _tourService = tourService;
        _logger = logger;
    }

    /// <summary>Handles GET requests.</summary>
    public async Task<IActionResult> OnGetAsync(int? tourId, CancellationToken cancellationToken)
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
            return RedirectToPage("/Account/Login");

        Input.Email = email;
        Input.FirstName = HttpContext.Session.GetString("UserFirstName") ?? string.Empty;

        if (tourId.HasValue)
        {
            var tour = await _tourService.GetByIdAsync(tourId.Value, cancellationToken);
            if (tour != null)
            {
                Input.TourId = tour.TourId;
                Input.TourName = tour.TourName;
                Input.Place = tour.Place;
            }
        }

        return Page();
    }

    /// <summary>Handles POST requests for creating a booking.</summary>
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            var dto = new BookingCreateDto
            {
                TourName = Input.TourName,
                Place = Input.Place,
                Email = Input.Email,
                FirstName = Input.FirstName,
                TourId = Input.TourId
            };

            await _bookingService.CreateAsync(dto, cancellationToken);
            _logger.LogInformation("Booking created for user {Email}", Input.Email);
            TempData["SuccessMessage"] = "Booking confirmed successfully!";
            return RedirectToPage("/Bookings/MyBookings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for {Email}", Input.Email);
            ErrorMessage = "An error occurred while creating the booking. Please try again.";
            return Page();
        }
    }
}
