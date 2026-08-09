using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Interfaces.Services;
namespace TourManagement.Web.Pages.Bookings;
public sealed class EditModel(IBookingService service) : PageModel { [BindProperty] public BookingUpdateDto Input { get; set; } = new(); public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken) { var b = await service.GetByIdAsync(id, cancellationToken); if (b is null) return NotFound(); Input = new BookingUpdateDto { TourId = b.TourId, CustomerEmail = b.CustomerEmail, CustomerName = b.CustomerName, Status = b.Status }; return Page(); } public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken) { if (!ModelState.IsValid) return Page(); await service.UpdateAsync(id, Input, cancellationToken); return RedirectToPage("Index"); } }
