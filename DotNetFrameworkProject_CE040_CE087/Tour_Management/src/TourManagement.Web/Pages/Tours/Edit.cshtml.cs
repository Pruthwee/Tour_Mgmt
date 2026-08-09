using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Interfaces.Services;
namespace TourManagement.Web.Pages.Tours;
public sealed class EditModel(ITourService service) : PageModel
{
    [BindProperty] public TourUpdateDto Input { get; set; } = new();
    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    { var tour = await service.GetByIdAsync(id, cancellationToken); if (tour is null) return NotFound(); Input = new TourUpdateDto { Name = tour.Name, Place = tour.Place, Days = tour.Days, Price = tour.Price, Locations = tour.Locations, Description = tour.Description, PictureFileName = tour.PictureFileName }; return Page(); }
    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken)
    { if (!ModelState.IsValid) return Page(); await service.UpdateAsync(id, Input, cancellationToken); return RedirectToPage("Index"); }
}
