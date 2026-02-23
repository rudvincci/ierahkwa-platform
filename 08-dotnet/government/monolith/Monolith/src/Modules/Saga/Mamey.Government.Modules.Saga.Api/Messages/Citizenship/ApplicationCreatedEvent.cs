using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Saga.Api.Messages.Citizenship;


public record ApplicationCreatedEvent(
    Guid ApplicationId,
    Guid TenantId,
    string ApplicationNumber) : IEvent;
