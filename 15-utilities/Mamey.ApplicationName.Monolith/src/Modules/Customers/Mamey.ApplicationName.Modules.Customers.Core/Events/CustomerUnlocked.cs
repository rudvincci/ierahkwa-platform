using System;
using Mamey.CQRS.Events;
using Mamey.MessageBrokers;

namespace Mamey.ApplicationName.Modules.Customers.Core.Events;

internal record CustomerUnlocked(Guid CustomerId) : IEvent;
