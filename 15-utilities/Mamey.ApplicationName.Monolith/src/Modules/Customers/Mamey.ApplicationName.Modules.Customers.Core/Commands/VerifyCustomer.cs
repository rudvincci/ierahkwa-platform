using System;
using Mamey.CQRS.Commands;

namespace Mamey.ApplicationName.Modules.Customers.Core.Commands;

internal record VerifyCustomer(Guid CustomerId) : ICommand;