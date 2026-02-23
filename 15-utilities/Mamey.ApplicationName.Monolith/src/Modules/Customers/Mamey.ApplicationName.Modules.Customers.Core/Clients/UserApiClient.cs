using System.Threading.Tasks;
using Mamey.ApplicationName.Modules.Customers.Core.Clients.DTO;
using Mamey.MicroMonolith.Abstractions.Modules;
using Mamey.Modules;

namespace Mamey.ApplicationName.Modules.Customers.Core.Clients;

internal class UserApiClient : IUserApiClient
{
    private readonly IModuleClient _moduleClient;

    public UserApiClient(IModuleClient moduleClient)
    {
        _moduleClient = moduleClient;
    }

    public Task<UserDto> GetAsync(string email)
        => _moduleClient.SendAsync<UserDto>("users/get", new { email });
}
