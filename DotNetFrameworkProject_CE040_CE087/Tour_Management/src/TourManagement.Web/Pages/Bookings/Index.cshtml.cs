using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Interfaces.Services;
namespace TourManagement.Web.Pages.Bookings;
public sealed class IndexModel(IBookingService service) : PageModel { public IReadOnlyList<BookingDto> Bookings { get; private set; } = []; public async Task OnGetAsync(CancellationToken cancellationToken) => Bookings = await service.GetAllAsync(cancellationToken); }
