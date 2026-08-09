using TourManagement.Application.DTOs;

namespace TourManagement.Domain.Interfaces.Services;

/// <summary>Service contract for booking use cases.</summary>
public interface IBookingService
{
    Task<IReadOnlyList<BookingDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BookingDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<BookingDto> CreateAsync(BookingCreateDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, BookingUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookingDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookingDto>> GetByCustomerEmailAsync(string email, CancellationToken cancellationToken = default);
}
