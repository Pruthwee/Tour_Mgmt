using TourManagement.Application.DTOs;

namespace TourManagement.Domain.Interfaces.Services;

/// <summary>Service contract for tour package use cases.</summary>
public interface ITourService
{
    Task<IReadOnlyList<TourDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TourDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TourDto> CreateAsync(TourCreateDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, TourUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TourDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
