using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Tenant.Core.Domain.Repositories;
using Mamey.Government.Modules.Tenant.Core.DTO;
using Mamey.Government.Modules.Tenant.Core.Mappings;
using Mamey.Types;

namespace Mamey.Government.Modules.Tenant.Core.Queries.Handlers;

internal sealed class GetTenantHandler : IQueryHandler<GetTenant, TenantDto?>
{
    private readonly ITenantRepository _repository;

    public GetTenantHandler(ITenantRepository repository)
    {
        _repository = repository;
    }

    public async Task<TenantDto?> HandleAsync(GetTenant query, CancellationToken cancellationToken = default)
    {
        var tenant = await _repository.GetAsync(new TenantId(query.TenantId), cancellationToken);
        return tenant?.AsDto();
    }
}
