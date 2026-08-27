using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Interfaces.Repositories;
using Tour_Management.Infrastructure.Data;

namespace Tour_Management.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for Booking entity.
/// </summary>
public class BookingRepository : IBookingRepository
{
    private readonly TourManagementDbContext _context;
    private readonly ILogger<BookingRepository> _logger;

    /// <summary>Initializes a new instance of <see cref="BookingRepository"/>.</summary>
    public BookingRepository(TourManagementDbContext context, ILogger<BookingRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Booking>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Bookings.AsNoTracking()
            .Where(b => b.IsActive)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings.AsNoTracking()
            .FirstOrDefaultAsync(b => b.BookingId == id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Booking>> GetByUserEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings.AsNoTracking()
            .Where(b => b.Email == email && b.IsActive)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Booking> AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync(cancellationToken);
        return booking;
    }

    /// <inheritdoc/>
    public async Task<Booking> UpdateAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync(cancellationToken);
        return booking;
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var booking = await _context.Bookings.FindAsync(new object[] { id }, cancellationToken);
        if (booking != null)
        {
            booking.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings.AnyAsync(b => b.BookingId == id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Booking>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var term = searchTerm.ToLower();
        return await _context.Bookings.AsNoTracking()
            .Where(b => b.IsActive &&
                       (b.TourName.ToLower().Contains(term) || b.Email.ToLower().Contains(term)))
            .ToListAsync(cancellationToken);
    }
}
