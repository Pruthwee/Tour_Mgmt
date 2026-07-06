using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Tour_Management.Application.Mappings;
using Tour_Management.Application.Services;
using Tour_Management.Domain.DTOs;
using Tour_Management.Domain.Entities;
using Tour_Management.Domain.Exceptions;
using Tour_Management.Domain.Interfaces.Repositories;
using Xunit;

namespace Tour_Management.UnitTests.Services;

/// <summary>Unit tests for UserInfoService.</summary>
public class UserInfoServiceTests
{
    private readonly Mock<IUserInfoRepository> _mockRepository;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<UserInfoService>> _mockLogger;
    private readonly UserInfoService _service;

    public UserInfoServiceTests()
    {
        _mockRepository = new Mock<IUserInfoRepository>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _mockLogger = new Mock<ILogger<UserInfoService>>();
        _service = new UserInfoService(_mockRepository.Object, _mapper, _mockLogger.Object);
    }

    [Fact]
    public async Task RegisterAsync_WithNewEmail_ShouldCreateUser()
    {
        // Arrange
        var createDto = new UserInfoCreateDto
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "password123",
            Street = "123 Main St",
            City = "Mumbai",
            State = "Maharashtra"
        };
        var createdUser = new UserInfo
        {
            UserInfoId = 1,
            Email = createDto.Email,
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            IsActive = true
        };

        _mockRepository.Setup(r => r.GetByEmailAsync(createDto.Email, It.IsAny<CancellationToken>())).ReturnsAsync((UserInfo?)null);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>())).ReturnsAsync(createdUser);

        // Act
        var result = await _service.RegisterAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldThrowDuplicateEntityException()
    {
        // Arrange
        var createDto = new UserInfoCreateDto
        {
            Email = "existing@example.com",
            FirstName = "Jane",
            LastName = "Doe",
            Gender = "Female",
            Password = "password123",
            Street = "456 Oak Ave",
            City = "Delhi",
            State = "Delhi"
        };
        var existingUser = new UserInfo { UserInfoId = 1, Email = createDto.Email };
        _mockRepository.Setup(r => r.GetByEmailAsync(createDto.Email, It.IsAny<CancellationToken>())).ReturnsAsync(existingUser);

        // Act & Assert
        await _service.Invoking(s => s.RegisterAsync(createDto))
            .Should().ThrowAsync<DuplicateEntityException>();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<UserInfo>
        {
            new UserInfo { UserInfoId = 1, Email = "user1@example.com", FirstName = "Alice", LastName = "Smith", IsActive = true },
            new UserInfo { UserInfoId = 2, Email = "user2@example.com", FirstName = "Bob", LastName = "Jones", IsActive = true }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }
}
