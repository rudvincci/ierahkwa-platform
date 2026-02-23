using System.Threading;
using System.Threading.Tasks;
using Mamey.ApplicationName.Modules.Customers.Core.Domain.Repositories;
using Mamey.ApplicationName.Modules.Customers.Core.Events;
using Mamey.ApplicationName.Modules.Customers.Core.Exceptions;
using Mamey.CQRS.Commands;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Mamey.ApplicationName.Modules.Customers.Core.Commands.Handlers;

internal sealed class UnlockCustomerHandler : ICommandHandler<UnlockCustomer>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<UnlockCustomerHandler> _logger;

    public UnlockCustomerHandler(ICustomerRepository customerRepository, IMessageBroker messageBroker,
        ILogger<UnlockCustomerHandler> logger)
    {
        _customerRepository = customerRepository;
        _messageBroker = messageBroker;
        _logger = logger;
    }
        
    public async Task HandleAsync(UnlockCustomer command, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetAsync(command.CustomerId);
        if (customer is null)
        {
            throw new CustomerNotFoundException(command.CustomerId);
        }
            
        customer.Unlock(command.Notes);
        await _customerRepository.UpdateAsync(customer);
        await _messageBroker.PublishAsync(new CustomerUnlocked(command.CustomerId));
        _logger.LogInformation($"Unlocked a customer with ID: '{command.CustomerId}'.");
    }
}
