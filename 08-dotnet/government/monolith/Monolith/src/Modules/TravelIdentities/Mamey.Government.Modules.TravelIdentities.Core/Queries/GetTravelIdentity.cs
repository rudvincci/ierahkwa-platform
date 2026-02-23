using System;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.TravelIdentities.Core.DTO;

namespace Mamey.Government.Modules.TravelIdentities.Core.Queries;

internal class GetTravelIdentity : IQuery<TravelIdentityDto?>
{
    public Guid TravelIdentityId { get; set; }
}
