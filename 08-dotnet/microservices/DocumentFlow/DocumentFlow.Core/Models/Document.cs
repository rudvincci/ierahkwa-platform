namespace DocumentFlow.Core.Models;

public class Document
{
    public Guid Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DocumentType Type { get; set; }
    public DocumentStatus Status { get; set; }
    public string? Category { get; set; }
    public string? Department { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public string? StorageUrl { get; set; }
    public string? HashSHA256 { get; set; }
    public string? BlockchainHash { get; set; }
    public int Version { get; set; } = 1;
    public Guid? ParentDocumentId { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public Guid? OwnerId { get; set; }
    public string? OwnerName { get; set; }
    public bool IsTemplate { get; set; }
    public bool IsArchived { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public DateTime? RetentionDate { get; set; }
    public SecurityLevel SecurityLevel { get; set; }
    public string? Tags { get; set; }
    public string? Metadata { get; set; }
    public string? OcrContent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public List<DocumentVersion> Versions { get; set; } = new();
    public List<DocumentComment> Comments { get; set; } = new();
    public List<DocumentPermission> Permissions { get; set; } = new();
    public List<DocumentWorkflow> Workflows { get; set; } = new();
}

public enum DocumentType
{
    General,
    Official,
    Legal,
    Financial,
    Contract,
    Report,
    Memo,
    Letter,
    Resolution,
    Decree,
    Policy,
    Procedure,
    Form,
    Invoice,
    Receipt,
    Certificate,
    License,
    Permit,
    Application,
    Other
}

public enum DocumentStatus
{
    Draft,
    PendingReview,
    UnderReview,
    Approved,
    Rejected,
    Published,
    Archived,
    Expired,
    Revoked
}

public enum SecurityLevel
{
    Public,
    Internal,
    Confidential,
    Secret,
    TopSecret
}
