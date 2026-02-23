using Mamey.CQRS.Commands;


namespace Mamey.ApplicationName.Modules.Customers.Core.Commands;

internal record CreateCustomer(string Email) : ICommand;
