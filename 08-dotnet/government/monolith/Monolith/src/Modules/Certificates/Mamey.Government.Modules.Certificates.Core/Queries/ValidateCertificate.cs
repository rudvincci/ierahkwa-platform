using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Certificates.Core.DTO;

namespace Mamey.Government.Modules.Certificates.Core.Queries;

/// <summary>
/// Public validation query for certificate verification.
/// </summary>
public class ValidateCertificate : IQuery<CertificateValidationResultDto?>
{
    public string CertificateNumber { get; set; } = string.Empty;
}
