using System;
using Mamey.CQRS.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Events;

public record ApplicationSubmittedEvent(
    Guid ApplicationId,
    string ApplicationNumber) : IEvent;
