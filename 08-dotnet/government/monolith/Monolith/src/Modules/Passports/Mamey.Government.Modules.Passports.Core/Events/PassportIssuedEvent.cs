using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Passports.Core.Events;

/// <summary>
/// Integration event published when a passport is issued.
/// Other modules can subscribe to this event.
/// </summary>
public record PassportIssuedEvent(
    Guid PassportId, 
    Guid CitizenId, 
    string PassportNumber,
    Guid TenantId = default,
    Guid? ApplicationId = null) : IEvent;
