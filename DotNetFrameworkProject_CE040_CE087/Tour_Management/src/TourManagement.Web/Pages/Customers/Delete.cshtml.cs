using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Interfaces.Services;
namespace TourManagement.Web.Pages.Customers;
public sealed class DeleteModel(ICustomerService service) : PageModel { public CustomerDto? Customer { get; private set; } public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken) { Customer = await service.GetByIdAsync(id, cancellationToken); return Customer is null ? NotFound() : Page(); } public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken) { await service.DeleteAsync(id, cancellationToken); return RedirectToPage("Index"); } }
