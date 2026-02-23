using System;
using Mamey.CQRS.Commands;

namespace Mamey.ApplicationName.Modules.Customers.Core.Commands;

internal record UnlockCustomer(Guid CustomerId, string Notes = null) : ICommand;
