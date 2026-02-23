using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Passports.Core.Events;

/// <summary>
/// Integration event published when a passport is revoked.
/// </summary>
public record PassportRevokedEvent(
    Guid PassportId, 
    Guid CitizenId, 
    string Reason) : IEvent;
