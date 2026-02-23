using System;

namespace Mamey.Government.Modules.Certificates.Core.DTO;

public class CertificateSummaryDto
{
    public Guid Id { get; set; }
    public string CertificateType { get; set; } = string.Empty;
    public string CertificateNumber { get; set; } = string.Empty;
    public Guid? CitizenId { get; set; }
    public string? CitizenName { get; set; }
    public DateTime IssuedDate { get; set; }
    public bool IsActive { get; set; }
    public string Status => IsActive ? "Active" : "Revoked";
}
