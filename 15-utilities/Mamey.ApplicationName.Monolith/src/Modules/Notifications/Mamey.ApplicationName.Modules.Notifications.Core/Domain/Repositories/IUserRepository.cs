using Mamey.ApplicationName.Modules.Notifications.Core.Domain.Entities;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Domain.Repositories;

internal interface IUserRepository
{
    Task<User?> GetAsync(Guid userId);
}