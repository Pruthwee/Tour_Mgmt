using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Interfaces.Services;
namespace TourManagement.Web.Pages.Customers;
public sealed class IndexModel(ICustomerService service) : PageModel { public IReadOnlyList<CustomerDto> Customers { get; private set; } = []; public async Task OnGetAsync(CancellationToken cancellationToken) => Customers = await service.GetAllAsync(cancellationToken); }
