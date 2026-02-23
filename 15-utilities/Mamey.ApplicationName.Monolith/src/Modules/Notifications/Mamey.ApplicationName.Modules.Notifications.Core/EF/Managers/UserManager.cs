using Mamey.ApplicationName.Modules.Notifications.Core.Domain.Entities;
using Mamey.Persistence.SQL;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ApplicationName.Modules.Notifications.Core.EF.Managers;

internal class UserManager : GenericManager<User>, IUserManager
{
    
    public UserManager(NotificationsDbContext context) 
        : base(context)
    {
    }


    public IEnumerable<User> GetByEmail(string email)
    {
        throw new NotImplementedException();
    }
}

public static class Extensions
{
    public static IServiceCollection AddManagers(this IServiceCollection services)
    {
        services.AddScoped<IUserManager, UserManager>();
        services.AddScoped<INotificationManager, NotificationManager>();
        
        return services;
    }
}