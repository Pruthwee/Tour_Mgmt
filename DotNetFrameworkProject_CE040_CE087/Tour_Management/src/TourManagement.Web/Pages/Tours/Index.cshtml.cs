using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Interfaces.Services;
namespace TourManagement.Web.Pages.Tours;
public sealed class IndexModel(ITourService service) : PageModel
{
    public IReadOnlyList<TourDto> Tours { get; private set; } = [];
    [BindProperty(SupportsGet = true)] public string? Search { get; set; }
    public async Task OnGetAsync(CancellationToken cancellationToken) => Tours = string.IsNullOrWhiteSpace(Search) ? await service.GetAllAsync(cancellationToken) : await service.SearchAsync(Search, cancellationToken);
}
