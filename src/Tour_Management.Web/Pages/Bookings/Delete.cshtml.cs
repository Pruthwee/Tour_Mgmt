using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Interfaces.Services;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Pages.Bookings;

/// <summary>Page model for deleting/cancelling a booking.</summary>
public class DeleteModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(IBookingService bookingService, ILogger<DeleteModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    [BindProperty]
    public BookingDeleteViewModel Booking { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserEmail") == null)
            return RedirectToPage("/Users/Login");

        try
        {
            var dto = await _bookingService.GetByIdAsync(id, cancellationToken);
            if (dto is null)
                return NotFound();

            // Manual mapping from DTO to ViewModel
            Booking = new BookingDeleteViewModel
            {
                BookingId = dto.BookingId,
                TourName = dto.TourName,
                FirstName = dto.FirstName,
                Email = dto.Email
            };
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking for delete, ID {BookingId}", id);
            return RedirectToPage("./MyBookings");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserEmail") == null)
            return RedirectToPage("/Users/Login");

        try
        {
            await _bookingService.DeleteAsync(Booking.BookingId, cancellationToken);
            TempData["SuccessMessage"] = "Booking cancelled successfully!";
            return RedirectToPage("./MyBookings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking with ID {BookingId}", Booking.BookingId);
            TempData["ErrorMessage"] = "An error occurred while cancelling the booking.";
            return RedirectToPage("./MyBookings");
        }
    }
}
