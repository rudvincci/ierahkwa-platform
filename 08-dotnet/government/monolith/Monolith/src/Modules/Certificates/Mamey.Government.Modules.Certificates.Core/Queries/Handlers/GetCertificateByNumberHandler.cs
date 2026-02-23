using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Certificates.Core.Domain.Repositories;
using Mamey.Government.Modules.Certificates.Core.DTO;
using Mamey.Government.Modules.Certificates.Core.Mappings;

namespace Mamey.Government.Modules.Certificates.Core.Queries.Handlers;

internal sealed class GetCertificateByNumberHandler : IQueryHandler<GetCertificateByNumber, CertificateDto?>
{
    private readonly ICertificateRepository _repository;

    public GetCertificateByNumberHandler(ICertificateRepository repository)
    {
        _repository = repository;
    }

    public async Task<CertificateDto?> HandleAsync(GetCertificateByNumber query, CancellationToken cancellationToken = default)
    {
        var certificate = await _repository.GetByCertificateNumberAsync(query.CertificateNumber, cancellationToken);
        return certificate?.AsDto();
    }
}
