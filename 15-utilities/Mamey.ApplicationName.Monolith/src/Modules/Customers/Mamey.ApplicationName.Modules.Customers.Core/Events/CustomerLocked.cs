using System;
using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Customers.Core.Events;

internal record CustomerLocked(Guid CustomerId) : IEvent;
