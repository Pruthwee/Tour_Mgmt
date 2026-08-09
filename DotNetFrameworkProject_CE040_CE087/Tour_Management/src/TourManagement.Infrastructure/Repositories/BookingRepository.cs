using Microsoft.EntityFrameworkCore;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Infrastructure.Data;

namespace TourManagement.Infrastructure.Repositories;

public sealed class BookingRepository(TourManagementDbContext dbContext) : IBookingRepository
{
    public async Task<IReadOnlyList<Booking>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Bookings.AsNoTracking().Include(b => b.Tour).Where(b => b.IsActive).OrderByDescending(b => b.BookingDate).ToListAsync(cancellationToken);

    public async Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await dbContext.Bookings.AsNoTracking().Include(b => b.Tour).FirstOrDefaultAsync(b => b.Id == id && b.IsActive, cancellationToken);

    public async Task<IReadOnlyList<Booking>> GetByCustomerEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await dbContext.Bookings.AsNoTracking().Include(b => b.Tour).Where(b => b.IsActive && b.CustomerEmail == email).ToListAsync(cancellationToken);

    public async Task<Booking> AddAsync(Booking entity, CancellationToken cancellationToken = default)
    {
        dbContext.Bookings.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(Booking entity, CancellationToken cancellationToken = default)
    {
        entity.ModifiedDate = DateTime.UtcNow;
        dbContext.Bookings.Update(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Bookings.FindAsync([id], cancellationToken);
        if (entity is null) return;
        entity.IsActive = false;
        entity.ModifiedDate = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) => await dbContext.Bookings.AnyAsync(b => b.Id == id && b.IsActive, cancellationToken);

    public async Task<IReadOnlyList<Booking>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        searchTerm = searchTerm.Trim();
        return await dbContext.Bookings.AsNoTracking().Include(b => b.Tour).Where(b => b.IsActive && (b.CustomerEmail.Contains(searchTerm) || b.CustomerName.Contains(searchTerm) || b.Status.Contains(searchTerm))).ToListAsync(cancellationToken);
    }
}
