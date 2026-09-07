using System.Collections.Generic;
using System.Threading.Tasks;
using TourManagement.Domain.Entities;

namespace TourManagement.Application.Interfaces
{
    public interface ITourService
    {
        Task<IEnumerable<Tour>> GetAllToursAsync();
        Task<Tour> GetTourByIdAsync(int id);
        Task CreateTourAsync(Tour tour);
        Task UpdateTourAsync(Tour tour);
        Task DeleteTourAsync(int id);
    }

    public interface IBookingService
    {
        Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId);
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task CreateBookingAsync(Booking booking);
        Task UpdateBookingStatusAsync(int bookingId, string status);
    }

    public interface IUserService
    {
        Task<User> AuthenticateAsync(string username, string password);
        Task CreateUserAsync(User user);
        Task<User> GetUserByIdAsync(int id);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}
