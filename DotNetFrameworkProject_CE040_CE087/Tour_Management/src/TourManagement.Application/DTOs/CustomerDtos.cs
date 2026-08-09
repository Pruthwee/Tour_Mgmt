using System.ComponentModel.DataAnnotations;

namespace TourManagement.Application.DTOs;

public sealed record CustomerDto(int Id, string Email, string FirstName, string LastName, string? City, string? State);

public sealed class CustomerCreateDto
{
    [Required, EmailAddress, StringLength(256)] public string Email { get; set; } = string.Empty;
    [Required, StringLength(100)] public string FirstName { get; set; } = string.Empty;
    [Required, StringLength(100)] public string LastName { get; set; } = string.Empty;
    [StringLength(30)] public string? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    [StringLength(200)] public string? Street { get; set; }
    [StringLength(100)] public string? City { get; set; }
    [StringLength(100)] public string? State { get; set; }
}

public sealed class CustomerUpdateDto : CustomerCreateDto
{
}
