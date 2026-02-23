using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Certificates.Core.DTO;

namespace Mamey.Government.Modules.Certificates.Core.Queries;

internal class GetCertificateByNumber : IQuery<CertificateDto?>
{
    public string CertificateNumber { get; set; } = string.Empty;
}
