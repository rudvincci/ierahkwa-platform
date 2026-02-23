using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Saga.Api.Messages.Citizenship;

public record ApplicationSubmittedEvent(
    Guid ApplicationId,
    string ApplicationNumber) : IEvent;
