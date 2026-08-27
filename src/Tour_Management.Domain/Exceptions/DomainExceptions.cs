namespace Tour_Management.Domain.Exceptions;

/// <summary>
/// Exception thrown when a requested entity is not found.
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>Initializes a new instance of <see cref="NotFoundException"/>.</summary>
    public NotFoundException(string message) : base(message) { }

    /// <summary>Initializes a new instance of <see cref="NotFoundException"/> with entity details.</summary>
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found.") { }
}

/// <summary>
/// Exception thrown when a validation error occurs.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>Gets the validation errors.</summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    /// <summary>Initializes a new instance of <see cref="ValidationException"/>.</summary>
    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    /// <summary>Initializes a new instance of <see cref="ValidationException"/> with errors.</summary>
    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = new Dictionary<string, string[]>(errors);
    }
}

/// <summary>
/// Exception thrown when a duplicate entity is detected.
/// </summary>
public class DuplicateEntityException : Exception
{
    /// <summary>Initializes a new instance of <see cref="DuplicateEntityException"/>.</summary>
    public DuplicateEntityException(string message) : base(message) { }
}
