using Mamey.FWID.Notifications.Application.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.FWID.Notifications.Infrastructure.Clients;

internal static class Extensions
{
    public static IServiceCollection AddServiceClients(this IServiceCollection services)
    {
        services.AddScoped<IIdentitiesServiceClient, IdentitiesServiceClient>();
        services.AddScoped<IDIDsServiceClient, DIDsServiceClient>();
        services.AddScoped<IZKPsServiceClient, ZKPsServiceClient>();
        services.AddScoped<ICredentialsServiceClient, CredentialsServiceClient>();
        services.AddScoped<IAccessControlsServiceClient, AccessControlsServiceClient>();
        return services;
    }
}







