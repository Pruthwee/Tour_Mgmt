using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tour_Management.Domain.DTOs;
using Tour_Management.Domain.Interfaces.Services;
using Tour_Management.Web.ViewModels;

namespace Tour_Management.Web.Pages.Users;

/// <summary>Page model for editing a user.</summary>
public class EditModel : PageModel
{
    private readonly IUserInfoService _userInfoService;
    private readonly ILogger<EditModel> _logger;

    public EditModel(IUserInfoService userInfoService, ILogger<EditModel> logger)
    {
        _userInfoService = userInfoService;
        _logger = logger;
    }

    [BindProperty]
    public new UserEditViewModel User { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true" &&
            HttpContext.Session.GetInt32("UserId") != id)
            return RedirectToPage("./Login");

        try
        {
            var dto = await _userInfoService.GetByIdAsync(id, cancellationToken);
            if (dto is null)
                return NotFound();

            // Manual mapping from DTO to ViewModel
            User = new UserEditViewModel
            {
                UserInfoId = dto.UserInfoId,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Gender = dto.Gender,
                Dob = dto.Dob,
                Street = dto.Street,
                City = dto.City,
                State = dto.State,
                IsActive = dto.IsActive
            };
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user for edit, ID {UserId}", id);
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true" &&
            HttpContext.Session.GetInt32("UserId") != User.UserInfoId)
            return RedirectToPage("./Login");

        if (!ModelState.IsValid)
            return Page();

        try
        {
            // Manual mapping from ViewModel to DTO
            var updateDto = new UserInfoUpdateDto
            {
                Email = User.Email,
                FirstName = User.FirstName,
                LastName = User.LastName,
                Gender = User.Gender,
                Dob = User.Dob,
                Street = User.Street,
                City = User.City,
                State = User.State,
                IsActive = User.IsActive
            };

            await _userInfoService.UpdateAsync(User.UserInfoId, updateDto, cancellationToken);
            TempData["SuccessMessage"] = "User updated successfully!";

            if (HttpContext.Session.GetString("IsAdmin") == "true")
                return RedirectToPage("./Index");
            return RedirectToPage("./Profile");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID {UserId}", User.UserInfoId);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the user.");
            return Page();
        }
    }
}
