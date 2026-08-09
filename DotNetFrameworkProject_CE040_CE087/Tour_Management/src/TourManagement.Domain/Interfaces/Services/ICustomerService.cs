using TourManagement.Application.DTOs;

namespace TourManagement.Domain.Interfaces.Services;

/// <summary>Service contract for customer use cases.</summary>
public interface ICustomerService
{
    Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CustomerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CustomerDto> CreateAsync(CustomerCreateDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, CustomerUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CustomerDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
