using System;
using System.Collections.Generic;

namespace Mamey.FWID.Sagas.Core.Data;

/// <summary>
/// Saga data structure for tracking Identity Registration process.
/// Complete workflow: StartRegistration → CaptureBiometrics → SubmitForClanApproval → WaitForApproval → IssueDID → GrantAccess → GenerateCredentials → Complete
/// </summary>
internal class IdentityRegistrationSagaData
{
    #region Identity Information
    
    public Guid IdentityId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Zone { get; set; }
    public string? Clan { get; set; }
    
    #endregion
    
    #region Workflow Status
    
    public RegistrationStep CurrentStep { get; set; } = RegistrationStep.Started;
    public string Status { get; set; } = "Pending";
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
    public const int MaxRetries = 3;
    
    #endregion
    
    #region Step Completion Flags
    
    public bool IdentityCreated { get; set; }
    public bool BiometricsCaptured { get; set; }
    public bool BiometricsVerified { get; set; }
    public bool ClanApprovalSubmitted { get; set; }
    public bool ClanApprovalReceived { get; set; }
    public bool DIDCreated { get; set; }
    public bool ZoneAccessGranted { get; set; }
    public bool CredentialIssued { get; set; }
    
    #endregion
    
    #region Generated IDs
    
    public Guid? DIDId { get; set; }
    public string? DIDString { get; set; }
    public Guid? AccessControlId { get; set; }
    public Guid? CredentialId { get; set; }
    public Guid? BiometricTemplateId { get; set; }
    public Guid? ClanApprovalId { get; set; }
    public Guid? AssignedRegistrarId { get; set; }
    
    #endregion
    
    #region Biometric Data
    
    public bool HasFingerprint { get; set; }
    public bool HasFaceRecognition { get; set; }
    public double? BiometricQualityScore { get; set; }
    
    #endregion
    
    #region Clan Approval
    
    public ClanApprovalStatus ClanApprovalStatus { get; set; } = ClanApprovalStatus.NotSubmitted;
    public string? ClanApprovalNotes { get; set; }
    public DateTime? ClanApprovalSubmittedAt { get; set; }
    public DateTime? ClanApprovalDecidedAt { get; set; }
    public DateTime? ClanApprovalTimeout { get; set; }
    
    #endregion
    
    #region Timestamps
    
    public DateTime StartedAt { get; set; }
    public DateTime? BiometricsCapturedAt { get; set; }
    public DateTime? ClanApprovedAt { get; set; }
    public DateTime? DIDCreatedAt { get; set; }
    public DateTime? CredentialIssuedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    #endregion
    
    #region Audit Trail
    
    public List<SagaStepAudit> AuditTrail { get; set; } = new();
    
    #endregion
    
    #region Helper Methods
    
    public void AddAuditEntry(string action, string? details = null)
    {
        AuditTrail.Add(new SagaStepAudit
        {
            Timestamp = DateTime.UtcNow,
            Step = CurrentStep,
            Action = action,
            Details = details
        });
    }
    
    public bool CanProceed => RetryCount < MaxRetries && Status != "Failed" && Status != "Cancelled";
    
    public TimeSpan ElapsedTime => 
        (CompletedAt ?? DateTime.UtcNow) - StartedAt;
    
    #endregion
}

/// <summary>
/// Steps in the identity registration workflow.
/// </summary>
public enum RegistrationStep
{
    Started = 0,
    IdentityCreated = 1,
    BiometricCapture = 2,
    BiometricVerification = 3,
    ClanApprovalPending = 4,
    ClanApproved = 5,
    DIDIssuance = 6,
    ZoneAccessGrant = 7,
    CredentialGeneration = 8,
    Completed = 9,
    Failed = 10,
    Cancelled = 11,
    CompensationInProgress = 12
}

/// <summary>
/// Status of clan approval.
/// </summary>
public enum ClanApprovalStatus
{
    NotSubmitted = 0,
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Escalated = 4,
    TimedOut = 5
}

/// <summary>
/// Audit entry for saga steps.
/// </summary>
public class SagaStepAudit
{
    public DateTime Timestamp { get; set; }
    public RegistrationStep Step { get; set; }
    public string Action { get; set; } = null!;
    public string? Details { get; set; }
    public string? ActorId { get; set; }
}



