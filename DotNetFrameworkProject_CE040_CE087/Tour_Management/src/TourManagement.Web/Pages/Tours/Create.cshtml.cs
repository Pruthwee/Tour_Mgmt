using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Interfaces.Services;
namespace TourManagement.Web.Pages.Tours;
public sealed class CreateModel(ITourService service) : PageModel
{
    [BindProperty] public TourCreateDto Input { get; set; } = new();
    public void OnGet() { }
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    { if (!ModelState.IsValid) return Page(); await service.CreateAsync(Input, cancellationToken); return RedirectToPage("Index"); }
}
