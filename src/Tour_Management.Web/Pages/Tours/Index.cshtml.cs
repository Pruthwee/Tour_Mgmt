using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Interfaces.Services;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Pages.Tours;

/// <summary>Page model for the tours list page.</summary>
public class IndexModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ITourService tourService, ILogger<IndexModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    public IEnumerable<TourListViewModel> Tours { get; set; } = Enumerable.Empty<TourListViewModel>();
    public string? SearchTerm { get; set; }

    public async Task OnGetAsync(string? searchTerm, CancellationToken cancellationToken)
    {
        try
        {
            SearchTerm = searchTerm;
            var dtos = string.IsNullOrWhiteSpace(searchTerm)
                ? await _tourService.GetAllAsync(cancellationToken)
                : await _tourService.SearchAsync(searchTerm, cancellationToken);

            // Manual mapping from DTO to ViewModel
            Tours = dtos.Select(dto => new TourListViewModel
            {
                TourId = dto.TourId,
                TourName = dto.TourName,
                Place = dto.Place,
                Days = dto.Days,
                Price = dto.Price,
                Locations = dto.Locations,
                Pic = dto.Pic
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tours list");
            Tours = Enumerable.Empty<TourListViewModel>();
        }
    }
}
