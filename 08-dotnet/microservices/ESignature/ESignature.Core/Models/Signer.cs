namespace ESignature.Core.Models;

public class Signer
{
    public Guid Id { get; set; }
    public Guid SignatureRequestId { get; set; }
    public Guid? UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Role { get; set; }
    public int SigningOrder { get; set; }
    public SignerStatus Status { get; set; }
    public string? AccessCode { get; set; }
    public string? SigningToken { get; set; }
    public DateTime? TokenExpiresAt { get; set; }
    public DateTime? ViewedAt { get; set; }
    public DateTime? SignedAt { get; set; }
    public DateTime? DeclinedAt { get; set; }
    public string? DeclineReason { get; set; }
    public string? SignatureImageUrl { get; set; }
    public string? SignatureData { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? GeoLocation { get; set; }
    public bool IdentityVerified { get; set; }
    public string? VerificationMethod { get; set; }
    public string? VerificationData { get; set; }
    public int RemindersSent { get; set; }
    public DateTime? LastReminderAt { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public SignatureRequest? SignatureRequest { get; set; }
    public List<SignatureField> AssignedFields { get; set; } = new();
}

public enum SignerStatus
{
    Pending,
    Sent,
    Viewed,
    Signed,
    Declined,
    Delegated,
    Expired
}
