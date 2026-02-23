using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Repositories;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.ValueObjects;
using Mamey.Government.Modules.TravelIdentities.Core.DTO;
using Mamey.Government.Modules.TravelIdentities.Core.Mappings;

namespace Mamey.Government.Modules.TravelIdentities.Core.Queries.Handlers;

internal sealed class GetTravelIdentityHandler : IQueryHandler<GetTravelIdentity, TravelIdentityDto?>
{
    private readonly ITravelIdentityRepository _repository;

    public GetTravelIdentityHandler(ITravelIdentityRepository repository)
    {
        _repository = repository;
    }

    public async Task<TravelIdentityDto?> HandleAsync(GetTravelIdentity query, CancellationToken cancellationToken = default)
    {
        var travelIdentity = await _repository.GetAsync(new TravelIdentityId(query.TravelIdentityId), cancellationToken);
        return travelIdentity?.AsDto();
    }
}
