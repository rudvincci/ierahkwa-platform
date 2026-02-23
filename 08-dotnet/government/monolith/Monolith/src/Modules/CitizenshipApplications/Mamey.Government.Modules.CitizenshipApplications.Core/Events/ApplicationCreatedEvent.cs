using System;
using Mamey.CQRS.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Events;


public record ApplicationCreatedEvent(
    Guid ApplicationId,
    Guid TenantId,
    string ApplicationNumber) : IEvent;
