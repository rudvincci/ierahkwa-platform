using Mamey.FWID.Notifications.Application.Services;
using Mamey.FWID.Notifications.Infrastructure.MinIO.Services;
using Mamey.Microservice.Infrastructure;
using Mamey.Persistence.Minio;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mamey.FWID.Notifications.Infrastructure.MinIO;

internal static class Extensions
{
    /// <summary>
    /// Adds MinIO storage services for the Notifications service.
    /// </summary>
    /// <param name="builder">The Mamey builder.</param>
    /// <returns>The Mamey builder for chaining.</returns>
    public static IMameyBuilder AddMinIOStorage(this IMameyBuilder builder)
    {
        // MinIO services are already registered by AddMinio() in Infrastructure/Extensions.cs
        // We just need to register our service-specific implementations

        // Register NotificationStorageService
        builder.Services.AddScoped<INotificationStorageService, NotificationStorageService>();

        // Register bucket initialization service
        builder.Services.AddHostedService<BucketInitializationService>();

        return builder;
    }
}







