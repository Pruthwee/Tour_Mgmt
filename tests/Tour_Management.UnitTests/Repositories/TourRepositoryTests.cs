using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Tour_Management.Domain.Entities;
using Tour_Management.Infrastructure.Data;
using Tour_Management.Infrastructure.Repositories;

namespace Tour_Management.UnitTests.Repositories;

public class TourRepositoryTests : IDisposable
{
    private readonly TourManagementDbContext _context;
    private readonly Mock<ILogger<TourRepository>> _mockLogger;
    private readonly TourRepository _repository;

    public TourRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new TourManagementDbContext(options);
        _mockLogger = new Mock<ILogger<TourRepository>>();
        _repository = new TourRepository(_context, _mockLogger.Object);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task GetAllAsync_ReturnsActiveTours_Only()
    {
        _context.Tours.AddRange(
            new Tour { TourName = "Paris Tour", Place = "Paris", Days = 7, Price = 1500m, Locations = "Eiffel", TourInfo = "Great", IsActive = true },
            new Tour { TourName = "Rome Tour", Place = "Rome", Days = 5, Price = 1200m, Locations = "Colosseum", TourInfo = "Nice", IsActive = false }
        );
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        Assert.NotNull(result);
        var list = new List<Tour>(result);
        Assert.Single(list);
        Assert.Equal("Paris Tour", list[0].TourName);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoActiveTours()
    {
        _context.Tours.Add(new Tour { TourName = "Inactive Tour", Place = "Place", Days = 3, Price = 500m, Locations = "Loc", TourInfo = "Info", IsActive = false });
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsTour_WhenExists()
    {
        var tour = new Tour { TourName = "Paris Tour", Place = "Paris", Days = 7, Price = 1500m, Locations = "Eiffel", TourInfo = "Great", IsActive = true };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(tour.TourId);

        Assert.NotNull(result);
        Assert.Equal("Paris Tour", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var result = await _repository.GetByIdAsync(999);
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_AddsTour_Successfully()
    {
        var tour = new Tour { TourName = "New Tour", Place = "London", Days = 5, Price = 1000m, Locations = "Big Ben", TourInfo = "Great London tour", IsActive = true };

        var result = await _repository.AddAsync(tour);

        Assert.NotNull(result);
        Assert.Equal("New Tour", result.TourName);
        Assert.Equal(1, await _context.Tours.CountAsync());
    }

    [Fact]
    public async Task UpdateAsync_UpdatesTour_Successfully()
    {
        var tour = new Tour { TourName = "Old Tour", Place = "Old Place", Days = 3, Price = 500m, Locations = "Old Loc", TourInfo = "Old Info", IsActive = true };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        tour.TourName = "Updated Tour";
        var result = await _repository.UpdateAsync(tour);

        Assert.NotNull(result);
        Assert.Equal("Updated Tour", result.TourName);
    }

    [Fact]
    public async Task DeleteAsync_SetsIsActiveToFalse_WhenTourExists()
    {
        var tour = new Tour { TourName = "Tour To Delete", Place = "Place", Days = 3, Price = 500m, Locations = "Loc", TourInfo = "Info", IsActive = true };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();
        var tourId = tour.TourId;

        await _repository.DeleteAsync(tourId);

        var deletedTour = await _context.Tours.FindAsync(tourId);
        Assert.NotNull(deletedTour);
        Assert.False(deletedTour.IsActive);
        Assert.NotNull(deletedTour.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_DoesNotThrow_WhenTourNotFound()
    {
        await _repository.DeleteAsync(999);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenTourExists()
    {
        var tour = new Tour { TourName = "Paris Tour", Place = "Paris", Days = 7, Price = 1500m, Locations = "Eiffel", TourInfo = "Great", IsActive = true };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        var result = await _repository.ExistsAsync(tour.TourId);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenTourNotFound()
    {
        var result = await _repository.ExistsAsync(999);
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingTours_ByTourName()
    {
        _context.Tours.AddRange(
            new Tour { TourName = "Paris Tour", Place = "Paris", Days = 7, Price = 1500m, Locations = "Eiffel", TourInfo = "Great", IsActive = true },
            new Tour { TourName = "Rome Tour", Place = "Rome", Days = 5, Price = 1200m, Locations = "Colosseum", TourInfo = "Nice", IsActive = true }
        );
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("paris");

        Assert.NotNull(result);
        var list = new List<Tour>(result);
        Assert.Single(list);
        Assert.Equal("Paris Tour", list[0].TourName);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingTours_ByPlace()
    {
        _context.Tours.Add(new Tour { TourName = "London Tour", Place = "London", Days = 5, Price = 1000m, Locations = "Big Ben", TourInfo = "Great", IsActive = true });
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("london");

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task SearchAsync_ExcludesInactiveTours()
    {
        _context.Tours.Add(new Tour { TourName = "Inactive Paris Tour", Place = "Paris", Days = 7, Price = 1500m, Locations = "Eiffel", TourInfo = "Great", IsActive = false });
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("paris");

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenNoMatch()
    {
        _context.Tours.Add(new Tour { TourName = "Paris Tour", Place = "Paris", Days = 7, Price = 1500m, Locations = "Eiffel", TourInfo = "Great", IsActive = true });
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("xyz");

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsTours_OrderedByTourName()
    {
        _context.Tours.AddRange(
            new Tour { TourName = "Rome Tour", Place = "Rome", Days = 5, Price = 1200m, Locations = "Colosseum", TourInfo = "Nice", IsActive = true },
            new Tour { TourName = "Athens Tour", Place = "Athens", Days = 4, Price = 900m, Locations = "Acropolis", TourInfo = "Ancient", IsActive = true },
            new Tour { TourName = "Paris Tour", Place = "Paris", Days = 7, Price = 1500m, Locations = "Eiffel", TourInfo = "Great", IsActive = true }
        );
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        var list = new List<Tour>(result);
        Assert.Equal(3, list.Count);
        Assert.Equal("Athens Tour", list[0].TourName);
        Assert.Equal("Paris Tour", list[1].TourName);
        Assert.Equal("Rome Tour", list[2].TourName);
    }
}
