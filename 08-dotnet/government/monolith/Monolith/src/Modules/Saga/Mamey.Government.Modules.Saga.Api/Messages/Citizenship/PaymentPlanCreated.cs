using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Saga.Api.Messages.Citizenship;

internal record PaymentPlanCreated(Guid PaymentPlanId, Guid CitizenId, Guid ApplicationId) : IEvent;
