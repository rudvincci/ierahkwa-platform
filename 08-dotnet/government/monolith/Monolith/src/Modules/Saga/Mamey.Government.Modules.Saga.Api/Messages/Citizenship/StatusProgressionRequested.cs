using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Saga.Api.Messages.Citizenship;

internal record StatusProgressionRequested(Guid CitizenId, string FromStatus, string ToStatus) : IEvent;
