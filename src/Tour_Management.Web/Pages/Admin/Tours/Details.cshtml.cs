using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Application.DTOs;
using Tour_Management.Application.Interfaces;

namespace Tour_Management.Web.Pages.Admin.Tours;

/// <summary>Page model for tour details (admin).</summary>
public class DetailsModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<DetailsModel> _logger;

    /// <summary>Gets or sets the tour to display.</summary>
    public TourDto? Tour { get; set; }

    /// <summary>Initializes a new instance of <see cref="DetailsModel"/>.</summary>
    public DetailsModel(ITourService tourService, ILogger<DetailsModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    /// <summary>Handles GET requests.</summary>
    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToPage("/Account/AdminLogin");

        Tour = await _tourService.GetByIdAsync(id, cancellationToken);
        if (Tour == null)
            return NotFound();

        return Page();
    }
}
