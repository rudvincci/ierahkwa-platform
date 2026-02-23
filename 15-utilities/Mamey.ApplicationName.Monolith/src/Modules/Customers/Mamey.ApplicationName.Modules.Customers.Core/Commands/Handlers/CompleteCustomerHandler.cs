using System.Threading;
using System.Threading.Tasks;
using Mamey.ApplicationName.Modules.Customers.Core.Domain.Repositories;
using Mamey.ApplicationName.Modules.Customers.Core.Domain.ValueObjects;
using Mamey.ApplicationName.Modules.Customers.Core.Events;
using Mamey.ApplicationName.Modules.Customers.Core.Exceptions;
using Mamey.CQRS.Commands;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Mamey.MicroMonolith.Abstractions.Time;
using Microsoft.Extensions.Logging;

namespace Mamey.ApplicationName.Modules.Customers.Core.Commands.Handlers;

internal sealed class CompleteCustomerHandler : ICommandHandler<CompleteCustomer>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMessageBroker _messageBroker;
    private readonly IClock _clock;
    private readonly ILogger<CompleteCustomerHandler> _logger;

    public CompleteCustomerHandler(ICustomerRepository customerRepository, IMessageBroker messageBroker,
        IClock clock, ILogger<CompleteCustomerHandler> logger)
    {
        _customerRepository = customerRepository;
        _messageBroker = messageBroker;
        _clock = clock;
        _logger = logger;
    }

    public async Task HandleAsync(CompleteCustomer command, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetAsync(command.CustomerId);
        if (customer is null)
        {
            throw new CustomerNotFoundException(command.CustomerId);
        }

        if (!string.IsNullOrWhiteSpace(command.Name?.ToString()) && await _customerRepository.ExistsAsync(command.Name?.ToString()))
        {
            throw new CustomerAlreadyExistsException(command.Name?.ToString());
        }

        customer.Complete(command.Name, command.FullName, command.Address, command.Nationality,
            new Identity(command.IdentityType, command.IdentitySeries), _clock.CurrentDate());
        await _customerRepository.UpdateAsync(customer);
        await _messageBroker.PublishAsync(new CustomerCompleted(customer.Id, customer.Name, customer.FullName,
            customer.Nationality), cancellationToken);
        _logger.LogInformation($"Completed a customer with ID: '{command.CustomerId}'.");
    }
}