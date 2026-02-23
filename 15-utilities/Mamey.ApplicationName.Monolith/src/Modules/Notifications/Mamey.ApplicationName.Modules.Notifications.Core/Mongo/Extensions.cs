using Mamey.ApplicationName.Modules.Notifications.Core.Mongo.Documents;
using Mamey.ApplicationName.Modules.Notifications.Core.Mongo.Repositories;
using Mamey.ApplicationName.Modules.Notifications.Core.Domain.Repositories;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Mongo
{
    internal static class Extensions
    {
        internal static readonly string Schema = "notifications-module";

        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
            services.AddScoped<INotificationRepository, NotificationsMongoRepository>();
            services.AddScoped<IEmailNotificationRepository, EmailNotificationMongoRepository>();
            services.AddScoped<ISMSNotificationRepository, SMSNotificationMongoRepository>();
            services.AddMongoRepository<NotificationDocument, Guid>($"{Schema}.notifications");
            services.AddMongoRepository<EmailNotificationDocument, Guid>($"{Schema}.email.notifications");
            services.AddMongoRepository<SMSNotificationDocument, Guid>($"{Schema}.sms.notifications");
            return services;
        }
        
        public static IApplicationBuilder UseMongoDb(this IApplicationBuilder builder)
        {
            using var scope = builder.ApplicationServices.CreateScope();
            var notifications = scope.ServiceProvider.GetService<IMongoRepository<NotificationDocument, Guid>>().Collection;
            var notificationBuilderIndexKeys = Builders<NotificationDocument>.IndexKeys;
            var emailNotifications = scope.ServiceProvider.GetService<IMongoRepository<EmailNotificationDocument, Guid>>().Collection;
            var emailNotificationBuilderIndexKeys = Builders<EmailNotificationDocument>.IndexKeys;
            var smsNotifications = scope.ServiceProvider.GetService<IMongoRepository<SMSNotificationDocument, Guid>>().Collection;
            var smsNotificationBuilderIndexKeys = Builders<SMSNotificationDocument>.IndexKeys;
        
            Task.Run(async () => await notifications.Indexes.CreateManyAsync(
                new[]
                {
                    new CreateIndexModel<NotificationDocument>(notificationBuilderIndexKeys.Ascending(i => i.Id),
                        new CreateIndexOptions
                        {
                            Unique = true
                        }),
                }));
            Task.Run(async () => await emailNotifications.Indexes.CreateManyAsync(
                new[]
                {
                    new CreateIndexModel<EmailNotificationDocument>(emailNotificationBuilderIndexKeys.Ascending(i => i.Id),
                        new CreateIndexOptions
                        {
                            Unique = true
                        }),
                }));
            Task.Run(async () => await smsNotifications.Indexes.CreateManyAsync(
                new[]
                {
                    new CreateIndexModel<SMSNotificationDocument>(smsNotificationBuilderIndexKeys.Ascending(i => i.Id),
                        new CreateIndexOptions
                        {
                            Unique = true
                        }),
                }));
            return builder;
        }
    }
}
