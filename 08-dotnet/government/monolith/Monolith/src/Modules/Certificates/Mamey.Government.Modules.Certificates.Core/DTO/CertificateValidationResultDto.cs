using System;

namespace Mamey.Government.Modules.Certificates.Core.DTO;

public class CertificateValidationResultDto
{
    public bool IsValid { get; set; }
    public string CertificateNumber { get; set; } = string.Empty;
    public string CertificateType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? IssueDate { get; set; }
    public DateTime VerifiedAt { get; set; }
    public string? Message { get; set; }
}
