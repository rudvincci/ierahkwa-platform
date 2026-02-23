using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ApplicationName.Modules.Identity.Core.Storage;

internal static class Extensions
{
    public static IServiceCollection AddStorage(this IServiceCollection services)
    {
        services.AddScoped<IUserRequestStorage, UserRequestStorage>();
        return services;
    }
}