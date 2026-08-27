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

namespace Tour_Management.UnitTests.Services;

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

    [Fact]
    public async Task GetAllAsync_ReturnsAllBookings()
    {
        var bookings = new List<Booking>
        {
            new Booking { BookingId = 1, TourName = "Paris Tour", Email = "user1@example.com", IsActive = true },
            new Booking { BookingId = 2, TourName = "Rome Tour", Email = "user2@example.com", IsActive = true }
        };
        _mockBookingRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(bookings);

        var result = await _bookingService.GetAllAsync();

        Assert.NotNull(result);
        var list = new List<BookingDto>(result);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoBookings()
    {
        _mockBookingRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Booking>());

        var result = await _bookingService.GetAllAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ThrowsException_WhenRepositoryFails()
    {
        _mockBookingRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        await Assert.ThrowsAsync<Exception>(() => _bookingService.GetAllAsync());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsBooking_WhenBookingExists()
    {
        var booking = new Booking { BookingId = 1, TourName = "Paris Tour", Email = "user@example.com", IsActive = true };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(booking);

        var result = await _bookingService.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.BookingId);
        Assert.Equal("Paris Tour", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenBookingNotFound()
    {
        _mockBookingRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Booking?)null);

        var result = await _bookingService.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserEmailAsync_ReturnsBookings_ForUser()
    {
        var email = "user@example.com";
        var bookings = new List<Booking>
        {
            new Booking { BookingId = 1, TourName = "Paris Tour", Email = email, IsActive = true },
            new Booking { BookingId = 2, TourName = "Rome Tour", Email = email, IsActive = true }
        };
        _mockBookingRepository.Setup(r => r.GetByUserEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync(bookings);

        var result = await _bookingService.GetByUserEmailAsync(email);

        Assert.NotNull(result);
        var list = new List<BookingDto>(result);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public async Task GetByUserEmailAsync_ReturnsEmpty_WhenNoBookingsForUser()
    {
        _mockBookingRepository.Setup(r => r.GetByUserEmailAsync("noone@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(new List<Booking>());

        var result = await _bookingService.GetByUserEmailAsync("noone@example.com");

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateAsync_CreatesBooking_Successfully()
    {
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
            BookingId = 1, TourName = dto.TourName, Place = dto.Place,
            Email = dto.Email, FirstName = dto.FirstName, TourId = dto.TourId, IsActive = true
        };
        _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).ReturnsAsync(createdBooking);

        var result = await _bookingService.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("Paris Tour", result.TourName);
        Assert.Equal(1, result.BookingId);
    }

    [Fact]
    public async Task CreateAsync_ThrowsException_WhenRepositoryFails()
    {
        var dto = new BookingCreateDto { TourName = "Test", Place = "Test", Email = "test@test.com", FirstName = "Test" };
        _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        await Assert.ThrowsAsync<Exception>(() => _bookingService.CreateAsync(dto));
    }

    [Fact]
    public async Task UpdateAsync_UpdatesBooking_WhenBookingExists()
    {
        var existingBooking = new Booking
        {
            BookingId = 1, TourName = "Old Tour", Place = "Old Place",
            Email = "user@example.com", FirstName = "John", IsActive = true
        };
        var updateDto = new BookingUpdateDto
        {
            TourName = "Updated Tour", Place = "New Place",
            Email = "user@example.com", FirstName = "John", IsActive = true
        };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingBooking);
        _mockBookingRepository.Setup(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).ReturnsAsync(existingBooking);

        var result = await _bookingService.UpdateAsync(1, updateDto);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsNotFoundException_WhenBookingNotFound()
    {
        _mockBookingRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Booking?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _bookingService.UpdateAsync(999, new BookingUpdateDto()));
    }

    [Fact]
    public async Task DeleteAsync_DeletesBooking_WhenBookingExists()
    {
        _mockBookingRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockBookingRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await _bookingService.DeleteAsync(1);

        _mockBookingRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsNotFoundException_WhenBookingNotFound()
    {
        _mockBookingRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        await Assert.ThrowsAsync<NotFoundException>(() => _bookingService.DeleteAsync(999));
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingBookings()
    {
        var bookings = new List<Booking>
        {
            new Booking { BookingId = 1, TourName = "Paris Tour", Email = "user@example.com", IsActive = true }
        };
        _mockBookingRepository.Setup(r => r.SearchAsync("Paris", It.IsAny<CancellationToken>())).ReturnsAsync(bookings);

        var result = await _bookingService.SearchAsync("Paris");

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenNoMatch()
    {
        _mockBookingRepository.Setup(r => r.SearchAsync("xyz", It.IsAny<CancellationToken>())).ReturnsAsync(new List<Booking>());

        var result = await _bookingService.SearchAsync("xyz");

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
