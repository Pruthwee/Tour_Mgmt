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

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<UserService>>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _userService = new UserService(_mockUserRepository.Object, _mapper, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        var users = new List<UserInfo>
        {
            new UserInfo { Email = "user1@example.com", FirstName = "John", LastName = "Doe", Role = "User" },
            new UserInfo { Email = "user2@example.com", FirstName = "Jane", LastName = "Smith", Role = "User" }
        };
        _mockUserRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

        var result = await _userService.GetAllAsync();

        Assert.NotNull(result);
        var list = new List<UserDto>(result);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoUsers()
    {
        _mockUserRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<UserInfo>());

        var result = await _userService.GetAllAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ThrowsException_WhenRepositoryFails()
    {
        _mockUserRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        await Assert.ThrowsAsync<Exception>(() => _userService.GetAllAsync());
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsUser_WhenUserExists()
    {
        var user = new UserInfo { Email = "john@example.com", FirstName = "John", LastName = "Doe", Role = "User" };
        _mockUserRepository.Setup(r => r.GetByEmailAsync("john@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _userService.GetByEmailAsync("john@example.com");

        Assert.NotNull(result);
        Assert.Equal("john@example.com", result.Email);
        Assert.Equal("John", result.FirstName);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsNull_WhenUserNotFound()
    {
        _mockUserRepository.Setup(r => r.GetByEmailAsync("notfound@example.com", It.IsAny<CancellationToken>())).ReturnsAsync((UserInfo?)null);

        var result = await _userService.GetByEmailAsync("notfound@example.com");

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_CreatesUser_WhenEmailNotExists()
    {
        var dto = new UserCreateDto
        {
            Email = "new@example.com",
            FirstName = "New",
            LastName = "User",
            Gender = "Male",
            Password = "password123",
            Dob = new DateTime(1990, 1, 1),
            Street = "123 Main St",
            City = "NYC",
            State = "NY"
        };
        var createdUser = new UserInfo { Email = dto.Email, FirstName = dto.FirstName, LastName = dto.LastName, Role = "User" };

        _mockUserRepository.Setup(r => r.ExistsAsync(dto.Email, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>())).ReturnsAsync(createdUser);

        var result = await _userService.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("new@example.com", result.Email);
    }

    [Fact]
    public async Task CreateAsync_ThrowsDuplicateEntityException_WhenEmailExists()
    {
        var dto = new UserCreateDto { Email = "existing@example.com", Password = "password123" };
        _mockUserRepository.Setup(r => r.ExistsAsync(dto.Email, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        await Assert.ThrowsAsync<DuplicateEntityException>(() => _userService.CreateAsync(dto));
    }

    [Fact]
    public async Task UpdateAsync_UpdatesUser_WhenUserExists()
    {
        var email = "john@example.com";
        var existingUser = new UserInfo
        {
            Email = email, FirstName = "John", LastName = "Doe", Gender = "Male",
            Password = "hashed", Role = "User", Dob = new DateTime(1990, 1, 1),
            Street = "Old St", City = "Old City", State = "OS"
        };
        var updateDto = new UserUpdateDto
        {
            FirstName = "Johnny", LastName = "Doe", Gender = "Male",
            Dob = new DateTime(1990, 1, 1), Street = "New St", City = "New City", State = "NS"
        };

        _mockUserRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync(existingUser);
        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>())).ReturnsAsync(existingUser);

        var result = await _userService.UpdateAsync(email, updateDto);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsNotFoundException_WhenUserNotFound()
    {
        _mockUserRepository.Setup(r => r.GetByEmailAsync("notfound@example.com", It.IsAny<CancellationToken>())).ReturnsAsync((UserInfo?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _userService.UpdateAsync("notfound@example.com", new UserUpdateDto()));
    }

    [Fact]
    public async Task DeleteAsync_DeletesUser_WhenUserExists()
    {
        var email = "john@example.com";
        _mockUserRepository.Setup(r => r.ExistsAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.DeleteAsync(email, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await _userService.DeleteAsync(email);

        _mockUserRepository.Verify(r => r.DeleteAsync(email, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsNotFoundException_WhenUserNotFound()
    {
        _mockUserRepository.Setup(r => r.ExistsAsync("notfound@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(false);

        await Assert.ThrowsAsync<NotFoundException>(() => _userService.DeleteAsync("notfound@example.com"));
    }

    [Fact]
    public async Task ValidateLoginAsync_ReturnsNull_WhenUserNotFound()
    {
        _mockUserRepository.Setup(r => r.GetByEmailAsync("notfound@example.com", It.IsAny<CancellationToken>())).ReturnsAsync((UserInfo?)null);

        var result = await _userService.ValidateLoginAsync("notfound@example.com", "password");

        Assert.Null(result);
    }

    [Fact]
    public async Task ValidateLoginAsync_ReturnsNull_WhenPasswordInvalid()
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correctpassword");
        var user = new UserInfo { Email = "user@example.com", FirstName = "John", LastName = "Doe", Role = "User", Password = hashedPassword };
        _mockUserRepository.Setup(r => r.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _userService.ValidateLoginAsync("user@example.com", "wrongpassword");

        Assert.Null(result);
    }

    [Fact]
    public async Task ValidateLoginAsync_ReturnsUser_WhenCredentialsValid()
    {
        var password = "correctpassword";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new UserInfo { Email = "user@example.com", FirstName = "John", LastName = "Doe", Role = "User", Password = hashedPassword };
        _mockUserRepository.Setup(r => r.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _userService.ValidateLoginAsync("user@example.com", password);

        Assert.NotNull(result);
        Assert.Equal("user@example.com", result.Email);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingUsers()
    {
        var users = new List<UserInfo>
        {
            new UserInfo { Email = "john@example.com", FirstName = "John", LastName = "Doe", Role = "User" }
        };
        _mockUserRepository.Setup(r => r.SearchAsync("john", It.IsAny<CancellationToken>())).ReturnsAsync(users);

        var result = await _userService.SearchAsync("john");

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenNoMatch()
    {
        _mockUserRepository.Setup(r => r.SearchAsync("xyz", It.IsAny<CancellationToken>())).ReturnsAsync(new List<UserInfo>());

        var result = await _userService.SearchAsync("xyz");

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
