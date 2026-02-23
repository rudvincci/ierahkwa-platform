using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.TravelIdentities.Core.Sync;

internal static class Extensions
{
    public static IServiceCollection AddReadModelSyncServices(this IServiceCollection services)
    {
        services.AddScoped<IReadModelSyncService, ReadModelSyncService>();
        return services;
    }
}
