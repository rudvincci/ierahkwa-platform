namespace Mamey.Portal.Citizenship.Infrastructure.Persistence;

/// <summary>
/// Tracks citizenship status for approved applicants
/// </summary>
public sealed class CitizenshipStatusRow
{
    public Guid Id { get; set; }

    public string TenantId { get; set; } = string.Empty;
    
    /// <summary>
    /// Email of the citizen (used to link to applications)
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Current citizenship status (Probationary, Resident, Citizen)
    /// </summary>
    public string Status { get; set; } = "Probationary";

    /// <summary>
    /// Application ID that granted this status
    /// </summary>
    public Guid ApplicationId { get; set; }

    /// <summary>
    /// Date when this status was granted
    /// </summary>
    public DateTimeOffset StatusGrantedAt { get; set; }

    /// <summary>
    /// Date when this status expires (for Probationary/Resident) or null for permanent (Citizen)
    /// </summary>
    public DateTimeOffset? StatusExpiresAt { get; set; }

    /// <summary>
    /// Total years completed (for tracking progression eligibility)
    /// </summary>
    public int YearsCompleted { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Status progression applications (for Resident/Citizen status)
    /// </summary>
    public List<StatusProgressionApplicationRow> ProgressionApplications { get; set; } = new();
}


