using System.ComponentModel.DataAnnotations;

namespace ONGC.MilestoneAPI.Models.Entities;

/// <summary>
/// Base entity class with common properties for audit trail
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Primary key identifier
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Timestamp when the entity was created
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the entity was last updated
    /// </summary>
    [Required]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// ID of the user who created this entity
    /// </summary>
    public int? CreatedBy { get; set; }

    /// <summary>
    /// ID of the user who last updated this entity
    /// </summary>
    public int? UpdatedBy { get; set; }
}
