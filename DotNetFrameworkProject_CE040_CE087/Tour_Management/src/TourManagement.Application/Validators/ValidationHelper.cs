using System.ComponentModel.DataAnnotations;

namespace TourManagement.Application.Validators;

/// <summary>Centralized validation helper for DTO validation.</summary>
public static class ValidationHelper
{
    public static void Validate(object instance)
    {
        ArgumentNullException.ThrowIfNull(instance);
        var context = new ValidationContext(instance);
        Validator.ValidateObject(instance, context, validateAllProperties: true);
    }
}
