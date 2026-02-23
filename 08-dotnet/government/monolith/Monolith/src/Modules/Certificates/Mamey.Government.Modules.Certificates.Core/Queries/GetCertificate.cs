using System;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Certificates.Core.DTO;

namespace Mamey.Government.Modules.Certificates.Core.Queries;

internal class GetCertificate : IQuery<CertificateDto?>
{
    public Guid CertificateId { get; set; }
}
