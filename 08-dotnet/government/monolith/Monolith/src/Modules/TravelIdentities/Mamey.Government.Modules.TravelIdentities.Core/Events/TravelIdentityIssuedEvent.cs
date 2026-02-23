using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.TravelIdentities.Core.Events;

/// <summary>
/// Integration event published when a travel identity is issued.
/// Other modules can subscribe to this event.
/// </summary>
public record TravelIdentityIssuedEvent(
    Guid TravelIdentityId, 
    Guid CitizenId, 
    string TravelIdentityNumber,
    Guid TenantId = default,
    Guid? ApplicationId = null) : IEvent;
