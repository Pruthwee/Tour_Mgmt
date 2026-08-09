using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Interfaces.Services;
namespace TourManagement.Web.Pages.Customers;
public sealed class EditModel(ICustomerService service) : PageModel
{ [BindProperty] public CustomerUpdateDto Input { get; set; } = new(); public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken) { var c = await service.GetByIdAsync(id, cancellationToken); if (c is null) return NotFound(); Input = new CustomerUpdateDto { Email = c.Email, FirstName = c.FirstName, LastName = c.LastName, City = c.City, State = c.State }; return Page(); } public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken) { if (!ModelState.IsValid) return Page(); await service.UpdateAsync(id, Input, cancellationToken); return RedirectToPage("Index"); } }
