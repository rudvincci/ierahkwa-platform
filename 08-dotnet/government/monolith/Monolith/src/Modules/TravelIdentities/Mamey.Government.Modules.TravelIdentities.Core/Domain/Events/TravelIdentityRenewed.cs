using Mamey.CQRS;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.ValueObjects;

namespace Mamey.Government.Modules.TravelIdentities.Core.Domain.Events;

public record TravelIdentityRenewed(
    TravelIdentityId TravelIdentityId,
    DateTime NewExpiryDate) : IDomainEvent;
