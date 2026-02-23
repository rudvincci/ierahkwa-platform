using Mamey.Government.Modules.Notifications.Core.Domain.Repositories;
using Mamey.Government.Modules.Notifications.Core.EF.Repositories;
using Mamey.Government.Modules.Notifications.Core.Mongo.Repositories;
using Mamey.Government.Modules.Notifications.Core.Redis.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.Notifications.Core.Composite;

internal static class Extensions
{
    public static IServiceCollection AddCompositeRepositories(this IServiceCollection services)
    {
        // Register individual repositories (for composite to use)
        services.AddScoped<NotificationPostgresRepository>();
        services.AddScoped<NotificationsMongoRepository>();
        services.AddScoped<NotificationRedisRepository>();
        
        // Register composite repository as the interface
        services.AddScoped<INotificationRepository, CompositeNotificationRepository>();
        
        return services;
    }
}
