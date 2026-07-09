using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace TourManagement.Application.Services
{
    public interface ITourService
    {
        Task<IEnumerable<Tour>> GetAllToursAsync();
        Task<Tour?> GetTourByIdAsync(int id);
        Task CreateTourAsync(Tour tour);
        Task UpdateTourAsync(Tour tour);
        Task DeleteTourAsync(int id);
    }

    public class TourService : ITourService
    {
        private readonly TourDbContext _context;

        public TourService(TourDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tour>> GetAllToursAsync() => await _context.Tours.ToListAsync();

        public async Task<Tour?> GetTourByIdAsync(int id) => await _context.Tours.FindAsync(id);

        public async Task CreateTourAsync(Tour tour)
        {
            _context.Tours.Add(tour);
            await _context.SaveChangesAsync();
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
}
