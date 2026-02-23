using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.TravelIdentities.Core.Events;

public record TravelIdentityRenewedEvent(
    Guid TravelIdentityId, 
    Guid CitizenId, 
    DateTime NewExpiryDate) : IEvent;
