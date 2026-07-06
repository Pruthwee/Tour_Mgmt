using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Interfaces.Services;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Pages.Users;

/// <summary>Page model for deleting a user.</summary>
public class DeleteModel : PageModel
{
    private readonly IUserInfoService _userInfoService;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(IUserInfoService userInfoService, ILogger<DeleteModel> logger)
    {
        _userInfoService = userInfoService;
        _logger = logger;
    }

    [BindProperty]
    public new UserProfileViewModel User { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToPage("./Login");

        try
        {
            var dto = await _userInfoService.GetByIdAsync(id, cancellationToken);
            if (dto is null)
                return NotFound();

            // Manual mapping from DTO to ViewModel
            User = new UserProfileViewModel
            {
                UserInfoId = dto.UserInfoId,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                City = dto.City
            };
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user for delete, ID {UserId}", id);
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToPage("./Login");

        try
        {
            await _userInfoService.DeleteAsync(User.UserInfoId, cancellationToken);
            TempData["SuccessMessage"] = "User deleted successfully!";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID {UserId}", User.UserInfoId);
            TempData["ErrorMessage"] = "An error occurred while deleting the user.";
            return RedirectToPage("./Index");
        }
    }
}
