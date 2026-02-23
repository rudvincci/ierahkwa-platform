using Mamey.ApplicationName.Modules.Identity.Contracts.Commands;
using Mamey.Blazor.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ApplicationName.Modules.Identity.Blazor.Services;

public static class Extensions
{
    public static IServiceCollection AddIdentityBlazorServices(this IServiceCollection services)
    {
        // services.AddScoped<IIdentityAuthService, IdentityAuthService>();
        return services;
    }
}