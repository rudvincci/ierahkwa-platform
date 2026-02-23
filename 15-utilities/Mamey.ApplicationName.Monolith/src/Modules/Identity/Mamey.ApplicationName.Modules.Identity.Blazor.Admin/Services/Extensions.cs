using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ApplicationName.Modules.Identity.Blazor.Admin.Services;

internal static class Extensions
{
    public static IServiceCollection AddBlazorIdentityAdminServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}