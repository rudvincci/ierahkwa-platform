using Mamey.FWID.Identities.Application.ClanRegistrar.Models;

namespace Mamey.FWID.Identities.Application.ClanRegistrar.Services;

/// <summary>
/// Interface for clan registrar service.
/// Manages clan registrar workflows and identity approvals.
/// </summary>
public interface IClanRegistrarService
{
    #region Registrar Management
    
    /// <summary>
    /// Gets a registrar by ID.
    /// </summary>
    Task<Models.ClanRegistrar?> GetRegistrarAsync(Guid registrarId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a registrar by identity ID.
    /// </summary>
    Task<Models.ClanRegistrar?> GetRegistrarByIdentityAsync(Guid identityId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all registrars for a zone.
    /// </summary>
    Task<IReadOnlyList<Models.ClanRegistrar>> GetRegistrarsByZoneAsync(string zone, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the appropriate registrar for an identity registration.
    /// </summary>
    Task<Models.ClanRegistrar?> FindRegistrarForIdentityAsync(
        Guid identityId,
        string zone,
        string? clanName = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Registers a new clan registrar.
    /// </summary>
    Task<Models.ClanRegistrar> RegisterRegistrarAsync(
        RegisterRegistrarRequest request,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deactivates a registrar.
    /// </summary>
    Task<bool> DeactivateRegistrarAsync(Guid registrarId, string reason, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delegates authority to another registrar.
    /// </summary>
    Task<bool> DelegateAuthorityAsync(
        Guid fromRegistrarId,
        Guid toRegistrarId,
        RegistrarScope scope,
        CancellationToken cancellationToken = default);
    
    #endregion
    
    #region Approval Workflow
    
    /// <summary>
    /// Submits an identity registration for approval.
    /// </summary>
    Task<RegistrationApproval> SubmitForApprovalAsync(
        SubmitApprovalRequest request,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets pending approvals for a registrar.
    /// </summary>
    Task<IReadOnlyList<RegistrationApproval>> GetPendingApprovalsAsync(
        Guid registrarId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets an approval request by ID.
    /// </summary>
    Task<RegistrationApproval?> GetApprovalAsync(Guid approvalId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Reviews an approval request.
    /// </summary>
    Task<RegistrationApproval> ReviewApprovalAsync(
        ReviewApprovalRequest request,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Requests additional information for an approval.
    /// </summary>
    Task<RegistrationApproval> RequestAdditionalInfoAsync(
        Guid approvalId,
        Guid registrarId,
        string requestedInfo,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Escalates an approval to the elder council.
    /// </summary>
    Task<RegistrationApproval> EscalateToCouncilAsync(
        Guid approvalId,
        Guid registrarId,
        string reason,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Provides additional documents for an approval.
    /// </summary>
    Task<RegistrationApproval> ProvideDocumentAsync(
        Guid approvalId,
        string documentType,
        string documentId,
        CancellationToken cancellationToken = default);
    
    #endregion
    
    #region Statistics
    
    /// <summary>
    /// Gets approval statistics for a registrar.
    /// </summary>
    Task<RegistrarStatistics> GetStatisticsAsync(
        Guid registrarId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets zone-wide approval statistics.
    /// </summary>
    Task<ZoneStatistics> GetZoneStatisticsAsync(
        string zone,
        CancellationToken cancellationToken = default);
    
    #endregion
}

/// <summary>
/// Request to register a new registrar.
/// </summary>
public class RegisterRegistrarRequest
{
    public Guid IdentityId { get; set; }
    public string DID { get; set; } = null!;
    public RegistrarType Type { get; set; }
    public string Zone { get; set; } = null!;
    public string? ClanName { get; set; }
    public string? InstitutionName { get; set; }
    public string Title { get; set; } = null!;
    public RegistrarScope? Scope { get; set; }
    public Guid? AppointedBy { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

/// <summary>
/// Request to submit identity for approval.
/// </summary>
public class SubmitApprovalRequest
{
    public Guid IdentityId { get; set; }
    public string Zone { get; set; } = null!;
    public string? ClanName { get; set; }
    public ApprovalPriority Priority { get; set; } = ApprovalPriority.Normal;
    public List<string> DocumentTypes { get; set; } = new();
    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// Request to review an approval.
/// </summary>
public class ReviewApprovalRequest
{
    public Guid ApprovalId { get; set; }
    public Guid RegistrarId { get; set; }
    public ApprovalDecision Decision { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Statistics for a registrar.
/// </summary>
public class RegistrarStatistics
{
    public Guid RegistrarId { get; set; }
    public int TotalApprovals { get; set; }
    public int ApprovedCount { get; set; }
    public int RejectedCount { get; set; }
    public int PendingCount { get; set; }
    public int EscalatedCount { get; set; }
    public double AverageProcessingTimeHours { get; set; }
    public DateTime? LastApprovalAt { get; set; }
}

/// <summary>
/// Zone-wide statistics.
/// </summary>
public class ZoneStatistics
{
    public string Zone { get; set; } = null!;
    public int TotalRegistrars { get; set; }
    public int ActiveRegistrars { get; set; }
    public int TotalPendingApprovals { get; set; }
    public int ApprovedToday { get; set; }
    public double AverageProcessingTimeHours { get; set; }
}
