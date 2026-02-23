namespace Mamey.Portal.Citizenship.Infrastructure.Persistence;

/// <summary>
/// Tracks status progression applications (Probationary → Resident → Citizen)
/// </summary>
public sealed class StatusProgressionApplicationRow
{
    public Guid Id { get; set; }

    public string TenantId { get; set; } = string.Empty;
    
    /// <summary>
    /// Reference to the citizenship status record
    /// </summary>
    public Guid CitizenshipStatusId { get; set; }

    /// <summary>
    /// Application number for this progression application
    /// </summary>
    public string ApplicationNumber { get; set; } = string.Empty;

    /// <summary>
    /// Target status being applied for (Resident or Citizen)
    /// </summary>
    public string TargetStatus { get; set; } = string.Empty;

    /// <summary>
    /// Status of this progression application
    /// </summary>
    public string Status { get; set; } = "Submitted";

    /// <summary>
    /// Years completed at time of application
    /// </summary>
    public int YearsCompletedAtApplication { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}


