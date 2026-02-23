using Mamey.ApplicationName.Modules.Notifications.Core.EF.Managers;
using Mamey.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ApplicationName.Modules.Notifications.Core.EF
{
    public static class Extensions
    {
        public static IServiceCollection AddPostgres(this IServiceCollection services)
        {
            services.AddScoped<INotificationManager, NotificationManager>();
            services.AddScoped<IUserManager, UserManager>();
            services
                .AddPostgres<NotificationsDbContext>()
                .AddUnitOfWork<NotificationsUnitOfWork>()
                .AddManagers();

            return services;
        }
    }
}