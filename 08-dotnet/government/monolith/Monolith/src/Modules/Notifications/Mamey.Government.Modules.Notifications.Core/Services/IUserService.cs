using Mamey.Government.Modules.Notifications.Core.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Modules.Notifications.Core.Services;

internal interface IUserService
{
    Task<User> GetAsync(UserId userId);
}