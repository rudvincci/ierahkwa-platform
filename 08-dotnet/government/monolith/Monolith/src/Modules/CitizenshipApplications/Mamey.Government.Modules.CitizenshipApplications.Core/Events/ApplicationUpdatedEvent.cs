using System;
using Mamey.CQRS.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Events;

public record ApplicationUpdatedEvent(Guid ApplicationId) : IEvent;
