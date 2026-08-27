using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Tour_Management.Domain.Entities;
using Tour_Management.Infrastructure.Data;
using Tour_Management.Infrastructure.Repositories;

namespace Tour_Management.Infrastructure.Repositories.Tests
{
    public class BookingRepositoryTests : IDisposable
    {
        private readonly TourManagementDbContext _context;
        private readonly Mock<ILogger<BookingRepository>> _mockLogger;
        private readonly BookingRepository _repository;

        public BookingRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TourManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TourManagementDbContext(options);
            _mockLogger = new Mock<ILogger<BookingRepository>>();
            _repository = new BookingRepository(_context, _mockLogger.Object);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsActiveBookings_Only()
        {
            // Arrange
            _context.Bookings.AddRange(
                new Booking { TourName = "Paris Tour", Place = "Paris", Email = "user1@example.com", FirstName = "John", IsActive = true, BookingDate = new DateTime(2024, 1, 1) },
                new Booking { TourName = "Rome Tour", Place = "Rome", Email = "user2@example.com", FirstName = "Jane", IsActive = false, BookingDate = new DateTime(2024, 1, 2) }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            var list = new List<Booking>(result);
            Assert.Single(list);
            Assert.Equal("Paris Tour", list[0].TourName);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmpty_WhenNoActiveBookings()
        {
            // Arrange
            _context.Bookings.Add(new Booking { TourName = "Inactive Tour", Place = "Place", Email = "user@example.com", FirstName = "User", IsActive = false });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsBooking_WhenExists()
        {
            // Arrange
            var booking = new Booking { TourName = "Paris Tour", Place = "Paris", Email = "user@example.com", FirstName = "John", IsActive = true };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(booking.BookingId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Paris Tour", result.TourName);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Act
            var result = await _repository.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUserEmailAsync_ReturnsActiveBookings_ForUser()
        {
            // Arrange
            var email = "user@example.com";
            _context.Bookings.AddRange(
                new Booking { TourName = "Paris Tour", Place = "Paris", Email = email, FirstName = "John", IsActive = true, BookingDate = new DateTime(2024, 1, 1) },
                new Booking { TourName = "Rome Tour", Place = "Rome", Email = email, FirstName = "John", IsActive = true, BookingDate = new DateTime(2024, 2, 1) },
                new Booking { TourName = "London Tour", Place = "London", Email = email, FirstName = "John", IsActive = false, BookingDate = new DateTime(2024, 3, 1) }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByUserEmailAsync(email);

            // Assert
            Assert.NotNull(result);
            var list = new List<Booking>(result);
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public async Task GetByUserEmailAsync_ReturnsEmpty_WhenNoBookingsForUser()
        {
            // Act
            var result = await _repository.GetByUserEmailAsync("noone@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddAsync_AddsBooking_Successfully()
        {
            // Arrange
            var booking = new Booking { TourName = "New Tour", Place = "London", Email = "user@example.com", FirstName = "John", IsActive = true };

            // Act
            var result = await _repository.AddAsync(booking);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Tour", result.TourName);
            Assert.Equal(1, await _context.Bookings.CountAsync());
        }

        [Fact]
        public async Task UpdateAsync_UpdatesBooking_Successfully()
        {
            // Arrange
            var booking = new Booking { TourName = "Old Tour", Place = "Old Place", Email = "user@example.com", FirstName = "John", IsActive = true };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            booking.TourName = "Updated Tour";

            // Act
            var result = await _repository.UpdateAsync(booking);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Tour", result.TourName);
        }

        [Fact]
        public async Task DeleteAsync_SetsIsActiveToFalse_WhenBookingExists()
        {
            // Arrange
            var booking = new Booking { TourName = "Tour To Delete", Place = "Place", Email = "user@example.com", FirstName = "John", IsActive = true };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            var bookingId = booking.BookingId;

            // Act
            await _repository.DeleteAsync(bookingId);

            // Assert
            var deletedBooking = await _context.Bookings.FindAsync(bookingId);
            Assert.NotNull(deletedBooking);
            Assert.False(deletedBooking.IsActive);
        }

        [Fact]
        public async Task DeleteAsync_DoesNotThrow_WhenBookingNotFound()
        {
            // Act & Assert (should not throw)
            await _repository.DeleteAsync(999);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenBookingExists()
        {
            // Arrange
            var booking = new Booking { TourName = "Paris Tour", Place = "Paris", Email = "user@example.com", FirstName = "John", IsActive = true };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(booking.BookingId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalse_WhenBookingNotFound()
        {
            // Act
            var result = await _repository.ExistsAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task SearchAsync_ReturnsMatchingBookings_ByTourName()
        {
            // Arrange
            _context.Bookings.AddRange(
                new Booking { TourName = "Paris Tour", Place = "Paris", Email = "user1@example.com", FirstName = "John", IsActive = true },
                new Booking { TourName = "Rome Tour", Place = "Rome", Email = "user2@example.com", FirstName = "Jane", IsActive = true }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.SearchAsync("paris");

            // Assert
            Assert.NotNull(result);
            var list = new List<Booking>(result);
            Assert.Single(list);
            Assert.Equal("Paris Tour", list[0].TourName);
        }

        [Fact]
        public async Task SearchAsync_ReturnsMatchingBookings_ByEmail()
        {
            // Arrange
            _context.Bookings.Add(new Booking { TourName = "Tour", Place = "Place", Email = "alice@example.com", FirstName = "Alice", IsActive = true });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.SearchAsync("alice");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task SearchAsync_ExcludesInactiveBookings()
        {
            // Arrange
            _context.Bookings.Add(new Booking { TourName = "Paris Tour", Place = "Paris", Email = "user@example.com", FirstName = "John", IsActive = false });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.SearchAsync("paris");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task SearchAsync_ReturnsEmpty_WhenNoMatch()
        {
            // Arrange
            _context.Bookings.Add(new Booking { TourName = "Paris Tour", Place = "Paris", Email = "user@example.com", FirstName = "John", IsActive = true });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.SearchAsync("xyz");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsBookings_OrderedByBookingDateDescending()
        {
            // Arrange
            _context.Bookings.AddRange(
                new Booking { TourName = "Tour A", Place = "Place", Email = "user@example.com", FirstName = "John", IsActive = true, BookingDate = new DateTime(2024, 1, 1) },
                new Booking { TourName = "Tour B", Place = "Place", Email = "user@example.com", FirstName = "John", IsActive = true, BookingDate = new DateTime(2024, 3, 1) },
                new Booking { TourName = "Tour C", Place = "Place", Email = "user@example.com", FirstName = "John", IsActive = true, BookingDate = new DateTime(2024, 2, 1) }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            var list = new List<Booking>(result);
            Assert.Equal(3, list.Count);
            Assert.Equal("Tour B", list[0].TourName); // Most recent first
            Assert.Equal("Tour C", list[1].TourName);
            Assert.Equal("Tour A", list[2].TourName);
        }
    }
}
