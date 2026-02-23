using System;
using Mamey.CQRS.Commands;
using Mamey.Types;

namespace Mamey.ApplicationName.Modules.Customers.Core.Commands;

internal record CompleteCustomer(Guid CustomerId, Name Name, string FullName, Address Address, string Nationality,
    string IdentityType, string IdentitySeries) : ICommand;
