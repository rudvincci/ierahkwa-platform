using Mamey.ApplicationName.Modules.Notifications.Core.Domain.Entities;
using Mamey.Types;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Services;

internal interface IUserService
{
    Task<User> GetAsync(UserId userId);
}