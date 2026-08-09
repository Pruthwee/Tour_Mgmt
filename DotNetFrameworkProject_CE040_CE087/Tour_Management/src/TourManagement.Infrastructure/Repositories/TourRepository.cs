using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Infrastructure.Data;

namespace TourManagement.Infrastructure.Repositories;

public sealed class TourRepository(TourManagementDbContext dbContext, ILogger<TourRepository> logger) : ITourRepository
{
    public async Task<IReadOnlyList<Tour>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Tours.AsNoTracking().Where(t => t.IsActive).OrderBy(t => t.Name).ToListAsync(cancellationToken);

    public async Task<Tour?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await dbContext.Tours.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id && t.IsActive, cancellationToken);

    public async Task<Tour> AddAsync(Tour entity, CancellationToken cancellationToken = default)
    {
        dbContext.Tours.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Tour {TourId} created", entity.Id);
        return entity;
    }

    public async Task UpdateAsync(Tour entity, CancellationToken cancellationToken = default)
    {
        entity.ModifiedDate = DateTime.UtcNow;
        dbContext.Tours.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Tours.FindAsync([id], cancellationToken);
        if (entity is null) return;
        entity.IsActive = false;
        entity.ModifiedDate = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) =>
        await dbContext.Tours.AnyAsync(t => t.Id == id && t.IsActive, cancellationToken);

    public async Task<IReadOnlyList<Tour>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        searchTerm = searchTerm.Trim();
        return await dbContext.Tours.AsNoTracking()
            .Where(t => t.IsActive && (t.Name.Contains(searchTerm) || t.Place.Contains(searchTerm) || t.Locations.Contains(searchTerm)))
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }
}
