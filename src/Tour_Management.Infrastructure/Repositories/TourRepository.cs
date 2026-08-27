using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Interfaces.Repositories;
using Tour_Management.Infrastructure.Data;

namespace Tour_Management.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for Tour entity.
/// </summary>
public class TourRepository : ITourRepository
{
    private readonly TourManagementDbContext _context;
    private readonly ILogger<TourRepository> _logger;

    /// <summary>Initializes a new instance of <see cref="TourRepository"/>.</summary>
    public TourRepository(TourManagementDbContext context, ILogger<TourRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Tour>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tours.AsNoTracking()
            .Where(t => t.IsActive)
            .OrderBy(t => t.TourName)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Tour?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Tours.AsNoTracking()
            .FirstOrDefaultAsync(t => t.TourId == id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Tour> AddAsync(Tour tour, CancellationToken cancellationToken = default)
    {
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync(cancellationToken);
        return tour;
    }

    /// <inheritdoc/>
    public async Task<Tour> UpdateAsync(Tour tour, CancellationToken cancellationToken = default)
    {
        _context.Tours.Update(tour);
        await _context.SaveChangesAsync(cancellationToken);
        return tour;
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var tour = await _context.Tours.FindAsync(new object[] { id }, cancellationToken);
        if (tour != null)
        {
            tour.IsActive = false;
            tour.ModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Tours.AnyAsync(t => t.TourId == id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Tour>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var term = searchTerm.ToLower();
        return await _context.Tours.AsNoTracking()
            .Where(t => t.IsActive &&
                       (t.TourName.ToLower().Contains(term) || t.Place.ToLower().Contains(term)))
            .ToListAsync(cancellationToken);
    }
}
