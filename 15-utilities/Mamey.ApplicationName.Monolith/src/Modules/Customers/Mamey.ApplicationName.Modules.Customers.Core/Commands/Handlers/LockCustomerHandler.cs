using System.Threading;
using System.Threading.Tasks;
using Mamey.ApplicationName.Modules.Customers.Core.Domain.Repositories;
using Mamey.ApplicationName.Modules.Customers.Core.Events;
using Mamey.ApplicationName.Modules.Customers.Core.Exceptions;
using Mamey.CQRS.Commands;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Mamey.ApplicationName.Modules.Customers.Core.Commands.Handlers;

internal sealed class LockCustomerHandler : ICommandHandler<LockCustomer>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<LockCustomerHandler> _logger;

    public LockCustomerHandler(ICustomerRepository customerRepository, IMessageBroker messageBroker,
        ILogger<LockCustomerHandler> logger)
    {
        _customerRepository = customerRepository;
        _messageBroker = messageBroker;
        _logger = logger;
    }
        
    public async Task HandleAsync(LockCustomer command, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetAsync(command.CustomerId);
        if (customer is null)
        {
            throw new CustomerNotFoundException(command.CustomerId);
        }
            
        customer.Lock(command.Notes);
        await _customerRepository.UpdateAsync(customer);
        await _messageBroker.PublishAsync(new CustomerLocked(command.CustomerId));
        _logger.LogInformation($"Locked a customer with ID: '{command.CustomerId}'.");
    }
}
