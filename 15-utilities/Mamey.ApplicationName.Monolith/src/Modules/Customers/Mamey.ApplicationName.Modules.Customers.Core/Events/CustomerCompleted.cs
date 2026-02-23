using System;
using Mamey.CQRS.Events;
using Mamey.Types;

namespace Mamey.ApplicationName.Modules.Customers.Core.Events;

internal record CustomerCompleted(Guid CustomerId, Name Name, string FullName, string Nationality) : IEvent;
