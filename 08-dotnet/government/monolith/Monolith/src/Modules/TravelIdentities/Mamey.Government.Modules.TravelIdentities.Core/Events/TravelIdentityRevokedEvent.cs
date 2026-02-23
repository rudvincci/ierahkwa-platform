using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.TravelIdentities.Core.Events;

public record TravelIdentityRevokedEvent(
    Guid TravelIdentityId, 
    Guid CitizenId, 
    string Reason) : IEvent;
