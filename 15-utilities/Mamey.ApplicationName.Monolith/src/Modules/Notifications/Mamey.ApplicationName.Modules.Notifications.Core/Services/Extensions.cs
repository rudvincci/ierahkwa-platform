using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Services;

internal static class Extensions
{
    public static IServiceCollection AddNotificationServices(this IServiceCollection services)
    {
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}