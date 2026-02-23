namespace DigitalVault.Core.Models;

public class ArchiveItem
{
    public Guid Id { get; set; }
    public string ArchiveCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ArchiveType Type { get; set; }
    public ClassificationLevel Classification { get; set; }
    public string? Category { get; set; }
    public string? Department { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public string? StorageUrl { get; set; }
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string? HashSHA256 { get; set; }
    public string? BlockchainHash { get; set; }
    public bool IsEncrypted { get; set; }
    public string? EncryptionAlgorithm { get; set; }
    public DateTime? RetentionUntil { get; set; }
    public RetentionPolicy? RetentionPolicy { get; set; }
    public bool IsLegalHold { get; set; }
    public string? LegalHoldReason { get; set; }
    public Guid? ParentFolderId { get; set; }
    public string? Tags { get; set; }
    public string? Metadata { get; set; }
    public Guid CreatedBy { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; }
    public List<ArchiveAccess> AccessLog { get; set; } = new();
}

public class ArchiveFolder
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentFolderId { get; set; }
    public string Path { get; set; } = string.Empty;
    public ClassificationLevel Classification { get; set; }
    public string? Department { get; set; }
    public Guid OwnerId { get; set; }
    public int ItemCount { get; set; }
    public long TotalSize { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ArchiveAccess
{
    public Guid Id { get; set; }
    public Guid ArchiveItemId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public AccessAction Action { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime AccessedAt { get; set; }
}

public class RetentionPolicy
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int RetentionYears { get; set; }
    public RetentionAction ActionOnExpiry { get; set; }
    public bool RequiresApproval { get; set; }
    public string? ApplicableCategories { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}

public class ArchiveRequest
{
    public Guid Id { get; set; }
    public string RequestNumber { get; set; } = string.Empty;
    public Guid ArchiveItemId { get; set; }
    public string ArchiveItemName { get; set; } = string.Empty;
    public RequestType Type { get; set; }
    public Guid RequestedBy { get; set; }
    public string RequestedByName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public RequestStatus Status { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovalNotes { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public enum ArchiveType { Document, Image, Video, Audio, Email, Record, Legal, Financial, Historical, Other }
public enum ClassificationLevel { Public, Internal, Confidential, Secret, TopSecret }
public enum AccessAction { View, Download, Print, Share, Edit, Delete }
public enum RetentionAction { Archive, Delete, Review, Transfer }
public enum RequestType { Access, Download, Restore, Delete, Declassify }
public enum RequestStatus { Pending, Approved, Rejected, Expired, Completed }
