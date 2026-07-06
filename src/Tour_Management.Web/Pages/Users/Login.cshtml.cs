using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Interfaces.Services;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Pages.Users;

/// <summary>Page model for user login.</summary>
public class LoginModel : PageModel
{
    private readonly IUserInfoService _userInfoService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(IUserInfoService userInfoService, IConfiguration configuration, ILogger<LoginModel> logger)
    {
        _userInfoService = userInfoService;
        _configuration = configuration;
        _logger = logger;
    }

    [BindProperty]
    public LoginViewModel Login { get; set; } = new();

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetString("UserEmail") != null)
            return RedirectToPage("/Index");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            var user = await _userInfoService.ValidateLoginAsync(Login.Email, Login.Password, cancellationToken);
            if (user is not null)
            {
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserFirstName", user.FirstName);
                HttpContext.Session.SetInt32("UserId", user.UserInfoId);
                _logger.LogInformation("User {Email} logged in successfully", Login.Email);
                return RedirectToPage("/Tours/Index");
            }

            // Check admin credentials
            var adminEmail = _configuration["AdminCredentials:Email"];
            var adminPassword = _configuration["AdminCredentials:Password"];
            if (Login.Email == adminEmail && Login.Password == adminPassword)
            {
                HttpContext.Session.SetString("UserEmail", Login.Email);
                HttpContext.Session.SetString("UserFirstName", "Admin");
                HttpContext.Session.SetString("IsAdmin", "true");
                _logger.LogInformation("Admin logged in successfully");
                return RedirectToPage("/Admin/Dashboard");
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", Login.Email);
            ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
            return Page();
        }
    }
}
