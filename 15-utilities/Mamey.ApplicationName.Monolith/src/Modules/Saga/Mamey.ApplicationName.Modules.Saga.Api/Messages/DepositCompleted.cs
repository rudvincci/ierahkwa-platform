using System;
using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Saga.Api.Messages;

internal record DepositCompleted(Guid DepositId, Guid CustomerId, string Currency, decimal Amount) : IEvent;
