using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Tour_Management.Application.DTOs;
using Tour_Management.Application.Mappings;
using Tour_Management.Application.Services;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Exceptions;
using Tour_Management.Domain.Interfaces.Repositories;

namespace Tour_Management.Application.Services.Tests
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _mockBookingRepository;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<BookingService>> _mockLogger;
        private readonly BookingService _bookingService;

        public BookingServiceTests()
        {
            _mockBookingRepository = new Mock<IBookingRepository>();
            _mockLogger = new Mock<ILogger<BookingService>>();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();

            _bookingService = new BookingService(_mockBookingRepository.Object, _mapper, _mockLogger.Object);
        }

        // GetAllAsync Tests
        [Fact]
        public async Task GetAllAsync_ReturnsAllBookings()
        {
            // Arrange
            var bookings = new List<Booking>
            {
                new Booking { BookingId = 1, TourName = "Paris Tour", Email = "user1@example.com", IsActive = true },
                new Booking { BookingId = 2, TourName = "Rome Tour", Email = "user2@example.com", IsActive = true }
            };
            _mockBookingRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(bookings);

            // Act
            var result = await _bookingService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            var list = new List<BookingDto>(result);
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoBookings()
        {
            // Arrange
            _mockBookingRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Booking>());

            // Act
            var result = await _bookingService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_ThrowsException_WhenRepositoryFails()
        {
            // Arrange
            _mockBookingRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _bookingService.GetAllAsync());
        }

        // GetByIdAsync Tests
        [Fact]
        public async Task GetByIdAsync_ReturnsBooking_WhenBookingExists()
        {
            // Arrange
            var booking = new Booking { BookingId = 1, TourName = "Paris Tour", Email = "user@example.com", IsActive = true };
            _mockBookingRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            // Act
            var result = await _bookingService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.BookingId);
            Assert.Equal("Paris Tour", result.TourName);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenBookingNotFound()
        {
            // Arrange
            _mockBookingRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Booking?)null);

            // Act
            var result = await _bookingService.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        // GetByUserEmailAsync Tests
        [Fact]
        public async Task GetByUserEmailAsync_ReturnsBookings_ForUser()
        {
            // Arrange
            var email = "user@example.com";
            var bookings = new List<Booking>
            {
                new Booking { BookingId = 1, TourName = "Paris Tour", Email = email, IsActive = true },
                new Booking { BookingId = 2, TourName = "Rome Tour", Email = email, IsActive = true }
            };
            _mockBookingRepository.Setup(r => r.GetByUserEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(bookings);

            // Act
            var result = await _bookingService.GetByUserEmailAsync(email);

            // Assert
            Assert.NotNull(result);
            var list = new List<BookingDto>(result);
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public async Task GetByUserEmailAsync_ReturnsEmpty_WhenNoBookingsForUser()
        {
            // Arrange
            _mockBookingRepository.Setup(r => r.GetByUserEmailAsync("noone@example.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Booking>());

            // Act
            var result = await _bookingService.GetByUserEmailAsync("noone@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        // CreateAsync Tests
        [Fact]
        public async Task CreateAsync_CreatesBooking_Successfully()
        {
            // Arrange
            var dto = new BookingCreateDto
            {
                TourName = "Paris Tour",
                Place = "Paris",
                Email = "user@example.com",
                FirstName = "John",
                TourId = 1
            };
            var createdBooking = new Booking
            {
                BookingId = 1,
                TourName = dto.TourName,
                Place = dto.Place,
                Email = dto.Email,
                FirstName = dto.FirstName,
                TourId = dto.TourId,
                IsActive = true
            };

            _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdBooking);

            // Act
            var result = await _bookingService.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Paris Tour", result.TourName);
            Assert.Equal(1, result.BookingId);
        }

        [Fact]
        public async Task CreateAsync_ThrowsException_WhenRepositoryFails()
        {
            // Arrange
            var dto = new BookingCreateDto { TourName = "Test", Place = "Test", Email = "test@test.com", FirstName = "Test" };
            _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _bookingService.CreateAsync(dto));
        }

        // UpdateAsync Tests
        [Fact]
        public async Task UpdateAsync_UpdatesBooking_WhenBookingExists()
        {
            // Arrange
            var existingBooking = new Booking
            {
                BookingId = 1,
                TourName = "Old Tour",
                Place = "Old Place",
                Email = "user@example.com",
                FirstName = "John",
                IsActive = true
            };
            var updateDto = new BookingUpdateDto
            {
                TourName = "Updated Tour",
                Place = "New Place",
                Email = "user@example.com",
                FirstName = "John",
                IsActive = true
            };

            _mockBookingRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingBooking);
            _mockBookingRepository.Setup(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingBooking);

            // Act
            var result = await _bookingService.UpdateAsync(1, updateDto);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsNotFoundException_WhenBookingNotFound()
        {
            // Arrange
            _mockBookingRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Booking?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _bookingService.UpdateAsync(999, new BookingUpdateDto()));
        }

        // DeleteAsync Tests
        [Fact]
        public async Task DeleteAsync_DeletesBooking_WhenBookingExists()
        {
            // Arrange
            _mockBookingRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mockBookingRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _bookingService.DeleteAsync(1);

            // Assert
            _mockBookingRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsNotFoundException_WhenBookingNotFound()
        {
            // Arrange
            _mockBookingRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _bookingService.DeleteAsync(999));
        }

        // SearchAsync Tests
        [Fact]
        public async Task SearchAsync_ReturnsMatchingBookings()
        {
            // Arrange
            var bookings = new List<Booking>
            {
                new Booking { BookingId = 1, TourName = "Paris Tour", Email = "user@example.com", IsActive = true }
            };
            _mockBookingRepository.Setup(r => r.SearchAsync("Paris", It.IsAny<CancellationToken>()))
                .ReturnsAsync(bookings);

            // Act
            var result = await _bookingService.SearchAsync("Paris");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task SearchAsync_ReturnsEmpty_WhenNoMatch()
        {
            // Arrange
            _mockBookingRepository.Setup(r => r.SearchAsync("xyz", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Booking>());

            // Act
            var result = await _bookingService.SearchAsync("xyz");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
