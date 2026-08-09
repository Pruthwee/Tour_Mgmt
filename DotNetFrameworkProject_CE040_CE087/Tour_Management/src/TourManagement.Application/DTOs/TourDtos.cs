using System.ComponentModel.DataAnnotations;

namespace TourManagement.Application.DTOs;

public sealed record TourDto(int Id, string Name, string Place, int Days, decimal Price, string Locations, string Description, string? PictureFileName);

public sealed class TourCreateDto
{
    [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
    [Required, StringLength(150)] public string Place { get; set; } = string.Empty;
    [Range(1, 365)] public int Days { get; set; }
    [Range(0.01, 999999)] public decimal Price { get; set; }
    [Required, StringLength(500)] public string Locations { get; set; } = string.Empty;
    [Required, StringLength(4000)] public string Description { get; set; } = string.Empty;
    [StringLength(255)] public string? PictureFileName { get; set; }
}

public sealed class TourUpdateDto : TourCreateDto
{
}
