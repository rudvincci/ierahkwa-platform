using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Saga.Api.Messages.Citizenship;

public record ApplicationRejectedEvent(
    Guid ApplicationId,
    string ApplicationNumber,
    string Reason,
    string RejectedBy,
    DateTime RejectedAt) : IEvent;
