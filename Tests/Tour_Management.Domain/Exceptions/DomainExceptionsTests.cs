using System;
using System.Collections.Generic;
using Xunit;
using Tour_Management.Domain.Exceptions;

namespace Tour_Management.Domain.Exceptions.Tests
{
    public class DomainExceptionsTests
    {
        // NotFoundException Tests
        [Fact]
        public void NotFoundException_WithMessage_SetsMessage()
        {
            // Arrange & Act
            var ex = new NotFoundException("Entity not found");

            // Assert
            Assert.Equal("Entity not found", ex.Message);
        }

        [Fact]
        public void NotFoundException_WithEntityNameAndKey_FormatsMessage()
        {
            // Arrange & Act
            var ex = new NotFoundException("Tour", 42);

            // Assert
            Assert.Contains("Tour", ex.Message);
            Assert.Contains("42", ex.Message);
        }

        [Fact]
        public void NotFoundException_WithStringKey_FormatsMessage()
        {
            // Arrange & Act
            var ex = new NotFoundException("UserInfo", "user@example.com");

            // Assert
            Assert.Contains("UserInfo", ex.Message);
            Assert.Contains("user@example.com", ex.Message);
        }

        [Fact]
        public void NotFoundException_IsException()
        {
            // Arrange & Act
            var ex = new NotFoundException("Test");

            // Assert
            Assert.IsAssignableFrom<Exception>(ex);
        }

        [Fact]
        public void NotFoundException_CanBeCaught_AsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<NotFoundException>(() =>
            {
                throw new NotFoundException("Not found");
            });
        }

        // ValidationException Tests
        [Fact]
        public void ValidationException_WithMessage_SetsMessage()
        {
            // Arrange & Act
            var ex = new ValidationException("Validation failed");

            // Assert
            Assert.Equal("Validation failed", ex.Message);
        }

        [Fact]
        public void ValidationException_WithMessage_HasEmptyErrors()
        {
            // Arrange & Act
            var ex = new ValidationException("Validation failed");

            // Assert
            Assert.NotNull(ex.Errors);
            Assert.Empty(ex.Errors);
        }

        [Fact]
        public void ValidationException_WithErrors_SetsErrors()
        {
            // Arrange
            var errors = new Dictionary<string, string[]>
            {
                { "Email", new[] { "Email is required.", "Invalid email format." } },
                { "Password", new[] { "Password is too short." } }
            };

            // Act
            var ex = new ValidationException(errors);

            // Assert
            Assert.Equal("One or more validation errors occurred.", ex.Message);
            Assert.Equal(2, ex.Errors.Count);
            Assert.Contains("Email", ex.Errors.Keys);
            Assert.Contains("Password", ex.Errors.Keys);
        }

        [Fact]
        public void ValidationException_WithErrors_ErrorsAreReadOnly()
        {
            // Arrange
            var errors = new Dictionary<string, string[]>
            {
                { "Field", new[] { "Error message" } }
            };

            // Act
            var ex = new ValidationException(errors);

            // Assert
            Assert.IsAssignableFrom<IReadOnlyDictionary<string, string[]>>(ex.Errors);
        }

        [Fact]
        public void ValidationException_IsException()
        {
            // Arrange & Act
            var ex = new ValidationException("Test");

            // Assert
            Assert.IsAssignableFrom<Exception>(ex);
        }

        [Fact]
        public void ValidationException_CanBeCaught_AsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ValidationException>(() =>
            {
                throw new ValidationException("Validation error");
            });
        }

        // DuplicateEntityException Tests
        [Fact]
        public void DuplicateEntityException_WithMessage_SetsMessage()
        {
            // Arrange & Act
            var ex = new DuplicateEntityException("Entity already exists");

            // Assert
            Assert.Equal("Entity already exists", ex.Message);
        }

        [Fact]
        public void DuplicateEntityException_IsException()
        {
            // Arrange & Act
            var ex = new DuplicateEntityException("Test");

            // Assert
            Assert.IsAssignableFrom<Exception>(ex);
        }

        [Fact]
        public void DuplicateEntityException_CanBeCaught_AsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<DuplicateEntityException>(() =>
            {
                throw new DuplicateEntityException("Duplicate entity");
            });
        }

        [Fact]
        public void DuplicateEntityException_MessageContainsEmailInfo()
        {
            // Arrange
            var email = "user@example.com";

            // Act
            var ex = new DuplicateEntityException($"A user with email '{email}' already exists.");

            // Assert
            Assert.Contains(email, ex.Message);
        }
    }
}
