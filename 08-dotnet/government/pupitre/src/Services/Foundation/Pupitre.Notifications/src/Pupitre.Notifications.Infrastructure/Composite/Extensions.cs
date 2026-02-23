using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Notifications.Domain.Repositories;

namespace Pupitre.Notifications.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<INotificationRepository, CompositeNotificationRepository>();
        return builder;
    }
}
