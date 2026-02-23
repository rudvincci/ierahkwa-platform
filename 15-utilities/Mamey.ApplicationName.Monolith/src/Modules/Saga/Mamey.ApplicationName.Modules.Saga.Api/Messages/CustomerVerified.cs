using System;
using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Saga.Api.Messages;

internal record CustomerVerified(Guid CustomerId) : IEvent;
