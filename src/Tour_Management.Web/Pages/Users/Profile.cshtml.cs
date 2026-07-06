using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Interfaces.Services;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Pages.Users;

/// <summary>Page model for user profile page.</summary>
public class ProfileModel : PageModel
{
    private readonly IUserInfoService _userInfoService;
    private readonly ILogger<ProfileModel> _logger;

    public ProfileModel(IUserInfoService userInfoService, ILogger<ProfileModel> logger)
    {
        _userInfoService = userInfoService;
        _logger = logger;
    }

    public UserProfileViewModel? UserProfile { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userEmail = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(userEmail))
            return RedirectToPage("./Login");

        try
        {
            var dto = await _userInfoService.GetByEmailAsync(userEmail, cancellationToken);
            if (dto is null)
                return RedirectToPage("./Login");

            // Manual mapping from DTO to ViewModel
            UserProfile = new UserProfileViewModel
            {
                UserInfoId = dto.UserInfoId,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Gender = dto.Gender,
                Dob = dto.Dob,
                Street = dto.Street,
                City = dto.City,
                State = dto.State
            };
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading profile for {Email}", userEmail);
            return RedirectToPage("/Index");
        }
    }
}
