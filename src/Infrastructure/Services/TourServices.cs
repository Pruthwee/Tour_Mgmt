using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourManagement.Application.Interfaces;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Persistence;

namespace TourManagement.Infrastructure.Services
{
    public class TourService : ITourService
    {
        private readonly TourDbContext _context;
        public TourService(TourDbContext context) => _context = context;

        public async Task<IEnumerable<Tour>> GetAllToursAsync() => await _context.Tours.ToListAsync();
        public async Task<Tour> GetTourByIdAsync(int id) => await _context.Tours.FindAsync(id);
        public async Task<Tour> CreateTourAsync(Tour tour)
        {
            _context.Tours.Add(tour);
            await _context.SaveChangesAsync();
            return tour;
        }
        public async Task UpdateTourAsync(Tour tour)
        {
            _context.Tours.Update(tour);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteTourAsync(int id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour != null)
            {
                _context.Tours.Remove(tour);
                await _context.SaveChangesAsync();
            }
        }
    }

    public class UserService : IUserService
    {
        private readonly TourDbContext _context;
        public UserService(TourDbContext context) => _context = context;

        public async Task<User> GetUserByUsernameAsync(string username) => 
            await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync() => await _context.Users.ToListAsync();
        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }

    public class BookingService : IBookingService
    {
        private readonly TourDbContext _context;
        public BookingService(TourDbContext context) => _context = context;

        public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId) => 
            await _context.Bookings.Where(b => b.UserId == userId).ToListAsync();
        public async Task<IEnumerable<Booking>> GetAllBookingsAsync() => await _context.Bookings.ToListAsync();
        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }
        public async Task UpdateBookingStatusAsync(int bookingId, string status)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking != null)
            {
                booking.Status = status;
                await _context.SaveChangesAsync();
            }
        }
    }
}
