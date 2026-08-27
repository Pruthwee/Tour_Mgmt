using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Application.DTOs;
using Tour_Management.Application.Interfaces;

namespace Tour_Management.Web.Pages.Admin.Users;

/// <summary>Page model for the admin users listing page.</summary>
public class IndexModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<IndexModel> _logger;

    /// <summary>Gets or sets the list of users.</summary>
    public IEnumerable<UserDto> Users { get; set; } = new List<UserDto>();

    /// <summary>Initializes a new instance of <see cref="IndexModel"/>.</summary>
    public IndexModel(IUserService userService, ILogger<IndexModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>Handles GET requests.</summary>
    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToPage("/Account/AdminLogin");

        try
        {
            Users = await _userService.GetAllAsync(cancellationToken);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading users");
            return RedirectToPage("/Error");
        }
    }

    /// <summary>Handles POST requests for deleting a user.</summary>
    public async Task<IActionResult> OnPostDeleteAsync(string email, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToPage("/Account/AdminLogin");

        try
        {
            await _userService.DeleteAsync(email, cancellationToken);
            TempData["SuccessMessage"] = "User deleted successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {Email}", email);
            TempData["ErrorMessage"] = "Error deleting user.";
        }

        return RedirectToPage();
    }
}
