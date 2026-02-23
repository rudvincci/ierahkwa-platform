using Mamey.Government.Modules.Notifications.Core.Domain.Repositories;
using Mamey.Government.Modules.Notifications.Core.EF.Managers;
using Mamey.Government.Modules.Notifications.Core.EF.Repositories;
using Mamey.MicroMonolith.Infrastructure;
using Mamey.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.Notifications.Core.EF
{
    public static class Extensions
    {
        public static IServiceCollection AddPostgresDb(this IServiceCollection services)
        {
            services.AddScoped<INotificationManager, NotificationManager>();
            services.AddScoped<IUserManager, UserManager>();
            services
                .AddPostgres<NotificationsDbContext>(builder =>
                {
                    builder.MigrationsAssembly(typeof(NotificationsDbContext).Assembly.FullName);
                    builder.MigrationsHistoryTable("__EFMigrationsHistory_Notifications", "notifications");
                })
                .AddUnitOfWork<NotificationsUnitOfWork>()
                .AddManagers();
            services.AddScoped<NotificationsUnitOfWork>();
            services.AddTransient<NotificationsInitializer>();
            
            // Register Postgres repository for composite pattern
            services.AddScoped<Repositories.NotificationPostgresRepository>();

            services.AddInitializer<NotificationsInitializer>();
            return services;
        }
    }
}
