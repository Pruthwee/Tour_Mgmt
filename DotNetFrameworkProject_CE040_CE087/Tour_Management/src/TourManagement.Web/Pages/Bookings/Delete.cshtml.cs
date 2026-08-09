using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Interfaces.Services;
namespace TourManagement.Web.Pages.Bookings;
public sealed class DeleteModel(IBookingService service) : PageModel { public BookingDto? Booking { get; private set; } public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken) { Booking = await service.GetByIdAsync(id, cancellationToken); return Booking is null ? NotFound() : Page(); } public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken) { await service.DeleteAsync(id, cancellationToken); return RedirectToPage("Index"); } }
