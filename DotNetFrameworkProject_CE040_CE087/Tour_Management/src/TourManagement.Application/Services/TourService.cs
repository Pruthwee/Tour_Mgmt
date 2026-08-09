using Microsoft.Extensions.Logging;
using TourManagement.Application.DTOs;
using TourManagement.Application.Validators;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

public sealed class TourService(ITourRepository repository, ILogger<TourService> logger) : ITourService
{
    public async Task<IReadOnlyList<TourDto>> GetAllAsync(CancellationToken cancellationToken = default) => (await repository.GetAllAsync(cancellationToken)).Select(Map).ToList();
    public async Task<TourDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => (await repository.GetByIdAsync(id, cancellationToken)) is { } tour ? Map(tour) : null;
    public async Task<IReadOnlyList<TourDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default) => (await repository.SearchAsync(searchTerm ?? string.Empty, cancellationToken)).Select(Map).ToList();
    public async Task<TourDto> CreateAsync(TourCreateDto dto, CancellationToken cancellationToken = default)
    {
        ValidationHelper.Validate(dto);
        var entity = new Tour { Name = dto.Name.Trim(), Place = dto.Place.Trim(), Days = dto.Days, Price = dto.Price, Locations = dto.Locations.Trim(), Description = dto.Description.Trim(), PictureFileName = dto.PictureFileName };
        entity = await repository.AddAsync(entity, cancellationToken);
        logger.LogInformation("Created tour {TourId}", entity.Id);
        return Map(entity);
    }
    public async Task UpdateAsync(int id, TourUpdateDto dto, CancellationToken cancellationToken = default)
    {
        ValidationHelper.Validate(dto);
        var entity = await repository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException($"Tour {id} was not found.");
        entity.Name = dto.Name.Trim(); entity.Place = dto.Place.Trim(); entity.Days = dto.Days; entity.Price = dto.Price; entity.Locations = dto.Locations.Trim(); entity.Description = dto.Description.Trim(); entity.PictureFileName = dto.PictureFileName;
        await repository.UpdateAsync(entity, cancellationToken);
    }
    public Task DeleteAsync(int id, CancellationToken cancellationToken = default) => repository.DeleteAsync(id, cancellationToken);
    private static TourDto Map(Tour tour) => new(tour.Id, tour.Name, tour.Place, tour.Days, tour.Price, tour.Locations, tour.Description, tour.PictureFileName);
}
