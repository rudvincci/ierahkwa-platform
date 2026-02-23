using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Saga.Api.Messages.Citizenship;


public record ApplicationApprovedEvent(
    Guid ApplicationId,
    string ApplicationNumber,
    string ApprovedBy,
    DateTime ApprovedAt) : IEvent;
