using Mamey.FWID.Identities.Application.AML.Models;

namespace Mamey.FWID.Identities.Application.AML.Services;

/// <summary>
/// Interface for Suspicious Activity Report (SAR) service.
/// </summary>
public interface ISARService
{
    /// <summary>
    /// Creates a new SAR.
    /// </summary>
    Task<SuspiciousActivityReport> CreateSARAsync(
        CreateSARRequest request,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a SAR by ID.
    /// </summary>
    Task<SuspiciousActivityReport?> GetSARAsync(
        Guid sarId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets SAR by reference number.
    /// </summary>
    Task<SuspiciousActivityReport?> GetSARByReferenceAsync(
        string referenceNumber,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates SAR status.
    /// </summary>
    Task<SuspiciousActivityReport> UpdateStatusAsync(
        Guid sarId,
        SARStatus newStatus,
        Guid performedBy,
        string? comments = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Assigns SAR to a reviewer.
    /// </summary>
    Task<bool> AssignSARAsync(
        Guid sarId,
        Guid assigneeId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Adds evidence to SAR.
    /// </summary>
    Task<bool> AddEvidenceAsync(
        Guid sarId,
        SAREvidence evidence,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Submits SAR for approval.
    /// </summary>
    Task<SuspiciousActivityReport> SubmitForApprovalAsync(
        Guid sarId,
        Guid submittedBy,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Approves SAR for filing.
    /// </summary>
    Task<SuspiciousActivityReport> ApproveSARAsync(
        Guid sarId,
        Guid approvedBy,
        string? comments = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Files SAR with regulatory body.
    /// </summary>
    Task<SARFilingResult> FileSARAsync(
        Guid sarId,
        Guid filedBy,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets pending SARs.
    /// </summary>
    Task<IReadOnlyList<SuspiciousActivityReport>> GetPendingSARsAsync(
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets SARs approaching deadline.
    /// </summary>
    Task<IReadOnlyList<SuspiciousActivityReport>> GetSARsApproachingDeadlineAsync(
        int daysUntilDeadline = 3,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets SARs for an identity.
    /// </summary>
    Task<IReadOnlyList<SuspiciousActivityReport>> GetSARsForIdentityAsync(
        Guid identityId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generates SAR report document.
    /// </summary>
    Task<byte[]> GenerateSARReportAsync(
        Guid sarId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Request to create a SAR.
/// </summary>
public class CreateSARRequest
{
    public Guid SubjectIdentityId { get; set; }
    public string SubjectName { get; set; } = null!;
    public string? SubjectDID { get; set; }
    public SARTrigger Trigger { get; set; }
    public SARType Type { get; set; }
    public SARPriority Priority { get; set; }
    public string Narrative { get; set; } = null!;
    public DateTime ActivityDetectedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public List<SAREvidence>? InitialEvidence { get; set; }
    public List<SARRelatedTransaction>? RelatedTransactions { get; set; }
}

/// <summary>
/// Result of SAR filing.
/// </summary>
public class SARFilingResult
{
    public Guid SARId { get; set; }
    public bool Success { get; set; }
    public string? ConfirmationNumber { get; set; }
    public string? RegulatoryBody { get; set; }
    public DateTime? FiledAt { get; set; }
    public string? ErrorMessage { get; set; }
}
