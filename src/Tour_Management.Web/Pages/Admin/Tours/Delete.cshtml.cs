using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Application.DTOs;
using Tour_Management.Application.Interfaces;

namespace Tour_Management.Web.Pages.Admin.Tours;

/// <summary>Page model for deleting a tour (admin).</summary>
public class DeleteModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<DeleteModel> _logger;

    /// <summary>Gets or sets the tour to delete.</summary>
    public TourDto? Tour { get; set; }

    /// <summary>Initializes a new instance of <see cref="DeleteModel"/>.</summary>
    public DeleteModel(ITourService tourService, ILogger<DeleteModel> logger)
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

    /// <summary>Handles POST requests for deleting a tour.</summary>
    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToPage("/Account/AdminLogin");

        try
        {
            await _tourService.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Tour deleted: {TourId}", id);
            TempData["SuccessMessage"] = "Tour deleted successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour {TourId}", id);
            TempData["ErrorMessage"] = "Error deleting tour.";
        }

        return RedirectToPage("/Admin/Tours/Index");
    }
}
