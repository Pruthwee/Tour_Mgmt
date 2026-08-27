using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tour_Management.Web.Pages;

/// <summary>Page model for the home/index page.</summary>
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    /// <summary>Gets whether the user is logged in.</summary>
    public bool IsLoggedIn { get; private set; }

    /// <summary>Initializes a new instance of <see cref="IndexModel"/>.</summary>
    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    /// <summary>Handles GET requests.</summary>
    public void OnGet()
    {
        _logger.LogInformation("Home page accessed");
        IsLoggedIn = HttpContext.Session.GetString("UserEmail") != null;
    }
}
