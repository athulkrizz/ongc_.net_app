namespace ONGC.MilestoneAPI.Models.Enums;

/// <summary>
/// Represents the type of milestone in the well design process
/// </summary>
public enum MilestoneType
{
    /// <summary>
    /// Geological and Geophysical data has been received
    /// </summary>
    GnGDataReceived = 0,

    /// <summary>
    /// Mechanical Data Transfer Conference has been conducted
    /// </summary>
    MDTConducted = 1,

    /// <summary>
    /// Drilling Well Plan sent to Directional and Fishing Services
    /// </summary>
    DWPSentDFS = 2,

    /// <summary>
    /// Drilling Well Plan received from Directional and Fishing Services
    /// </summary>
    DWPReceivedDFS = 3,

    /// <summary>
    /// Drilling Well Plan sent to Cementing team
    /// </summary>
    DWPSentCementing = 4,

    /// <summary>
    /// Drilling Well Plan received from Cementing team
    /// </summary>
    DWPReceivedCementing = 5,

    /// <summary>
    /// Design work has been initiated
    /// </summary>
    DesignInitiated = 6,

    /// <summary>
    /// Approval process has been initiated
    /// </summary>
    ApprovalInitiated = 7,

    /// <summary>
    /// Level 1 approval obtained
    /// </summary>
    Level1Approval = 8,

    /// <summary>
    /// Level 2 approval obtained
    /// </summary>
    Level2Approval = 9,

    /// <summary>
    /// Level 3 approval obtained (final approval)
    /// </summary>
    Level3Approval = 10
}
