using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.Saga.Api.Integration;

/// <summary>
/// Extension methods for registering saga integration handlers.
/// </summary>
public static class Extensions
{
    public static IServiceCollection AddSagaIntegration(this IServiceCollection services)
    {
        // Register integration event handlers
        services.AddScoped<CitizenshipApplicationEventHandler>();
        services.AddScoped<CitizensEventHandler>();
        // services.AddScoped<PassportsEventHandler>();
        // services.AddScoped<TravelIdentitiesEventHandler>();
        
        return services;
    }
}
