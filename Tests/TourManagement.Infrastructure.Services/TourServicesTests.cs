using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Persistence;
using TourManagement.Infrastructure.Services;
using System.Linq;

namespace TourManagement.Infrastructure.Services.Tests
{
    public class TourServiceTests
    {
        private TourDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<TourDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            return new TourDbContext(options);
        }

        [Fact]
        public async Task GetAllToursAsync_ShouldReturnAllTours()
        {
            // Arrange
            using var context = GetDbContext();
            context.Tours.AddRange(new List<Tour>
            {
                new Tour { TourId = 1, TourName = "Tour 1", Price = 100 },
                new Tour { TourId = 2, TourName = "Tour 2", Price = 200 }
            });
            await context.SaveChangesAsync();
            var service = new TourService(context);

            // Act
            var result = await service.GetAllToursAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetTourByIdAsync_ShouldReturnTour_WhenExists()
        {
            // Arrange
            using var context = GetDbContext();
            context.Tours.Add(new Tour { TourId = 1, TourName = "Tour 1", Price = 100 });
            await context.SaveChangesAsync();
            var service = new TourService(context);

            // Act
            var result = await service.GetTourByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Tour 1", result.TourName);
        }

        [Fact]
        public async Task GetTourByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            using var context = GetDbContext();
            var service = new TourService(context);

            // Act
            var result = await service.GetTourByIdAsync(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateTourAsync_ShouldAddTour()
        {
            // Arrange
            using var context = GetDbContext();
            var service = new TourService(context);
            var tour = new Tour { TourName = "New Tour", Price = 300 };

            // Act
            var result = await service.CreateTourAsync(tour);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Tour", result.TourName);
            Assert.Equal(1, await context.Tours.CountAsync());
        }

        [Fact]
        public async Task UpdateTourAsync_ShouldUpdateTour()
        {
            // Arrange
            using var context = GetDbContext();
            var tour = new Tour { TourId = 1, TourName = "Old Name", Price = 100 };
            context.Tours.Add(tour);
            await context.SaveChangesAsync();
            var service = new TourService(context);

            // Act
            tour.TourName = "New Name";
            await service.UpdateTourAsync(tour);

            // Assert
            var updatedTour = await context.Tours.FindAsync(1);
            Assert.Equal("New Name", updatedTour.TourName);
        }

        [Fact]
        public async Task DeleteTourAsync_ShouldRemoveTour_WhenExists()
        {
            // Arrange
            using var context = GetDbContext();
            context.Tours.Add(new Tour { TourId = 1, TourName = "Tour 1", Price = 100 });
            await context.SaveChangesAsync();
            var service = new TourService(context);

            // Act
            await service.DeleteTourAsync(1);

            // Assert
            Assert.Equal(0, await context.Tours.CountAsync());
        }
    }

    public class UserServiceTests
    {
        private TourDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<TourDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            return new TourDbContext(options);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_ShouldReturnUser_WhenExists()
        {
            // Arrange
            using var context = GetDbContext();
            context.Users.Add(new User { UserId = 1, Username = "testuser", Email = "test@test.com", Password = "pw" });
            await context.SaveChangesAsync();
            var service = new UserService(context);

            // Act
            var result = await service.GetUserByUsernameAsync("testuser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldAddUser()
        {
            // Arrange
            using var context = GetDbContext();
            var service = new UserService(context);
            var user = new User { Username = "newuser", Email = "new@test.com", Password = "pw" };

            // Act
            var result = await service.CreateUserAsync(user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("newuser", result.Username);
        }
    }

    public class BookingServiceTests
    {
        private TourDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<TourDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            return new TourDbContext(options);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldAddBooking()
        {
            // Arrange
            using var context = GetDbContext();
            var service = new BookingService(context);
            var booking = new Booking { UserId = 1, TourId = 1, TotalPrice = 500, Status = "Pending" };

            // Act
            var result = await service.CreateBookingAsync(booking);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result.TotalPrice);
        }

        [Fact]
        public async Task UpdateBookingStatusAsync_ShouldUpdateStatus_WhenExists()
        {
            // Arrange
            using var context = GetDbContext();
            context.Bookings.Add(new Booking { BookingId = 1, UserId = 1, TourId = 1, Status = "Pending" });
            await context.SaveChangesAsync();
            var service = new BookingService(context);

            // Act
            await service.UpdateBookingStatusAsync(1, "Confirmed");

            // Assert
            var booking = await context.Bookings.FindAsync(1);
            Assert.Equal("Confirmed", booking.Status);
        }
    }
}
