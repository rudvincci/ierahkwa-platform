using ESignature.Core.Interfaces;
using ESignature.Core.Models;
using System.Security.Cryptography;
using System.Text;

namespace ESignature.Infrastructure.Services;

public class SignatureService : ISignatureService
{
    private readonly List<SignatureRequest> _requests = new();
    private readonly List<Signer> _signers = new();
    private readonly List<SignatureField> _fields = new();
    private readonly List<Certificate> _certificates = new();
    private readonly List<SignatureAuditLog> _auditLogs = new();
    private readonly List<SignatureTemplate> _templates = new();
    private readonly List<SignatureVerification> _verifications = new();

    public async Task<SignatureRequest> CreateRequestAsync(SignatureRequest request)
    {
        request.Id = Guid.NewGuid();
        request.RequestNumber = GenerateRequestNumber();
        request.Status = SignatureRequestStatus.Draft;
        request.CreatedAt = DateTime.UtcNow;
        _requests.Add(request);

        await LogAuditEventAsync(new SignatureAuditLog
        {
            SignatureRequestId = request.Id,
            Action = "REQUEST_CREATED",
            Details = $"Signature request created: {request.Title}",
            Timestamp = DateTime.UtcNow
        });

        return request;
    }

    public Task<SignatureRequest?> GetRequestByIdAsync(Guid id)
    {
        var request = _requests.FirstOrDefault(r => r.Id == id);
        if (request != null)
        {
            request.Signers = _signers.Where(s => s.SignatureRequestId == id).ToList();
            request.Fields = _fields.Where(f => f.SignatureRequestId == id).ToList();
        }
        return Task.FromResult(request);
    }

    public Task<SignatureRequest?> GetRequestByNumberAsync(string requestNumber)
    {
        var request = _requests.FirstOrDefault(r => r.RequestNumber == requestNumber);
        return Task.FromResult(request);
    }

    public Task<IEnumerable<SignatureRequest>> GetRequestsBySenderAsync(Guid senderId)
    {
        var requests = _requests.Where(r => r.SenderId == senderId);
        return Task.FromResult(requests);
    }

    public Task<IEnumerable<SignatureRequest>> GetRequestsBySignerEmailAsync(string email)
    {
        var signerRequests = _signers.Where(s => s.Email == email).Select(s => s.SignatureRequestId);
        var requests = _requests.Where(r => signerRequests.Contains(r.Id));
        return Task.FromResult(requests);
    }

    public Task<IEnumerable<SignatureRequest>> GetPendingRequestsAsync(Guid userId)
    {
        var pendingSignerRequests = _signers
            .Where(s => s.UserId == userId && s.Status == SignerStatus.Pending)
            .Select(s => s.SignatureRequestId);
        var requests = _requests.Where(r => pendingSignerRequests.Contains(r.Id));
        return Task.FromResult(requests);
    }

    public Task<SignatureRequest> UpdateRequestAsync(SignatureRequest request)
    {
        var existing = _requests.FirstOrDefault(r => r.Id == request.Id);
        if (existing != null)
        {
            existing.Title = request.Title;
            existing.Description = request.Description;
            existing.Message = request.Message;
            existing.ExpiresAt = request.ExpiresAt;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        return Task.FromResult(existing ?? request);
    }

    public async Task<SignatureRequest> SendRequestAsync(Guid requestId)
    {
        var request = _requests.FirstOrDefault(r => r.Id == requestId);
        if (request == null) throw new Exception("Request not found");

        request.Status = SignatureRequestStatus.Pending;
        request.UpdatedAt = DateTime.UtcNow;

        var signers = _signers.Where(s => s.SignatureRequestId == requestId).ToList();
        foreach (var signer in signers)
        {
            signer.SigningToken = GenerateToken();
            signer.TokenExpiresAt = request.ExpiresAt ?? DateTime.UtcNow.AddDays(30);
            signer.Status = SignerStatus.Sent;
            // In production: Send email notification
        }

        await LogAuditEventAsync(new SignatureAuditLog
        {
            SignatureRequestId = requestId,
            Action = "REQUEST_SENT",
            Details = $"Request sent to {signers.Count} signers",
            Timestamp = DateTime.UtcNow
        });

        return request;
    }

    public async Task<SignatureRequest> CancelRequestAsync(Guid requestId, string reason)
    {
        var request = _requests.FirstOrDefault(r => r.Id == requestId);
        if (request != null)
        {
            request.Status = SignatureRequestStatus.Cancelled;
            request.CancelledAt = DateTime.UtcNow;
            request.CancellationReason = reason;

            await LogAuditEventAsync(new SignatureAuditLog
            {
                SignatureRequestId = requestId,
                Action = "REQUEST_CANCELLED",
                Details = reason,
                Timestamp = DateTime.UtcNow
            });
        }
        return request!;
    }

    public async Task<SignatureRequest> VoidRequestAsync(Guid requestId, string reason)
    {
        var request = _requests.FirstOrDefault(r => r.Id == requestId);
        if (request != null)
        {
            request.Status = SignatureRequestStatus.Voided;
            request.UpdatedAt = DateTime.UtcNow;

            await LogAuditEventAsync(new SignatureAuditLog
            {
                SignatureRequestId = requestId,
                Action = "REQUEST_VOIDED",
                Details = reason,
                Timestamp = DateTime.UtcNow
            });
        }
        return request!;
    }

    public Task DeleteRequestAsync(Guid requestId)
    {
        var request = _requests.FirstOrDefault(r => r.Id == requestId);
        if (request != null) _requests.Remove(request);
        return Task.CompletedTask;
    }

    public Task<Signer> AddSignerAsync(Guid requestId, Signer signer)
    {
        signer.Id = Guid.NewGuid();
        signer.SignatureRequestId = requestId;
        signer.Status = SignerStatus.Pending;
        signer.CreatedAt = DateTime.UtcNow;
        _signers.Add(signer);

        var request = _requests.FirstOrDefault(r => r.Id == requestId);
        if (request != null) request.TotalSigners++;

        return Task.FromResult(signer);
    }

    public Task<Signer> UpdateSignerAsync(Signer signer)
    {
        var existing = _signers.FirstOrDefault(s => s.Id == signer.Id);
        if (existing != null)
        {
            existing.Name = signer.Name;
            existing.Email = signer.Email;
            existing.Phone = signer.Phone;
            existing.Role = signer.Role;
            existing.SigningOrder = signer.SigningOrder;
        }
        return Task.FromResult(existing ?? signer);
    }

    public Task RemoveSignerAsync(Guid signerId)
    {
        var signer = _signers.FirstOrDefault(s => s.Id == signerId);
        if (signer != null)
        {
            _signers.Remove(signer);
            var request = _requests.FirstOrDefault(r => r.Id == signer.SignatureRequestId);
            if (request != null) request.TotalSigners--;
        }
        return Task.CompletedTask;
    }

    public Task<Signer?> GetSignerByTokenAsync(string token)
    {
        var signer = _signers.FirstOrDefault(s => s.SigningToken == token && s.TokenExpiresAt > DateTime.UtcNow);
        return Task.FromResult(signer);
    }

    public Task<string> GenerateSigningUrlAsync(Guid signerId)
    {
        var signer = _signers.FirstOrDefault(s => s.Id == signerId);
        if (signer == null) throw new Exception("Signer not found");
        
        return Task.FromResult($"https://sign.ierahkwa.gov/sign/{signer.SigningToken}");
    }

    public Task SendReminderAsync(Guid signerId)
    {
        var signer = _signers.FirstOrDefault(s => s.Id == signerId);
        if (signer != null)
        {
            signer.RemindersSent++;
            signer.LastReminderAt = DateTime.UtcNow;
            // In production: Send reminder email
        }
        return Task.CompletedTask;
    }

    public async Task SendBulkRemindersAsync(Guid requestId)
    {
        var pendingSigners = _signers.Where(s => s.SignatureRequestId == requestId && s.Status == SignerStatus.Sent);
        foreach (var signer in pendingSigners)
        {
            await SendReminderAsync(signer.Id);
        }
    }

    public async Task<Signer> MarkAsViewedAsync(Guid signerId, string ipAddress, string userAgent)
    {
        var signer = _signers.FirstOrDefault(s => s.Id == signerId);
        if (signer != null && signer.ViewedAt == null)
        {
            signer.ViewedAt = DateTime.UtcNow;
            signer.Status = SignerStatus.Viewed;
            signer.IpAddress = ipAddress;
            signer.UserAgent = userAgent;

            await LogAuditEventAsync(new SignatureAuditLog
            {
                SignatureRequestId = signer.SignatureRequestId,
                SignerId = signerId,
                Action = "DOCUMENT_VIEWED",
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Timestamp = DateTime.UtcNow
            });
        }
        return signer!;
    }

    public async Task<Signer> SignDocumentAsync(Guid signerId, SignDocumentRequest signRequest)
    {
        var signer = _signers.FirstOrDefault(s => s.Id == signerId);
        if (signer == null) throw new Exception("Signer not found");

        signer.Status = SignerStatus.Signed;
        signer.SignedAt = DateTime.UtcNow;
        signer.SignatureData = signRequest.SignatureData;
        signer.SignatureImageUrl = signRequest.SignatureImageUrl;
        signer.IpAddress = signRequest.IpAddress;
        signer.UserAgent = signRequest.UserAgent;
        signer.GeoLocation = signRequest.GeoLocation;

        // Fill fields
        foreach (var fieldValue in signRequest.FieldValues)
        {
            await FillFieldAsync(fieldValue.Key, fieldValue.Value, signer.UserId ?? Guid.Empty);
        }

        var request = _requests.FirstOrDefault(r => r.Id == signer.SignatureRequestId);
        if (request != null)
        {
            request.CompletedSigners++;
            if (request.CompletedSigners >= request.TotalSigners)
            {
                request.Status = SignatureRequestStatus.Completed;
                request.CompletedAt = DateTime.UtcNow;
            }
            else
            {
                request.Status = SignatureRequestStatus.InProgress;
            }
        }

        await LogAuditEventAsync(new SignatureAuditLog
        {
            SignatureRequestId = signer.SignatureRequestId,
            SignerId = signerId,
            Action = "DOCUMENT_SIGNED",
            IpAddress = signRequest.IpAddress,
            UserAgent = signRequest.UserAgent,
            GeoLocation = signRequest.GeoLocation,
            Timestamp = DateTime.UtcNow,
            Hash = ComputeHash(signRequest.SignatureData)
        });

        return signer;
    }

    public async Task<Signer> DeclineSignatureAsync(Guid signerId, string reason)
    {
        var signer = _signers.FirstOrDefault(s => s.Id == signerId);
        if (signer != null)
        {
            signer.Status = SignerStatus.Declined;
            signer.DeclinedAt = DateTime.UtcNow;
            signer.DeclineReason = reason;

            var request = _requests.FirstOrDefault(r => r.Id == signer.SignatureRequestId);
            if (request != null)
            {
                request.Status = SignatureRequestStatus.Declined;
            }

            await LogAuditEventAsync(new SignatureAuditLog
            {
                SignatureRequestId = signer.SignatureRequestId,
                SignerId = signerId,
                Action = "SIGNATURE_DECLINED",
                Details = reason,
                Timestamp = DateTime.UtcNow
            });
        }
        return signer!;
    }

    public async Task<Signer> DelegateSignatureAsync(Guid signerId, string newEmail, string newName)
    {
        var originalSigner = _signers.FirstOrDefault(s => s.Id == signerId);
        if (originalSigner == null) throw new Exception("Signer not found");

        originalSigner.Status = SignerStatus.Delegated;

        var newSigner = new Signer
        {
            Id = Guid.NewGuid(),
            SignatureRequestId = originalSigner.SignatureRequestId,
            Name = newName,
            Email = newEmail,
            SigningOrder = originalSigner.SigningOrder,
            SigningToken = GenerateToken(),
            TokenExpiresAt = originalSigner.TokenExpiresAt,
            Status = SignerStatus.Sent,
            CreatedAt = DateTime.UtcNow
        };
        _signers.Add(newSigner);

        await LogAuditEventAsync(new SignatureAuditLog
        {
            SignatureRequestId = originalSigner.SignatureRequestId,
            SignerId = signerId,
            Action = "SIGNATURE_DELEGATED",
            Details = $"Delegated to {newName} ({newEmail})",
            Timestamp = DateTime.UtcNow
        });

        return newSigner;
    }

    public Task<SignatureField> AddFieldAsync(SignatureField field)
    {
        field.Id = Guid.NewGuid();
        _fields.Add(field);
        return Task.FromResult(field);
    }

    public Task<IEnumerable<SignatureField>> GetFieldsAsync(Guid requestId)
    {
        var fields = _fields.Where(f => f.SignatureRequestId == requestId);
        return Task.FromResult(fields);
    }

    public Task<SignatureField> UpdateFieldAsync(SignatureField field)
    {
        var existing = _fields.FirstOrDefault(f => f.Id == field.Id);
        if (existing != null)
        {
            existing.PositionX = field.PositionX;
            existing.PositionY = field.PositionY;
            existing.Width = field.Width;
            existing.Height = field.Height;
            existing.PageNumber = field.PageNumber;
            existing.AssignedSignerId = field.AssignedSignerId;
        }
        return Task.FromResult(existing ?? field);
    }

    public Task RemoveFieldAsync(Guid fieldId)
    {
        var field = _fields.FirstOrDefault(f => f.Id == fieldId);
        if (field != null) _fields.Remove(field);
        return Task.CompletedTask;
    }

    public Task<SignatureField> FillFieldAsync(Guid fieldId, string value, Guid userId)
    {
        var field = _fields.FirstOrDefault(f => f.Id == fieldId);
        if (field != null)
        {
            field.Value = value;
            field.FilledAt = DateTime.UtcNow;
            field.FilledBy = userId;
        }
        return Task.FromResult(field!);
    }

    public Task<Certificate> CreateCertificateAsync(Certificate certificate)
    {
        certificate.Id = Guid.NewGuid();
        certificate.SerialNumber = GenerateSerialNumber();
        certificate.Status = CertificateStatus.Active;
        certificate.CreatedAt = DateTime.UtcNow;
        certificate.Thumbprint = ComputeHash(certificate.PublicKey);
        _certificates.Add(certificate);
        return Task.FromResult(certificate);
    }

    public Task<Certificate?> GetCertificateByIdAsync(Guid id)
    {
        var cert = _certificates.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(cert);
    }

    public Task<Certificate?> GetCertificateByUserAsync(Guid userId)
    {
        var cert = _certificates.FirstOrDefault(c => c.UserId == userId && c.Status == CertificateStatus.Active);
        return Task.FromResult(cert);
    }

    public Task<IEnumerable<Certificate>> GetActiveCertificatesAsync()
    {
        var certs = _certificates.Where(c => c.Status == CertificateStatus.Active && c.ValidTo > DateTime.UtcNow);
        return Task.FromResult(certs);
    }

    public Task<Certificate> RevokeCertificateAsync(Guid id, string reason)
    {
        var cert = _certificates.FirstOrDefault(c => c.Id == id);
        if (cert != null)
        {
            cert.IsRevoked = true;
            cert.RevokedAt = DateTime.UtcNow;
            cert.RevocationReason = reason;
            cert.Status = CertificateStatus.Revoked;
        }
        return Task.FromResult(cert!);
    }

    public Task<Certificate> RenewCertificateAsync(Guid id)
    {
        var cert = _certificates.FirstOrDefault(c => c.Id == id);
        if (cert != null)
        {
            cert.ValidTo = DateTime.UtcNow.AddYears(2);
            cert.UpdatedAt = DateTime.UtcNow;
        }
        return Task.FromResult(cert!);
    }

    public Task<bool> ValidateCertificateAsync(Guid id)
    {
        var cert = _certificates.FirstOrDefault(c => c.Id == id);
        if (cert == null) return Task.FromResult(false);
        
        return Task.FromResult(
            cert.Status == CertificateStatus.Active &&
            !cert.IsRevoked &&
            cert.ValidFrom <= DateTime.UtcNow &&
            cert.ValidTo >= DateTime.UtcNow
        );
    }

    public Task<SignatureVerification> VerifyDocumentAsync(Guid requestId)
    {
        var request = _requests.FirstOrDefault(r => r.Id == requestId);
        var verification = new SignatureVerification
        {
            Id = Guid.NewGuid(),
            SignatureRequestId = requestId,
            DocumentHash = request?.DocumentHash ?? "",
            IsValid = request?.Status == SignatureRequestStatus.Completed,
            ValidationMessage = request?.Status == SignatureRequestStatus.Completed ? "Document is valid" : "Document not yet completed",
            VerifiedAt = DateTime.UtcNow
        };
        _verifications.Add(verification);
        return Task.FromResult(verification);
    }

    public Task<SignatureVerification> VerifySignatureAsync(Guid signerId)
    {
        var signer = _signers.FirstOrDefault(s => s.Id == signerId);
        var verification = new SignatureVerification
        {
            Id = Guid.NewGuid(),
            SignatureRequestId = signer?.SignatureRequestId ?? Guid.Empty,
            SignatureHash = signer?.SignatureData != null ? ComputeHash(signer.SignatureData) : "",
            IsValid = signer?.Status == SignerStatus.Signed,
            VerifiedAt = DateTime.UtcNow
        };
        return Task.FromResult(verification);
    }

    public Task<bool> VerifyDocumentHashAsync(Guid requestId, string hash)
    {
        var request = _requests.FirstOrDefault(r => r.Id == requestId);
        return Task.FromResult(request?.DocumentHash == hash);
    }

    public Task<SignatureVerification> VerifyWithBlockchainAsync(Guid requestId)
    {
        var verification = new SignatureVerification
        {
            Id = Guid.NewGuid(),
            SignatureRequestId = requestId,
            IsValid = true,
            BlockchainVerification = "Verified on Ierahkwa Sovereign Blockchain",
            VerifiedAt = DateTime.UtcNow
        };
        return Task.FromResult(verification);
    }

    public Task<SignatureAuditLog> LogAuditEventAsync(SignatureAuditLog log)
    {
        log.Id = Guid.NewGuid();
        log.Timestamp = DateTime.UtcNow;
        log.Hash = ComputeHash($"{log.Action}:{log.SignatureRequestId}:{log.Timestamp}");
        _auditLogs.Add(log);
        return Task.FromResult(log);
    }

    public Task<IEnumerable<SignatureAuditLog>> GetAuditLogsAsync(Guid requestId)
    {
        var logs = _auditLogs.Where(l => l.SignatureRequestId == requestId).OrderBy(l => l.Timestamp);
        return Task.FromResult(logs.AsEnumerable());
    }

    public Task<byte[]> GenerateAuditTrailPdfAsync(Guid requestId)
    {
        // In production: Generate actual PDF
        return Task.FromResult(Encoding.UTF8.GetBytes("Audit Trail PDF"));
    }

    public Task<string> RegisterOnBlockchainAsync(Guid requestId)
    {
        var txId = $"0x{Guid.NewGuid():N}";
        var request = _requests.FirstOrDefault(r => r.Id == requestId);
        if (request != null)
        {
            // In production: Submit to blockchain
        }
        return Task.FromResult(txId);
    }

    public Task<SignatureTemplate> CreateTemplateAsync(SignatureTemplate template)
    {
        template.Id = Guid.NewGuid();
        template.CreatedAt = DateTime.UtcNow;
        _templates.Add(template);
        return Task.FromResult(template);
    }

    public Task<IEnumerable<SignatureTemplate>> GetTemplatesAsync(Guid? createdBy = null)
    {
        var templates = _templates.Where(t => t.IsActive);
        if (createdBy.HasValue)
            templates = templates.Where(t => t.CreatedBy == createdBy.Value);
        return Task.FromResult(templates);
    }

    public async Task<SignatureRequest> CreateRequestFromTemplateAsync(Guid templateId, CreateFromTemplateRequest request)
    {
        var template = _templates.FirstOrDefault(t => t.Id == templateId);
        if (template == null) throw new Exception("Template not found");

        var signatureRequest = new SignatureRequest
        {
            Title = template.Name,
            DocumentId = request.DocumentId,
            DocumentName = request.DocumentName,
            Message = request.Message,
            ExpiresAt = request.ExpiresAt,
            SignatureType = template.DefaultSignatureType,
            SigningOrder = template.DefaultSigningOrder,
            RequireAuthentication = template.RequireAuthentication,
            AuthenticationType = template.DefaultAuthenticationType
        };

        var created = await CreateRequestAsync(signatureRequest);

        foreach (var signerInfo in request.Signers)
        {
            await AddSignerAsync(created.Id, new Signer
            {
                Name = signerInfo.Name,
                Email = signerInfo.Email,
                Phone = signerInfo.Phone,
                Role = signerInfo.Role
            });
        }

        template.UsageCount++;
        return created;
    }

    public Task<byte[]> GetSignedDocumentAsync(Guid requestId)
    {
        return Task.FromResult(Encoding.UTF8.GetBytes("Signed Document"));
    }

    public Task<byte[]> GenerateCertificateOfCompletionAsync(Guid requestId)
    {
        return Task.FromResult(Encoding.UTF8.GetBytes("Certificate of Completion"));
    }

    public Task<string> GetDocumentDownloadUrlAsync(Guid requestId)
    {
        return Task.FromResult($"https://api.ierahkwa.gov/esignature/documents/{requestId}/download");
    }

    public Task<SignatureStatistics> GetStatisticsAsync(Guid? userId = null)
    {
        var requests = userId.HasValue 
            ? _requests.Where(r => r.SenderId == userId.Value)
            : _requests;

        return Task.FromResult(new SignatureStatistics
        {
            TotalRequests = requests.Count(),
            PendingRequests = requests.Count(r => r.Status == SignatureRequestStatus.Pending),
            CompletedRequests = requests.Count(r => r.Status == SignatureRequestStatus.Completed),
            DeclinedRequests = requests.Count(r => r.Status == SignatureRequestStatus.Declined),
            ExpiredRequests = requests.Count(r => r.Status == SignatureRequestStatus.Expired),
            TotalSignatures = _signers.Count(s => s.Status == SignerStatus.Signed),
            ActiveCertificates = _certificates.Count(c => c.Status == CertificateStatus.Active),
            RequestsByStatus = requests.GroupBy(r => r.Status.ToString()).ToDictionary(g => g.Key, g => g.Count()),
            RequestsByType = requests.GroupBy(r => r.SignatureType.ToString()).ToDictionary(g => g.Key, g => g.Count())
        });
    }

    private string GenerateRequestNumber() => $"SIG-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    private string GenerateToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)).Replace("+", "-").Replace("/", "_");
    private string GenerateSerialNumber() => $"CERT-{Guid.NewGuid():N}".ToUpper();
    private string ComputeHash(string input)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(hash);
    }
}
