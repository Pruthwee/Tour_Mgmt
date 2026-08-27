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
    public class UserRepositoryTests : IDisposable
    {
        private readonly TourManagementDbContext _context;
        private readonly Mock<ILogger<UserRepository>> _mockLogger;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TourManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TourManagementDbContext(options);
            _mockLogger = new Mock<ILogger<UserRepository>>();
            _repository = new UserRepository(_context, _mockLogger.Object);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllUsers()
        {
            // Arrange
            _context.UserInfos.AddRange(
                new UserInfo { Email = "user1@example.com", FirstName = "John", LastName = "Doe", Gender = "Male", Password = "hash1", Street = "St1", City = "City1", State = "S1" },
                new UserInfo { Email = "user2@example.com", FirstName = "Jane", LastName = "Smith", Gender = "Female", Password = "hash2", Street = "St2", City = "City2", State = "S2" }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            var list = new List<UserInfo>(result);
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmpty_WhenNoUsers()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsUser_WhenExists()
        {
            // Arrange
            var user = new UserInfo { Email = "john@example.com", FirstName = "John", LastName = "Doe", Gender = "Male", Password = "hash", Street = "St", City = "City", State = "S" };
            _context.UserInfos.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByEmailAsync("john@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("john@example.com", result.Email);
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsNull_WhenNotFound()
        {
            // Act
            var result = await _repository.GetByEmailAsync("notfound@example.com");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_AddsUser_Successfully()
        {
            // Arrange
            var user = new UserInfo { Email = "new@example.com", FirstName = "New", LastName = "User", Gender = "Male", Password = "hash", Street = "St", City = "City", State = "S" };

            // Act
            var result = await _repository.AddAsync(user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("new@example.com", result.Email);
            Assert.Equal(1, await _context.UserInfos.CountAsync());
        }

        [Fact]
        public async Task UpdateAsync_UpdatesUser_Successfully()
        {
            // Arrange
            var user = new UserInfo { Email = "john@example.com", FirstName = "John", LastName = "Doe", Gender = "Male", Password = "hash", Street = "St", City = "City", State = "S" };
            _context.UserInfos.Add(user);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            user.FirstName = "Johnny";

            // Act
            var result = await _repository.UpdateAsync(user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Johnny", result.FirstName);
        }

        [Fact]
        public async Task DeleteAsync_DeletesUser_WhenExists()
        {
            // Arrange
            var user = new UserInfo { Email = "john@example.com", FirstName = "John", LastName = "Doe", Gender = "Male", Password = "hash", Street = "St", City = "City", State = "S" };
            _context.UserInfos.Add(user);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync("john@example.com");

            // Assert
            Assert.Equal(0, await _context.UserInfos.CountAsync());
        }

        [Fact]
        public async Task DeleteAsync_DoesNotThrow_WhenUserNotFound()
        {
            // Act & Assert (should not throw)
            await _repository.DeleteAsync("notfound@example.com");
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenUserExists()
        {
            // Arrange
            var user = new UserInfo { Email = "john@example.com", FirstName = "John", LastName = "Doe", Gender = "Male", Password = "hash", Street = "St", City = "City", State = "S" };
            _context.UserInfos.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync("john@example.com");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalse_WhenUserNotFound()
        {
            // Act
            var result = await _repository.ExistsAsync("notfound@example.com");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task SearchAsync_ReturnsMatchingUsers_ByEmail()
        {
            // Arrange
            _context.UserInfos.AddRange(
                new UserInfo { Email = "john@example.com", FirstName = "John", LastName = "Doe", Gender = "Male", Password = "hash1", Street = "St1", City = "City1", State = "S1" },
                new UserInfo { Email = "jane@example.com", FirstName = "Jane", LastName = "Smith", Gender = "Female", Password = "hash2", Street = "St2", City = "City2", State = "S2" }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.SearchAsync("john");

            // Assert
            Assert.NotNull(result);
            var list = new List<UserInfo>(result);
            Assert.Single(list);
            Assert.Equal("john@example.com", list[0].Email);
        }

        [Fact]
        public async Task SearchAsync_ReturnsMatchingUsers_ByFirstName()
        {
            // Arrange
            _context.UserInfos.Add(new UserInfo { Email = "alice@example.com", FirstName = "Alice", LastName = "Wonder", Gender = "Female", Password = "hash", Street = "St", City = "City", State = "S" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.SearchAsync("alice");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task SearchAsync_ReturnsEmpty_WhenNoMatch()
        {
            // Arrange
            _context.UserInfos.Add(new UserInfo { Email = "john@example.com", FirstName = "John", LastName = "Doe", Gender = "Male", Password = "hash", Street = "St", City = "City", State = "S" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.SearchAsync("xyz");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task ValidateCredentialsAsync_ReturnsUser_WhenEmailExists()
        {
            // Arrange
            var user = new UserInfo { Email = "john@example.com", FirstName = "John", LastName = "Doe", Gender = "Male", Password = "hash", Street = "St", City = "City", State = "S" };
            _context.UserInfos.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ValidateCredentialsAsync("john@example.com", "anypassword");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("john@example.com", result.Email);
        }

        [Fact]
        public async Task ValidateCredentialsAsync_ReturnsNull_WhenEmailNotFound()
        {
            // Act
            var result = await _repository.ValidateCredentialsAsync("notfound@example.com", "password");

            // Assert
            Assert.Null(result);
        }
    }
}
