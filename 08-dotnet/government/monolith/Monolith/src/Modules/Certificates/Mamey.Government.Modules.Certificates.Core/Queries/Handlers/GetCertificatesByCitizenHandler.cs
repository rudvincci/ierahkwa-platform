using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Certificates.Core.Domain.Repositories;
using Mamey.Government.Modules.Certificates.Core.DTO;
using Mamey.Government.Modules.Certificates.Core.Mappings;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;

namespace Mamey.Government.Modules.Certificates.Core.Queries.Handlers;

internal sealed class GetCertificatesByCitizenHandler : IQueryHandler<GetCertificatesByCitizen, IEnumerable<CertificateSummaryDto>>
{
    private readonly ICertificateRepository _repository;

    public GetCertificatesByCitizenHandler(ICertificateRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CertificateSummaryDto>> HandleAsync(GetCertificatesByCitizen query, CancellationToken cancellationToken = default)
    {
        var certificates = await _repository.GetByCitizenAsync(new CitizenId(query.CitizenId), cancellationToken);
        return certificates.Select(c => c.AsSummaryDto());
    }
}
