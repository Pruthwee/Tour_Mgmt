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

public class TourServiceExtendedTests
{
    private readonly Mock<ITourRepository> _mockTourRepository;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<TourService>> _mockLogger;
    private readonly TourService _tourService;

    public TourServiceExtendedTests()
    {
        _mockTourRepository = new Mock<ITourRepository>();
        _mockLogger = new Mock<ILogger<TourService>>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _tourService = new TourService(_mockTourRepository.Object, _mapper, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllTours()
    {
        var tours = new List<Tour>
        {
            new Tour { TourId = 1, TourName = "Paris Tour", Place = "Paris", Days = 7, Price = 1500m, IsActive = true },
            new Tour { TourId = 2, TourName = "Rome Tour", Place = "Rome", Days = 5, Price = 1200m, IsActive = true }
        };
        _mockTourRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tours);

        var result = await _tourService.GetAllAsync();

        Assert.NotNull(result);
        var list = new List<TourDto>(result);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoTours()
    {
        _mockTourRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Tour>());

        var result = await _tourService.GetAllAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ThrowsException_WhenRepositoryFails()
    {
        _mockTourRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        await Assert.ThrowsAsync<Exception>(() => _tourService.GetAllAsync());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsTour_WhenTourExists()
    {
        var tour = new Tour { TourId = 1, TourName = "Paris Tour", Place = "Paris", Days = 7, Price = 1500m, IsActive = true };
        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        var result = await _tourService.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.TourId);
        Assert.Equal("Paris Tour", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenTourNotFound()
    {
        _mockTourRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        var result = await _tourService.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_CreatesTour_Successfully()
    {
        var dto = new TourCreateDto
        {
            TourName = "New Tour", Place = "London", Days = 5, Price = 1000m,
            Locations = "Big Ben, Tower Bridge", TourInfo = "A great London tour"
        };
        var createdTour = new Tour
        {
            TourId = 1, TourName = dto.TourName, Place = dto.Place,
            Days = dto.Days, Price = dto.Price, IsActive = true
        };
        _mockTourRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>())).ReturnsAsync(createdTour);

        var result = await _tourService.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("New Tour", result.TourName);
        Assert.Equal(1, result.TourId);
    }

    [Fact]
    public async Task CreateAsync_ThrowsException_WhenRepositoryFails()
    {
        var dto = new TourCreateDto { TourName = "Test", Place = "Test", Days = 1, Price = 100m };
        _mockTourRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        await Assert.ThrowsAsync<Exception>(() => _tourService.CreateAsync(dto));
    }

    [Fact]
    public async Task UpdateAsync_UpdatesTour_WhenTourExists()
    {
        var existingTour = new Tour
        {
            TourId = 1, TourName = "Old Tour", Place = "Old Place", Days = 3,
            Price = 500m, Locations = "Old Loc", TourInfo = "Old Info", IsActive = true
        };
        var updateDto = new TourUpdateDto
        {
            TourName = "Updated Tour", Place = "New Place", Days = 5,
            Price = 800m, Locations = "New Loc", TourInfo = "New Info", IsActive = true
        };
        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingTour);
        _mockTourRepository.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>())).ReturnsAsync(existingTour);

        var result = await _tourService.UpdateAsync(1, updateDto);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsNotFoundException_WhenTourNotFound()
    {
        _mockTourRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _tourService.UpdateAsync(999, new TourUpdateDto()));
    }

    [Fact]
    public async Task DeleteAsync_DeletesTour_WhenTourExists()
    {
        _mockTourRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockTourRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await _tourService.DeleteAsync(1);

        _mockTourRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingTours()
    {
        var tours = new List<Tour>
        {
            new Tour { TourId = 1, TourName = "Paris Tour", Place = "Paris", Days = 7, Price = 1500m, IsActive = true }
        };
        _mockTourRepository.Setup(r => r.SearchAsync("Paris", It.IsAny<CancellationToken>())).ReturnsAsync(tours);

        var result = await _tourService.SearchAsync("Paris");

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenNoMatch()
    {
        _mockTourRepository.Setup(r => r.SearchAsync("xyz", It.IsAny<CancellationToken>())).ReturnsAsync(new List<Tour>());

        var result = await _tourService.SearchAsync("xyz");

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
