using Mamey.Government.Modules.Notifications.Core.Clients.Users;
using Mamey.Government.Modules.Notifications.Core.Domain.Entities;
using Mamey.Government.Modules.Notifications.Core.EF.Managers;
using Mamey.Government.Modules.Notifications.Core.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Modules.Notifications.Core.Services;

internal class UserService : IUserService
{
    private readonly IUsersApiClient _usersApiClient;
    private readonly IUserManager _userManager;

    public UserService(IUserManager userManager, IUsersApiClient usersApiClient)
    {
        _userManager = userManager;
        _usersApiClient = usersApiClient;
    }

    public async Task<User> GetAsync(UserId userId)
    {
        var user = await _usersApiClient.GetAsync(userId);
        if (user == null)
        {
            throw new UserNotFoundException(userId);
        }

        var existingUser = _userManager.GetById(new object[] { new UserId(user.Id) });
        if (existingUser != null)
        {
            throw new UserAlreadyExistsException(existingUser.Id);
        }

        // TODO: Get User settings
        User newUser = new User(user.Id, user.Name, user.Email);
        await _userManager.AddAsync(newUser);
        // TODO: Cache user
        return newUser;
    }
}