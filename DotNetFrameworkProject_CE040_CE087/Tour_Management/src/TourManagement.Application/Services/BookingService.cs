using Microsoft.Extensions.Logging;
using TourManagement.Application.DTOs;
using TourManagement.Application.Validators;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

public sealed class BookingService(IBookingRepository repository, ITourRepository tourRepository, ILogger<BookingService> logger) : IBookingService
{
    public async Task<IReadOnlyList<BookingDto>> GetAllAsync(CancellationToken cancellationToken = default) => (await repository.GetAllAsync(cancellationToken)).Select(Map).ToList();
    public async Task<BookingDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => (await repository.GetByIdAsync(id, cancellationToken)) is { } booking ? Map(booking) : null;
    public async Task<IReadOnlyList<BookingDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default) => (await repository.SearchAsync(searchTerm ?? string.Empty, cancellationToken)).Select(Map).ToList();
    public async Task<IReadOnlyList<BookingDto>> GetByCustomerEmailAsync(string email, CancellationToken cancellationToken = default) => (await repository.GetByCustomerEmailAsync(email, cancellationToken)).Select(Map).ToList();
    public async Task<BookingDto> CreateAsync(BookingCreateDto dto, CancellationToken cancellationToken = default)
    {
        ValidationHelper.Validate(dto);
        if (!await tourRepository.ExistsAsync(dto.TourId, cancellationToken)) throw new InvalidOperationException("The selected tour does not exist.");
        var entity = new Booking { TourId = dto.TourId, CustomerEmail = dto.CustomerEmail.Trim(), CustomerName = dto.CustomerName.Trim(), Status = "Pending" };
        entity = await repository.AddAsync(entity, cancellationToken);
        logger.LogInformation("Created booking {BookingId}", entity.Id);
        return Map(entity);
    }
    public async Task UpdateAsync(int id, BookingUpdateDto dto, CancellationToken cancellationToken = default)
    {
        ValidationHelper.Validate(dto);
        var entity = await repository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException($"Booking {id} was not found.");
        entity.TourId = dto.TourId; entity.CustomerEmail = dto.CustomerEmail.Trim(); entity.CustomerName = dto.CustomerName.Trim(); entity.Status = dto.Status.Trim();
        await repository.UpdateAsync(entity, cancellationToken);
    }
    public Task DeleteAsync(int id, CancellationToken cancellationToken = default) => repository.DeleteAsync(id, cancellationToken);
    private static BookingDto Map(Booking booking) => new(booking.Id, booking.TourId, booking.Tour?.Name ?? "Unknown tour", booking.CustomerEmail, booking.CustomerName, booking.BookingDate, booking.Status);
}
