using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Identity.BlazorWasm.Storage;

public static class Extensions
{
    public static IServiceCollection AddProtectedLocalStorage(this IServiceCollection services)
    {
        services.AddDataProtection(); // backing for ProtectedLocalStorage

        services.AddScoped<ProtectedLocalStorage>();
        services.AddScoped<ITokenStore, ProtectedLocalTokenStore>();
        return services;
    }
}