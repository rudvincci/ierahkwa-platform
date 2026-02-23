using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Certificates.Core.Domain.Repositories;
using Mamey.Government.Modules.Certificates.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Certificates.Core.DTO;
using Mamey.Government.Modules.Certificates.Core.Mappings;

namespace Mamey.Government.Modules.Certificates.Core.Queries.Handlers;

internal sealed class GetCertificateHandler : IQueryHandler<GetCertificate, CertificateDto?>
{
    private readonly ICertificateRepository _repository;

    public GetCertificateHandler(ICertificateRepository repository)
    {
        _repository = repository;
    }

    public async Task<CertificateDto?> HandleAsync(GetCertificate query, CancellationToken cancellationToken = default)
    {
        var certificate = await _repository.GetAsync(new CertificateId(query.CertificateId), cancellationToken);
        return certificate?.AsDto();
    }
}
