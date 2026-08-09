using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Interfaces.Services;
namespace TourManagement.Web.Pages.Customers;
public sealed class CreateModel(ICustomerService service) : PageModel { [BindProperty] public CustomerCreateDto Input { get; set; } = new(); public void OnGet() { } public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken) { if (!ModelState.IsValid) return Page(); await service.CreateAsync(Input, cancellationToken); return RedirectToPage("Index"); } }
