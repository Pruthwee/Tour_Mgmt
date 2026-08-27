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

        // GetAllAsync Tests
        [Fact]
        public async Task GetAllAsync_ReturnsAllUsers()
        {
            // Arrange
            var users = new List<UserInfo>
            {
                new UserInfo { Email = "user1@example.com", FirstName = "John", LastName = "Doe", Role = "User" },
                new UserInfo { Email = "user2@example.com", FirstName = "Jane", LastName = "Smith", Role = "User" }
            };
            _mockUserRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            var list = new List<UserDto>(result);
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoUsers()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<UserInfo>());

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_ThrowsException_WhenRepositoryFails()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _userService.GetAllAsync());
        }

        // GetByEmailAsync Tests
        [Fact]
        public async Task GetByEmailAsync_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var user = new UserInfo { Email = "john@example.com", FirstName = "John", LastName = "Doe", Role = "User" };
            _mockUserRepository.Setup(r => r.GetByEmailAsync("john@example.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetByEmailAsync("john@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("john@example.com", result.Email);
            Assert.Equal("John", result.FirstName);
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsNull_WhenUserNotFound()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetByEmailAsync("notfound@example.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserInfo?)null);

            // Act
            var result = await _userService.GetByEmailAsync("notfound@example.com");

            // Assert
            Assert.Null(result);
        }

        // CreateAsync Tests
        [Fact]
        public async Task CreateAsync_CreatesUser_WhenEmailNotExists()
        {
            // Arrange
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
            var createdUser = new UserInfo
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Role = "User"
            };

            _mockUserRepository.Setup(r => r.ExistsAsync(dto.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdUser);

            // Act
            var result = await _userService.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("new@example.com", result.Email);
        }

        [Fact]
        public async Task CreateAsync_ThrowsDuplicateEntityException_WhenEmailExists()
        {
            // Arrange
            var dto = new UserCreateDto { Email = "existing@example.com", Password = "password123" };
            _mockUserRepository.Setup(r => r.ExistsAsync(dto.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<DuplicateEntityException>(() => _userService.CreateAsync(dto));
        }

        // UpdateAsync Tests
        [Fact]
        public async Task UpdateAsync_UpdatesUser_WhenUserExists()
        {
            // Arrange
            var email = "john@example.com";
            var existingUser = new UserInfo
            {
                Email = email,
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                Password = "hashed",
                Role = "User",
                Dob = new DateTime(1990, 1, 1),
                Street = "Old St",
                City = "Old City",
                State = "OS"
            };
            var updateDto = new UserUpdateDto
            {
                FirstName = "Johnny",
                LastName = "Doe",
                Gender = "Male",
                Dob = new DateTime(1990, 1, 1),
                Street = "New St",
                City = "New City",
                State = "NS"
            };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            // Act
            var result = await _userService.UpdateAsync(email, updateDto);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsNotFoundException_WhenUserNotFound()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetByEmailAsync("notfound@example.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserInfo?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _userService.UpdateAsync("notfound@example.com", new UserUpdateDto()));
        }

        // DeleteAsync Tests
        [Fact]
        public async Task DeleteAsync_DeletesUser_WhenUserExists()
        {
            // Arrange
            var email = "john@example.com";
            _mockUserRepository.Setup(r => r.ExistsAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mockUserRepository.Setup(r => r.DeleteAsync(email, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _userService.DeleteAsync(email);

            // Assert
            _mockUserRepository.Verify(r => r.DeleteAsync(email, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsNotFoundException_WhenUserNotFound()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.ExistsAsync("notfound@example.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _userService.DeleteAsync("notfound@example.com"));
        }

        // ValidateLoginAsync Tests
        [Fact]
        public async Task ValidateLoginAsync_ReturnsNull_WhenUserNotFound()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetByEmailAsync("notfound@example.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserInfo?)null);

            // Act
            var result = await _userService.ValidateLoginAsync("notfound@example.com", "password");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ValidateLoginAsync_ReturnsNull_WhenPasswordInvalid()
        {
            // Arrange
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correctpassword");
            var user = new UserInfo
            {
                Email = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                Role = "User",
                Password = hashedPassword
            };
            _mockUserRepository.Setup(r => r.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.ValidateLoginAsync("user@example.com", "wrongpassword");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ValidateLoginAsync_ReturnsUser_WhenCredentialsValid()
        {
            // Arrange
            var password = "correctpassword";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new UserInfo
            {
                Email = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                Role = "User",
                Password = hashedPassword
            };
            _mockUserRepository.Setup(r => r.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.ValidateLoginAsync("user@example.com", password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user@example.com", result.Email);
        }

        // SearchAsync Tests
        [Fact]
        public async Task SearchAsync_ReturnsMatchingUsers()
        {
            // Arrange
            var users = new List<UserInfo>
            {
                new UserInfo { Email = "john@example.com", FirstName = "John", LastName = "Doe", Role = "User" }
            };
            _mockUserRepository.Setup(r => r.SearchAsync("john", It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);

            // Act
            var result = await _userService.SearchAsync("john");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task SearchAsync_ReturnsEmpty_WhenNoMatch()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.SearchAsync("xyz", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<UserInfo>());

            // Act
            var result = await _userService.SearchAsync("xyz");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
