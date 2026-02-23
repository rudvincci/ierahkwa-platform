using Mamey.ApplicationName.Modules.Notifications.Core.Domain.Entities;
using Mamey.Persistence.SQL.Contracts;

namespace Mamey.ApplicationName.Modules.Notifications.Core.EF.Managers;

internal interface IUserManager : IGenericManager<User>
{
    IEnumerable<User> GetByEmail(string email);
}