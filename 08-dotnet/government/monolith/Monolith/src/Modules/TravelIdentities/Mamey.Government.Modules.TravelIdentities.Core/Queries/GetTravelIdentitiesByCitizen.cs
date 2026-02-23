using System;
using System.Collections.Generic;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.TravelIdentities.Core.DTO;

namespace Mamey.Government.Modules.TravelIdentities.Core.Queries;

internal class GetTravelIdentitiesByCitizen : IQuery<IEnumerable<TravelIdentitySummaryDto>>
{
    public Guid CitizenId { get; set; }
}
