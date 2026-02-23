using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Saga.Api.Messages.Citizenship;

internal record KycCompleted(Guid ApplicationId, bool IsApproved, string? RejectionReason = null) : IEvent;
