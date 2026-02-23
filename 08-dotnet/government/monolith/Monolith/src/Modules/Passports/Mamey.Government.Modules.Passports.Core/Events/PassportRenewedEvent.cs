using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Passports.Core.Events;

/// <summary>
/// Integration event published when a passport is renewed.
/// </summary>
public record PassportRenewedEvent(
    Guid PassportId, 
    Guid CitizenId, 
    DateTime NewExpiryDate) : IEvent;
