using Mamey.ApplicationName.Modules.Notifications.Core.Clients.Users.DTO;
using Mamey.MicroMonolith.Abstractions.Modules;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Clients.Users;

public class UsersApiClient : IUsersApiClient
{
    private readonly IModuleClient _moduleClient;

    public UsersApiClient(IModuleClient moduleClient)
    {
        _moduleClient = moduleClient;
    }
        
    public Task<ApplicationUserDto> GetAsync(Guid userId)
        => _moduleClient.SendAsync<ApplicationUserDto>($"identity-module/account/", new { UserId = userId });
}

public interface IUsersApiClient
{
    Task<ApplicationUserDto> GetAsync(Guid userId);
}