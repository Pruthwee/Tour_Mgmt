using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Application.Interfaces;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Pages.Account;

/// <summary>Page model for the user profile page.</summary>
public class ProfileModel : PageModel
{
    private readonly IUserService _userService;
    private readonly IBookingService _bookingService;
    private readonly ILogger<ProfileModel> _logger;

    /// <summary>Gets or sets the user profile view model.</summary>
    public UserProfileViewModel? Profile { get; set; }

    /// <summary>Initializes a new instance of <see cref="ProfileModel"/>.</summary>
    public ProfileModel(IUserService userService, IBookingService bookingService, ILogger<ProfileModel> logger)
    {
        _userService = userService;
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>Handles GET requests.</summary>
    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
            return RedirectToPage("/Account/Login");

        try
        {
            var user = await _userService.GetByEmailAsync(email, cancellationToken);
            if (user == null)
                return RedirectToPage("/Account/Login");

            var bookings = await _bookingService.GetByUserEmailAsync(email, cancellationToken);

            Profile = new UserProfileViewModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                Dob = user.Dob,
                Street = user.Street,
                City = user.City,
                State = user.State,
                Bookings = bookings.Select(b => new BookingListViewModel
                {
                    BookingId = b.BookingId,
                    TourName = b.TourName,
                    Place = b.Place,
                    Email = b.Email,
                    FirstName = b.FirstName,
                    BookingDate = b.BookingDate
                }).ToList()
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading profile for {Email}", email);
            return RedirectToPage("/Error");
        }
    }
}
