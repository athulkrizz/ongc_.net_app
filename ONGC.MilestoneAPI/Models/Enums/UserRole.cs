namespace ONGC.MilestoneAPI.Models.Enums;

/// <summary>
/// Represents the role of a user in the system
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Viewer role - can only view data
    /// </summary>
    Viewer = 0,

    /// <summary>
    /// User role - can create and edit data
    /// </summary>
    User = 1,

    /// <summary>
    /// Administrator role - full access to all features
    /// </summary>
    Admin = 2
}
