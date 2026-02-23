using Mamey.CQRS.Queries;
using Mamey.Government.Modules.TravelIdentities.Core.DTO;

namespace Mamey.Government.Modules.TravelIdentities.Core.Queries;

internal class GetTravelIdentityByNumber : IQuery<TravelIdentityDto?>
{
    public string TravelIdentityNumber { get; set; } = string.Empty;
}
