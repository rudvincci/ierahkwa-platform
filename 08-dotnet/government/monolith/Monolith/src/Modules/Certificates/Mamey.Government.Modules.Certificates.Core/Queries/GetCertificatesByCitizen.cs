using System;
using System.Collections.Generic;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Certificates.Core.DTO;

namespace Mamey.Government.Modules.Certificates.Core.Queries;

internal class GetCertificatesByCitizen : IQuery<IEnumerable<CertificateSummaryDto>>
{
    public Guid CitizenId { get; set; }
}
