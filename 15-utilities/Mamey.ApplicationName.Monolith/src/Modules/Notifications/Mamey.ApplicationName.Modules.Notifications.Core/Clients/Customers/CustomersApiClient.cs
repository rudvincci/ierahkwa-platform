using Mamey.ApplicationName.Modules.Notifications.Core.Clients.Customers.DTO;
using Mamey.MicroMonolith.Abstractions.Modules;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Clients.Customers;

public class CustomersApiClient
{
    private readonly IModuleClient _moduleClient;

    public CustomersApiClient(IModuleClient moduleClient)
    {
        _moduleClient = moduleClient;
    }
        
    public Task<CustomerDto> GetAsync(Guid customerId)
        => _moduleClient.SendAsync<CustomerDto>("customers/get", new { customerId });
}