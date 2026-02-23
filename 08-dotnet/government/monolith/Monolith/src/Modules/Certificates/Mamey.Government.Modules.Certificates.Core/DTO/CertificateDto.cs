using System;

namespace Mamey.Government.Modules.Certificates.Core.DTO;

public class CertificateDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid? CitizenId { get; set; }
    public string? CitizenName { get; set; }
    public string CertificateType { get; set; } = string.Empty;
    public string CertificateNumber { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public string? DocumentPath { get; set; }
    public bool IsActive { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevocationReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Status => IsActive ? "Active" : "Revoked";
}
