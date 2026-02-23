using Mamey.Government.Modules.Notifications.Core.Domain.Entities;

namespace Mamey.Government.Modules.Notifications.Core.Domain.Repositories;

internal interface IUserRepository
{
    Task<User?> GetAsync(Guid userId);
}