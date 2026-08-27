using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Tour_Management.Application.DTOs;
using Tour_Management.Application.Validators;

namespace Tour_Management.UnitTests.Validators;

public class ValidatorsTests
{
    private readonly UserCreateDtoValidator _userValidator = new UserCreateDtoValidator();
    private readonly TourCreateDtoValidator _tourValidator = new TourCreateDtoValidator();
    private readonly BookingCreateDtoValidator _bookingValidator = new BookingCreateDtoValidator();

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

    private TourCreateDto CreateValidTourDto() => new TourCreateDto
    {
        TourName = "Paris Tour",
        Place = "Paris",
        Days = 7,
        Price = 1500m,
        Locations = "Eiffel Tower, Louvre",
        TourInfo = "A wonderful tour of Paris"
    };

    private BookingCreateDto CreateValidBookingDto() => new BookingCreateDto
    {
        TourName = "Paris Tour",
        Place = "Paris",
        Email = "user@example.com",
        FirstName = "John"
    };

    // UserCreateDtoValidator Tests
    [Fact]
    public void UserCreateDtoValidator_ValidDto_PassesValidation()
    {
        var dto = CreateValidUserDto();
        var result = _userValidator.Validate(dto);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void UserCreateDtoValidator_EmptyEmail_FailsValidation()
    {
        var dto = CreateValidUserDto();
        dto.Email = "";
        var result = _userValidator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public void UserCreateDtoValidator_InvalidEmail_FailsValidation()
    {
        var dto = CreateValidUserDto();
        dto.Email = "not-an-email";
        var result = _userValidator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public void UserCreateDtoValidator_EmailTooLong_FailsValidation()
    {
        var dto = CreateValidUserDto();
        dto.Email = new string('a', 45) + "@test.com";
        var result = _userValidator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public void UserCreateDtoValidator_EmptyFirstName_FailsValidation()
    {
        var dto = CreateValidUserDto();
        dto.FirstName = "";
        var result = _userValidator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FirstName");
    }

    [Fact]
    public void UserCreateDtoValidator_EmptyLastName_FailsValidation()
    {
        var dto = CreateValidUserDto();
        dto.LastName = "";
        var result = _userValidator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "LastName");
    }

    [Fact]
    public void UserCreateDtoValidator_InvalidGender_FailsValidation()
    {
        var dto = CreateValidUserDto();
        dto.Gender = "Other";
        var result = _userValidator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Gender");
    }

    [Fact]
    public void UserCreateDtoValidator_ValidGenderMale_PassesValidation()
    {
        var dto = CreateValidUserDto();
        dto.Gender = "Male";
        var result = _userValidator.Validate(dto);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void UserCreateDtoValidator_ValidGenderFemale_PassesValidation()
    {
        var dto = CreateValidUserDto();
        dto.Gender = "Female";
        var result = _userValidator.Validate(dto);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void UserCreateDtoValidator_PasswordTooShort_FailsValidation()
    {
        var dto = CreateValidUserDto();
        dto.Password = "abc";
        var result = _userValidator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }

    [Fact]
    public void UserCreateDtoValidator_FutureDob_FailsValidation()
    {
        var dto = CreateValidUserDto();
        dto.Dob = DateTime.Today.AddDays(1);
        var result = _userValidator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Dob");
    }

    [Fact]
    public void UserCreateDtoValidator_EmptyStreet_FailsValidation()
    {
        var dto = CreateValidUserDto();
        dto.Street = "";
        var result = _userValidator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Street");
    }

    [Fact]
    public void UserCreateDtoValidator_EmptyCity_FailsValidation()
    {
        var dto = CreateValidUserDto();
        dto.City = "";
        var result = _userValidator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "City");
    }

    [Fact]
    public void UserCreateDtoValidator_EmptyState_FailsValidation()
    {
        var dto = CreateValidUserDto();
        dto.State = "";
        var result = _userValidator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "State");
    }

    // TourCreateDtoValidator Tests
    [Fact]
    public void TourCreateDtoValidator_ValidDto_PassesValidation()
    {
        var dto = CreateValidTourDto();
        var result = _tourValidator.Validate(dto);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void TourCreateDtoValidator_EmptyTourName_FailsValidation()
    {
        var dto = CreateValidTourDto();
        dto.TourName = "";
        var result = _tourValidator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TourName");
    }

    [Fact]
    public void TourCreateDtoValidator_TourNameTooLong_FailsValidation()
    {
        var dto = CreateValidTourDto();
        dto.TourName = new string('a', 21);
        var result = _tourValidator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TourName");
    }

    [Fact]
    public void TourCreateDtoValidator_ZeroDays_FailsValidation()
    {
        var dto = CreateValidTourDto();
        dto.Days = 0;
        var result = _tourValidator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Days");
    }

    [Fact]
    public void TourCreateDtoValidator_DaysExceedMax_FailsValidation()
    {
        var dto = CreateValidTourDto();
        dto.Days = 100;
        var result = _tourValidator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Days");
    }

    [Fact]
    public void TourCreateDtoValidator_ZeroPrice_FailsValidation()
    {
        var dto = CreateValidTourDto();
        dto.Price = 0m;
        var result = _tourValidator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Price");
    }

    [Fact]
    public void TourCreateDtoValidator_EmptyLocations_FailsValidation()
    {
        var dto = CreateValidTourDto();
        dto.Locations = "";
        var result = _tourValidator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Locations");
    }

    [Fact]
    public void TourCreateDtoValidator_EmptyTourInfo_FailsValidation()
    {
        var dto = CreateValidTourDto();
        dto.TourInfo = "";
        var result = _tourValidator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TourInfo");
    }

    [Fact]
    public void TourCreateDtoValidator_EmptyPlace_FailsValidation()
    {
        var dto = CreateValidTourDto();
        dto.Place = "";
        var result = _tourValidator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Place");
    }

    // BookingCreateDtoValidator Tests
    [Fact]
    public void BookingCreateDtoValidator_ValidDto_PassesValidation()
    {
        var dto = CreateValidBookingDto();
        var result = _bookingValidator.Validate(dto);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void BookingCreateDtoValidator_EmptyTourName_FailsValidation()
    {
        var dto = CreateValidBookingDto();
        dto.TourName = "";
        var result = _bookingValidator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TourName");
    }

    [Fact]
    public void BookingCreateDtoValidator_EmptyEmail_FailsValidation()
    {
        var dto = CreateValidBookingDto();
        dto.Email = "";
        var result = _bookingValidator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public void BookingCreateDtoValidator_InvalidEmail_FailsValidation()
    {
        var dto = CreateValidBookingDto();
        dto.Email = "not-an-email";
        var result = _bookingValidator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public void BookingCreateDtoValidator_EmptyFirstName_FailsValidation()
    {
        var dto = CreateValidBookingDto();
        dto.FirstName = "";
        var result = _bookingValidator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FirstName");
    }

    [Fact]
    public void BookingCreateDtoValidator_EmptyPlace_FailsValidation()
    {
        var dto = CreateValidBookingDto();
        dto.Place = "";
        var result = _bookingValidator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Place");
    }

    [Fact]
    public void BookingCreateDtoValidator_TourNameTooLong_FailsValidation()
    {
        var dto = CreateValidBookingDto();
        dto.TourName = new string('a', 51);
        var result = _bookingValidator.Validate(dto);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TourName");
    }
}
