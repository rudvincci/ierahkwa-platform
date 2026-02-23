using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.Identity.Blazor.Services;

/// <summary>
/// Extension methods for registering Identity Blazor services.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds Identity Blazor services to the service collection.
    /// </summary>
    public static IServiceCollection AddIdentityBlazor(this IServiceCollection services)
    {
        services.AddScoped<IIdentityAuthService, IdentityAuthService>();
        services.AddScoped<Providers.IdentityAuthStateProvider>();
        services.AddScoped<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider>(
            sp => sp.GetRequiredService<Providers.IdentityAuthStateProvider>());
        
        return services;
    }
}
