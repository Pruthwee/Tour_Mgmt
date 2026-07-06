using FluentValidation;
using Tour_Management.Domain.DTOs;

namespace Tour_Management.Application.Validators;

/// <summary>Validator for TourCreateDto.</summary>
public class TourCreateDtoValidator : AbstractValidator<TourCreateDto>
{
    public TourCreateDtoValidator()
    {
        RuleFor(x => x.TourName)
            .NotEmpty().WithMessage("Tour name is required.")
            .MaximumLength(200).WithMessage("Tour name must not exceed 200 characters.");

        RuleFor(x => x.Place)
            .NotEmpty().WithMessage("Place is required.")
            .MaximumLength(200).WithMessage("Place must not exceed 200 characters.");

        RuleFor(x => x.Days)
            .GreaterThan(0).WithMessage("Days must be greater than 0.")
            .LessThanOrEqualTo(365).WithMessage("Days must not exceed 365.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.Locations)
            .NotEmpty().WithMessage("Locations are required.")
            .MaximumLength(500).WithMessage("Locations must not exceed 500 characters.");

        RuleFor(x => x.TourInfo)
            .NotEmpty().WithMessage("Tour info is required.")
            .MaximumLength(250).WithMessage("Tour info must not exceed 250 characters.");
    }
}

/// <summary>Validator for TourUpdateDto.</summary>
public class TourUpdateDtoValidator : AbstractValidator<TourUpdateDto>
{
    public TourUpdateDtoValidator()
    {
        RuleFor(x => x.TourName)
            .NotEmpty().WithMessage("Tour name is required.")
            .MaximumLength(200).WithMessage("Tour name must not exceed 200 characters.");

        RuleFor(x => x.Place)
            .NotEmpty().WithMessage("Place is required.")
            .MaximumLength(200).WithMessage("Place must not exceed 200 characters.");

        RuleFor(x => x.Days)
            .GreaterThan(0).WithMessage("Days must be greater than 0.")
            .LessThanOrEqualTo(365).WithMessage("Days must not exceed 365.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.Locations)
            .NotEmpty().WithMessage("Locations are required.")
            .MaximumLength(500).WithMessage("Locations must not exceed 500 characters.");

        RuleFor(x => x.TourInfo)
            .NotEmpty().WithMessage("Tour info is required.")
            .MaximumLength(250).WithMessage("Tour info must not exceed 250 characters.");
    }
}
