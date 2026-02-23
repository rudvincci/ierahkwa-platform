using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Repositories;
using Mamey.Government.Modules.TravelIdentities.Core.DTO;
using Mamey.Government.Modules.TravelIdentities.Core.Mappings;

namespace Mamey.Government.Modules.TravelIdentities.Core.Queries.Handlers;

internal sealed class GetTravelIdentitiesByCitizenHandler : IQueryHandler<GetTravelIdentitiesByCitizen, IEnumerable<TravelIdentitySummaryDto>>
{
    private readonly ITravelIdentityRepository _repository;

    public GetTravelIdentitiesByCitizenHandler(ITravelIdentityRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TravelIdentitySummaryDto>> HandleAsync(GetTravelIdentitiesByCitizen query, CancellationToken cancellationToken = default)
    {
        var travelIdentities = await _repository.GetByCitizenAsync(new CitizenId(query.CitizenId), cancellationToken);
        return travelIdentities.Select(t => t.AsSummaryDto());
    }
}
