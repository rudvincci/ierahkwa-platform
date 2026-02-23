using System;
using Mamey.CQRS.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;

namespace Mamey.Government.Modules.Citizens.Core.Events;

[Message("citizens")]
public record CitizenDeactivatedEvent(
    Guid CitizenId,
    string Reason) : IEvent;
