using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.Interfaces.Services;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Pages.Users;

/// <summary>Page model for the all users list (admin).</summary>
public class IndexModel : PageModel
{
    private readonly IUserInfoService _userInfoService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IUserInfoService userInfoService, ILogger<IndexModel> logger)
    {
        _userInfoService = userInfoService;
        _logger = logger;
    }

    public IEnumerable<UserProfileViewModel> Users { get; set; } = Enumerable.Empty<UserProfileViewModel>();

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToPage("./Login");

        try
        {
            var dtos = await _userInfoService.GetAllAsync(cancellationToken);

            // Manual mapping from DTO to ViewModel
            Users = dtos.Select(dto => new UserProfileViewModel
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
            });
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading all users");
            Users = Enumerable.Empty<UserProfileViewModel>();
            return Page();
        }
    }
}
