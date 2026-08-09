using System.ComponentModel.DataAnnotations;

namespace TourManagement.Application.DTOs;

public sealed record BookingDto(int Id, int TourId, string TourName, string CustomerEmail, string CustomerName, DateTime BookingDate, string Status);

public sealed class BookingCreateDto
{
    [Range(1, int.MaxValue)] public int TourId { get; set; }
    [Required, EmailAddress, StringLength(256)] public string CustomerEmail { get; set; } = string.Empty;
    [Required, StringLength(200)] public string CustomerName { get; set; } = string.Empty;
}

public sealed class BookingUpdateDto : BookingCreateDto
{
    [Required, StringLength(50)] public string Status { get; set; } = "Pending";
}
