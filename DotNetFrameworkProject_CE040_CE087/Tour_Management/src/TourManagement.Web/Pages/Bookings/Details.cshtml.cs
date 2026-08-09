using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Interfaces.Services;
namespace TourManagement.Web.Pages.Bookings;
public sealed class DetailsModel(IBookingService service) : PageModel { public BookingDto? Booking { get; private set; } public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken) { Booking = await service.GetByIdAsync(id, cancellationToken); return Booking is null ? NotFound() : Page(); } }
