using System;
using Mamey.CQRS.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;

namespace Mamey.Government.Modules.Citizens.Core.Events;

[Message("citizens")]
public record CitizenCreatedEvent(
    Guid CitizenId,
    Guid TenantId,
    string FirstName,
    string LastName,
    string Status,
    Guid? ApplicationId = null) : IEvent;
