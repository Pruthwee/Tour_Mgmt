using FluentValidation;
using Tour_Management.Domain.DTOs;

namespace Tour_Management.Application.Validators;

/// <summary>Validator for BookingCreateDto.</summary>
public class BookingCreateDtoValidator : AbstractValidator<BookingCreateDto>
{
    public BookingCreateDtoValidator()
    {
        RuleFor(x => x.TourName)
            .NotEmpty().WithMessage("Tour name is required.")
            .MaximumLength(200).WithMessage("Tour name must not exceed 200 characters.");

        RuleFor(x => x.Place)
            .NotEmpty().WithMessage("City/Place is required.")
            .MaximumLength(200).WithMessage("City/Place must not exceed 200 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Mobile number or email is required.")
            .MaximumLength(100).WithMessage("Email/mobile must not exceed 100 characters.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters.");
    }
}

/// <summary>Validator for BookingUpdateDto.</summary>
public class BookingUpdateDtoValidator : AbstractValidator<BookingUpdateDto>
{
    public BookingUpdateDtoValidator()
    {
        RuleFor(x => x.TourName)
            .NotEmpty().WithMessage("Tour name is required.")
            .MaximumLength(200).WithMessage("Tour name must not exceed 200 characters.");

        RuleFor(x => x.Place)
            .NotEmpty().WithMessage("City/Place is required.")
            .MaximumLength(200).WithMessage("City/Place must not exceed 200 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Mobile number or email is required.")
            .MaximumLength(100).WithMessage("Email/mobile must not exceed 100 characters.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters.");
    }
}
