using FluentValidation;
using Tour_Management.Application.DTOs;

namespace Tour_Management.Application.Validators;

/// <summary>Validator for UserCreateDto.</summary>
public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
{
    /// <summary>Initializes a new instance of <see cref="UserCreateDtoValidator"/>.</summary>
    public UserCreateDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(50).WithMessage("Email must not exceed 50 characters.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender is required.")
            .Must(g => g == "Male" || g == "Female").WithMessage("Gender must be 'Male' or 'Female'.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
            .MaximumLength(50).WithMessage("Password must not exceed 50 characters.");

        RuleFor(x => x.Dob)
            .NotEmpty().WithMessage("Date of birth is required.")
            .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past.");

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required.")
            .MaximumLength(50).WithMessage("Street must not exceed 50 characters.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(50).WithMessage("City must not exceed 50 characters.");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required.")
            .MaximumLength(50).WithMessage("State must not exceed 50 characters.");
    }
}

/// <summary>Validator for TourCreateDto.</summary>
public class TourCreateDtoValidator : AbstractValidator<TourCreateDto>
{
    /// <summary>Initializes a new instance of <see cref="TourCreateDtoValidator"/>.</summary>
    public TourCreateDtoValidator()
    {
        RuleFor(x => x.TourName)
            .NotEmpty().WithMessage("Tour name is required.")
            .MaximumLength(20).WithMessage("Tour name must not exceed 20 characters.");

        RuleFor(x => x.Place)
            .NotEmpty().WithMessage("Place is required.")
            .MaximumLength(20).WithMessage("Place must not exceed 20 characters.");

        RuleFor(x => x.Days)
            .GreaterThan(0).WithMessage("Days must be greater than 0.")
            .LessThanOrEqualTo(99).WithMessage("Days must not exceed 99.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.Locations)
            .NotEmpty().WithMessage("Locations are required.")
            .MaximumLength(100).WithMessage("Locations must not exceed 100 characters.");

        RuleFor(x => x.TourInfo)
            .NotEmpty().WithMessage("Tour information is required.")
            .MaximumLength(200).WithMessage("Tour information must not exceed 200 characters.");
    }
}

/// <summary>Validator for BookingCreateDto.</summary>
public class BookingCreateDtoValidator : AbstractValidator<BookingCreateDto>
{
    /// <summary>Initializes a new instance of <see cref="BookingCreateDtoValidator"/>.</summary>
    public BookingCreateDtoValidator()
    {
        RuleFor(x => x.TourName)
            .NotEmpty().WithMessage("Tour name is required.")
            .MaximumLength(50).WithMessage("Tour name must not exceed 50 characters.");

        RuleFor(x => x.Place)
            .NotEmpty().WithMessage("Place is required.")
            .MaximumLength(50).WithMessage("Place must not exceed 50 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(50).WithMessage("Email must not exceed 50 characters.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");
    }
}
