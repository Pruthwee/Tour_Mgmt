namespace TourManagement.Domain.Entities;

/// <summary>Base entity properties shared by all aggregate roots.</summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string CreatedBy { get; set; } = "system";
    public string? ModifiedBy { get; set; }
}
