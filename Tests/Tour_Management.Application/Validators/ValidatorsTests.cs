using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentValidation;
using Tour_Management.Application.DTOs;
using Tour_Management.Application.Validators;

namespace Tour_Management.Application.Validators.Tests
{
    public class ValidatorsTests
    {
        // UserCreateDtoValidator Tests
        private readonly UserCreateDtoValidator _userValidator = new UserCreateDtoValidator();

        private UserCreateDto CreateValidUserDto() => new UserCreateDto
        {
            Email = "john@example.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "password123",
            Dob = new DateTime(1990, 1, 1),
            Street = "123 Main St",
            City = "New York",
            State = "NY"
        };

        [Fact]
        public void UserCreateDtoValidator_ValidDto_PassesValidation()
        {
            // Arrange
            var dto = CreateValidUserDto();

            // Act
            var result = _userValidator.Validate(dto);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void UserCreateDtoValidator_EmptyEmail_FailsValidation()
        {
            // Arrange
            var dto = CreateValidUserDto();
            dto.Email = "";

            // Act
            var result = _userValidator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Email");
        }

        [Fact]
        public void UserCreateDtoValidator_InvalidEmail_FailsValidation()
        {
            // Arrange
            var dto = CreateValidUserDto();
            dto.Email = "not-an-email";

            // Act
            var result = _userValidator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Email");
        }

        [Fact]
        public void UserCreateDtoValidator_EmailTooLong_FailsValidation()
        {
            // Arrange
            var dto = CreateValidUserDto();
            dto.Email = new string('a', 45) + "@test.com";

            // Act
            var result = _userValidator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Email");
        }

        [Fact]
        public void UserCreateDtoValidator_EmptyFirstName_FailsValidation()
        {
            // Arrange
            var dto = CreateValidUserDto();
            dto.FirstName = "";

            // Act
            var result = _userValidator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "FirstName");
        }

        [Fact]
        public void UserCreateDtoValidator_EmptyLastName_FailsValidation()
        {
            // Arrange
            var dto = CreateValidUserDto();
            dto.LastName = "";

            // Act
            var result = _userValidator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "LastName");
        }

        [Fact]
        public void UserCreateDtoValidator_InvalidGender_FailsValidation()
        {
            // Arrange
            var dto = CreateValidUserDto();
            dto.Gender = "Other";

            // Act
            var result = _userValidator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Gender");
        }

        [Fact]
        public void UserCreateDtoValidator_ValidGenderMale_PassesValidation()
        {
            // Arrange
            var dto = CreateValidUserDto();
            dto.Gender = "Male";

            // Act
            var result = _userValidator.Validate(dto);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void UserCreateDtoValidator_ValidGenderFemale_PassesValidation()
        {
            // Arrange
            var dto = CreateValidUserDto();
            dto.Gender = "Female";

            // Act
            var result = _userValidator.Validate(dto);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void UserCreateDtoValidator_PasswordTooShort_FailsValidation()
        {
            // Arrange
            var dto = CreateValidUserDto();
            dto.Password = "abc";

            // Act
            var result = _userValidator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Password");
        }

        [Fact]
        public void UserCreateDtoValidator_FutureDob_FailsValidation()
        {
            // Arrange
            var dto = CreateValidUserDto();
            dto.Dob = DateTime.Today.AddDays(1);

            // Act
            var result = _userValidator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Dob");
        }

        [Fact]
        public void UserCreateDtoValidator_EmptyStreet_FailsValidation()
        {
            // Arrange
            var dto = CreateValidUserDto();
            dto.Street = "";

            // Act
            var result = _userValidator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Street");
        }

        [Fact]
        public void UserCreateDtoValidator_EmptyCity_FailsValidation()
        {
            // Arrange
            var dto = CreateValidUserDto();
            dto.City = "";

            // Act
            var result = _userValidator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "City");
        }

        [Fact]
        public void UserCreateDtoValidator_EmptyState_FailsValidation()
        {
            // Arrange
            var dto = CreateValidUserDto();
            dto.State = "";

            // Act
            var result = _userValidator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "State");
        }

        // TourCreateDtoValidator Tests
        private readonly TourCreateDtoValidator _tourValidator = new TourCreateDtoValidator();

        private TourCreateDto CreateValidTourDto() => new TourCreateDto
        {
            TourName = "Paris Tour",
            Place = "Paris",
            Days = 7,
            Price = 1500m,
            Locations = "Eiffel Tower, Louvre",
            TourInfo = "A wonderful tour of Paris"
        };

        [Fact]
        public void TourCreateDtoValidator_ValidDto_PassesValidation()
        {
            // Arrange
            var dto = CreateValidTourDto();

            // Act
            var result = _tourValidator.Validate(dto);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void TourCreateDtoValidator_EmptyTourName_FailsValidation()
        {
            // Arrange
            var dto = CreateValidTourDto();
            dto.TourName = "";

            // Act
            var result = _tourValidator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "TourName");
        }

        [Fact]
        public void TourCreateDtoValidator_TourNameTooLong_FailsValidation()
        {
            // Arrange
            var dto = CreateValidTourDto();
            dto.TourName = new string('a', 21);

            // Act
            var result = _tourValidator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "TourName");
        }

        [Fact]
        public void TourCreateDtoValidator_ZeroDays_FailsValidation()
        {
            // Arrange
            var dto = CreateValidTourDto();
            dto.Days = 0;

            // Act
            var result = _tourValidator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Days");
        }

        [Fact]
        public void TourCreateDtoValidator_DaysExceedMax_FailsValidation()
        {
            // Arrange
            var dto = CreateValidTourDto();
            dto.Days = 100;

            // Act
            var result = _tourValidator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Days");
        }

        [Fact]
        public void TourCreateDtoValidator_ZeroPrice_FailsValidation()
        {
            // Arrange
            var dto = CreateValidTourDto();
            dto.Price = 0m;

            // Act
            var result = _tourValidator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Price");
        }

        [Fact]
        public void TourCreateDtoValidator_EmptyLocations_FailsValidation()
        {
            // Arrange
            var dto = CreateValidTourDto();
            dto.Locations = "";

            // Act
            var result = _tourValidator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Locations");
        }

        [Fact]
        public void TourCreateDtoValidator_EmptyTourInfo_FailsValidation()
        {
            // Arrange
            var dto = CreateValidTourDto();
            dto.TourInfo = "";

            // Act
            var result = _tourValidator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "TourInfo");
        }

        [Fact]
        public void TourCreateDtoValidator_EmptyPlace_FailsValidation()
        {
            // Arrange
            var dto = CreateValidTourDto();
            dto.Place = "";

            // Act
            var result = _tourValidator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Place");
        }

        // BookingCreateDtoValidator Tests
        private readonly BookingCreateDtoValidator _bookingValidator = new BookingCreateDtoValidator();

        private BookingCreateDto CreateValidBookingDto() => new BookingCreateDto
        {
            TourName = "Paris Tour",
            Place = "Paris",
            Email = "user@example.com",
            FirstName = "John"
        };

        [Fact]
        public void BookingCreateDtoValidator_ValidDto_PassesValidation()
        {
            // Arrange
            var dto = CreateValidBookingDto();

            // Act
            var result = _bookingValidator.Validate(dto);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void BookingCreateDtoValidator_EmptyTourName_FailsValidation()
        {
            // Arrange
            var dto = CreateValidBookingDto();
            dto.TourName = "";

            // Act
            var result = _bookingValidator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "TourName");
        }

        [Fact]
        public void BookingCreateDtoValidator_EmptyEmail_FailsValidation()
        {
            // Arrange
            var dto = CreateValidBookingDto();
            dto.Email = "";

            // Act
            var result = _bookingValidator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Email");
        }

        [Fact]
        public void BookingCreateDtoValidator_InvalidEmail_FailsValidation()
        {
            // Arrange
            var dto = CreateValidBookingDto();
            dto.Email = "not-an-email";

            // Act
            var result = _bookingValidator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Email");
        }

        [Fact]
        public void BookingCreateDtoValidator_EmptyFirstName_FailsValidation()
        {
            // Arrange
            var dto = CreateValidBookingDto();
            dto.FirstName = "";

            // Act
            var result = _bookingValidator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "FirstName");
        }

        [Fact]
        public void BookingCreateDtoValidator_EmptyPlace_FailsValidation()
        {
            // Arrange
            var dto = CreateValidBookingDto();
            dto.Place = "";

            // Act
            var result = _bookingValidator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Place");
        }

        [Fact]
        public void BookingCreateDtoValidator_TourNameTooLong_FailsValidation()
        {
            // Arrange
            var dto = CreateValidBookingDto();
            dto.TourName = new string('a', 51);

            // Act
            var result = _bookingValidator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "TourName");
        }
    }
}
