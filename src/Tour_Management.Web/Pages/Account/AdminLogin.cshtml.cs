using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Pages.Account;

/// <summary>Page model for the admin login page.</summary>
public class AdminLoginModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AdminLoginModel> _logger;

    /// <summary>Gets or sets the admin login input model.</summary>
    [BindProperty]
    public AdminLoginViewModel Input { get; set; } = new();

    /// <summary>Gets or sets the error message to display.</summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>Initializes a new instance of <see cref="AdminLoginModel"/>.</summary>
    public AdminLoginModel(IConfiguration configuration, ILogger<AdminLoginModel> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>Handles GET requests.</summary>
    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetString("IsAdmin") == "true")
            return RedirectToPage("/Admin/Tours/Index");
        return Page();
    }

    /// <summary>Handles POST requests for admin login.</summary>
    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
            return Page();

        var adminEmail = _configuration["AppSettings:AdminEmail"] ?? "admin@gmail.com";
        var adminPassword = _configuration["AppSettings:AdminPassword"] ?? "admin";

        if (Input.Email == adminEmail && Input.Password == adminPassword)
        {
            HttpContext.Session.SetString("UserEmail", Input.Email);
            HttpContext.Session.SetString("UserFirstName", "Admin");
            HttpContext.Session.SetString("IsAdmin", "true");
            _logger.LogInformation("Admin logged in successfully");
            return RedirectToPage("/Admin/Tours/Index");
        }

        ErrorMessage = "Invalid admin credentials.";
        return Page();
    }
}
