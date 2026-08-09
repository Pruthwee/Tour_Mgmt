using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Interfaces.Services;
namespace TourManagement.Web.Pages.Tours;
public sealed class DeleteModel(ITourService service) : PageModel
{
    public TourDto? Tour { get; private set; }
    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken) { Tour = await service.GetByIdAsync(id, cancellationToken); return Tour is null ? NotFound() : Page(); }
    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken) { await service.DeleteAsync(id, cancellationToken); return RedirectToPage("Index"); }
}
