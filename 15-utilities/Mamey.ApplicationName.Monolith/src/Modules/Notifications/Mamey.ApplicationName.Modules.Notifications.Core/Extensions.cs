using System.Runtime.CompilerServices;
using Mamey.ApplicationName.Modules.Notifications.Core.Clients;
using Mamey.ApplicationName.Modules.Notifications.Core.DTO;
using Mamey.ApplicationName.Modules.Notifications.Core.EF;
using Mamey.ApplicationName.Modules.Notifications.Core.Hubs;
using Mamey.ApplicationName.Modules.Notifications.Core.Mongo;
using Mamey.ApplicationName.Modules.Notifications.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Mamey.ApplicationName.Modules.Notifications.Api")]
namespace Mamey.ApplicationName.Modules.Notifications.Core
{
    internal static class Extensions
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
            => services
                .AddPostgres()
                .AddMongo()
                .AddNotificationServices()
                .AddModuleClients();

        public static IApplicationBuilder UseCore(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotificationHub>("/notificationsHub");
            });
            return app;
        }
        public static class MockNotificationService
        {
            private static readonly Random Random = new();
            private static readonly List<string> Messages = new()
            {
                "Your order has been shipped.",
                "New message from customer support.",
                "Reminder: Your subscription is about to expire.",
                "Security alert: Unusual login detected.",
                "System maintenance scheduled for tonight.",
                "New update available for your app.",
                "Congratulations! You've won a prize.",
                "Your password has been successfully changed.",
                "New comment on your post.",
                "Check out the latest deals in our store."
            };

            private static readonly List<string> Titles = new()
            {
                "Order UpdateAsync",
                "New Message",
                "Reminder",
                "Security Alert",
                "System UpdateAsync",
                "Promotion",
                "Congratulations",
                "Password Change",
                "Comment",
                "Special Offer"
            };

            private static readonly Dictionary<string, string> Icons = new()
            {
                { "Alert", "warning" },
                { "Message", "message" },
                { "Reminder", "notifications" },
                { "System", "update" },
                { "Promotion", "star" },
                { "Security", "lock" },
                { "Comment", "comment" }
            };

            private static readonly List<string> Categories = new()
            {
                "Alert",
                "Message",
                "Reminder",
                "System",
                "Promotion",
                "Security",
                "Comment"
            };

            /// <summary>
            /// Generate a list of mock notifications.
            /// </summary>
            /// <param name="count">Number of notifications to generate.</param>
            /// <returns>List of mock notifications.</returns>
            public static List<NotificationDetailsDto> GenerateMockNotifications(int count)
            {
                var notifications = new List<NotificationDetailsDto>();

                for (int i = 0; i < count; i++)
                {
                    var category = Categories[Random.Next(Categories.Count)];
                    var titleIndex = Random.Next(Titles.Count);
                    var messageIndex = Random.Next(Messages.Count);

                    var notification = new NotificationDetailsDto
                    {
                        Id = Guid.NewGuid(),
                        Title = Titles[titleIndex],
                        Message = Messages[messageIndex],
                        Icon = Icons.ContainsKey(category) ? Icons[category] : @"<path d=""M12 22c1.1 0 2-.9 2-2h-4c0 1.1.89 2 2 2zm6-6v-5c0-3.07-1.64-5.64-4.5-6.32V4c0-.83-.67-1.5-1.5-1.5s-1.5.67-1.5 1.5v.68C7.63 5.36 6 7.92 6 11v5l-2 2v1h16v-1l-2-2z""/>",
                        Timestamp = DateTime.UtcNow.AddMinutes(-Random.Next(1, 120)),
                        IsRead = Random.Next(0, 2) == 1,
                        Category = category,
                        UserId = Random.Next(0, 2) == 0 ? Guid.Parse("46517c75-d120-4616-b9b9-d2c1d38fd066").ToString() : null
                    };

                    notifications.Add(notification);
                }

                return notifications;
            }
        }
    }
}