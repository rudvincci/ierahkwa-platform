using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Repositories;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.ValueObjects;
using Mamey.Government.Modules.TravelIdentities.Core.DTO;
using Mamey.Government.Modules.TravelIdentities.Core.Mappings;

namespace Mamey.Government.Modules.TravelIdentities.Core.Queries.Handlers;

internal sealed class GetTravelIdentityByNumberHandler : IQueryHandler<GetTravelIdentityByNumber, TravelIdentityDto?>
{
    private readonly ITravelIdentityRepository _repository;

    public GetTravelIdentityByNumberHandler(ITravelIdentityRepository repository)
    {
        _repository = repository;
    }

    public async Task<TravelIdentityDto?> HandleAsync(GetTravelIdentityByNumber query, CancellationToken cancellationToken = default)
    {
        var travelIdentity = await _repository.GetByTravelIdentityNumberAsync(
            new TravelIdentityNumber(query.TravelIdentityNumber), cancellationToken);
        return travelIdentity?.AsDto();
    }
}
