namespace ESignature.Core.Models;

public class SignatureRequest
{
    public Guid Id { get; set; }
    public string RequestNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public SignatureRequestStatus Status { get; set; }
    public SignatureType SignatureType { get; set; }
    public Guid DocumentId { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public string? DocumentUrl { get; set; }
    public string? DocumentHash { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string? Message { get; set; }
    public SigningOrder SigningOrder { get; set; }
    public int TotalSigners { get; set; }
    public int CompletedSigners { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool RequireAuthentication { get; set; }
    public AuthenticationType AuthenticationType { get; set; }
    public bool SendReminders { get; set; }
    public int ReminderFrequencyDays { get; set; }
    public string? CallbackUrl { get; set; }
    public string? Metadata { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }

    // Navigation
    public List<Signer> Signers { get; set; } = new();
    public List<SignatureField> Fields { get; set; } = new();
    public List<SignatureAuditLog> AuditLogs { get; set; } = new();
}

public enum SignatureRequestStatus
{
    Draft,
    Pending,
    InProgress,
    Completed,
    Declined,
    Cancelled,
    Expired,
    Voided
}

public enum SignatureType
{
    Electronic,
    Digital,
    Advanced,
    Qualified,
    Biometric,
    Blockchain
}

public enum SigningOrder
{
    Parallel,
    Sequential,
    Custom
}

public enum AuthenticationType
{
    None,
    Email,
    SMS,
    TwoFactor,
    Biometric,
    GovernmentId,
    BlockchainWallet
}
