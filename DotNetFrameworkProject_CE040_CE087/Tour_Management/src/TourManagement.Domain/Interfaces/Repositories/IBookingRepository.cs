using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces.Repositories;

/// <summary>Repository contract for bookings.</summary>
public interface IBookingRepository : IRepository<Booking>
{
    Task<IReadOnlyList<Booking>> GetByCustomerEmailAsync(string email, CancellationToken cancellationToken = default);
}
