using ESignature.Core.Models;

namespace ESignature.Core.Interfaces;

public interface ISignatureService
{
    // Signature Request Operations
    Task<SignatureRequest> CreateRequestAsync(SignatureRequest request);
    Task<SignatureRequest?> GetRequestByIdAsync(Guid id);
    Task<SignatureRequest?> GetRequestByNumberAsync(string requestNumber);
    Task<IEnumerable<SignatureRequest>> GetRequestsBySenderAsync(Guid senderId);
    Task<IEnumerable<SignatureRequest>> GetRequestsBySignerEmailAsync(string email);
    Task<IEnumerable<SignatureRequest>> GetPendingRequestsAsync(Guid userId);
    Task<SignatureRequest> UpdateRequestAsync(SignatureRequest request);
    Task<SignatureRequest> SendRequestAsync(Guid requestId);
    Task<SignatureRequest> CancelRequestAsync(Guid requestId, string reason);
    Task<SignatureRequest> VoidRequestAsync(Guid requestId, string reason);
    Task DeleteRequestAsync(Guid requestId);

    // Signer Operations
    Task<Signer> AddSignerAsync(Guid requestId, Signer signer);
    Task<Signer> UpdateSignerAsync(Signer signer);
    Task RemoveSignerAsync(Guid signerId);
    Task<Signer?> GetSignerByTokenAsync(string token);
    Task<string> GenerateSigningUrlAsync(Guid signerId);
    Task SendReminderAsync(Guid signerId);
    Task SendBulkRemindersAsync(Guid requestId);

    // Signing Operations
    Task<Signer> MarkAsViewedAsync(Guid signerId, string ipAddress, string userAgent);
    Task<Signer> SignDocumentAsync(Guid signerId, SignDocumentRequest signRequest);
    Task<Signer> DeclineSignatureAsync(Guid signerId, string reason);
    Task<Signer> DelegateSignatureAsync(Guid signerId, string newEmail, string newName);

    // Field Operations
    Task<SignatureField> AddFieldAsync(SignatureField field);
    Task<IEnumerable<SignatureField>> GetFieldsAsync(Guid requestId);
    Task<SignatureField> UpdateFieldAsync(SignatureField field);
    Task RemoveFieldAsync(Guid fieldId);
    Task<SignatureField> FillFieldAsync(Guid fieldId, string value, Guid userId);

    // Certificate Operations
    Task<Certificate> CreateCertificateAsync(Certificate certificate);
    Task<Certificate?> GetCertificateByIdAsync(Guid id);
    Task<Certificate?> GetCertificateByUserAsync(Guid userId);
    Task<IEnumerable<Certificate>> GetActiveCertificatesAsync();
    Task<Certificate> RevokeCertificateAsync(Guid id, string reason);
    Task<Certificate> RenewCertificateAsync(Guid id);
    Task<bool> ValidateCertificateAsync(Guid id);

    // Verification Operations
    Task<SignatureVerification> VerifyDocumentAsync(Guid requestId);
    Task<SignatureVerification> VerifySignatureAsync(Guid signerId);
    Task<bool> VerifyDocumentHashAsync(Guid requestId, string hash);
    Task<SignatureVerification> VerifyWithBlockchainAsync(Guid requestId);

    // Audit Operations
    Task<SignatureAuditLog> LogAuditEventAsync(SignatureAuditLog log);
    Task<IEnumerable<SignatureAuditLog>> GetAuditLogsAsync(Guid requestId);
    Task<byte[]> GenerateAuditTrailPdfAsync(Guid requestId);
    Task<string> RegisterOnBlockchainAsync(Guid requestId);

    // Template Operations
    Task<SignatureTemplate> CreateTemplateAsync(SignatureTemplate template);
    Task<IEnumerable<SignatureTemplate>> GetTemplatesAsync(Guid? createdBy = null);
    Task<SignatureRequest> CreateRequestFromTemplateAsync(Guid templateId, CreateFromTemplateRequest request);

    // Document Operations
    Task<byte[]> GetSignedDocumentAsync(Guid requestId);
    Task<byte[]> GenerateCertificateOfCompletionAsync(Guid requestId);
    Task<string> GetDocumentDownloadUrlAsync(Guid requestId);

    // Statistics
    Task<SignatureStatistics> GetStatisticsAsync(Guid? userId = null);
}

public class SignDocumentRequest
{
    public string SignatureData { get; set; } = string.Empty; // Base64 image or biometric data
    public string? SignatureImageUrl { get; set; }
    public Dictionary<Guid, string> FieldValues { get; set; } = new();
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string? GeoLocation { get; set; }
    public string? VerificationCode { get; set; }
    public string? BiometricData { get; set; }
}

public class CreateFromTemplateRequest
{
    public Guid DocumentId { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public List<SignerInfo> Signers { get; set; } = new();
    public string? Message { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class SignerInfo
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Role { get; set; }
}

public class SignatureStatistics
{
    public int TotalRequests { get; set; }
    public int PendingRequests { get; set; }
    public int CompletedRequests { get; set; }
    public int DeclinedRequests { get; set; }
    public int ExpiredRequests { get; set; }
    public double AverageCompletionTimeHours { get; set; }
    public int TotalSignatures { get; set; }
    public int ActiveCertificates { get; set; }
    public Dictionary<string, int> RequestsByStatus { get; set; } = new();
    public Dictionary<string, int> RequestsByType { get; set; } = new();
}
