using Mamey.ApplicationName.Modules.Identity.Blazor.Admin.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ApplicationName.Modules.Identity.Blazor.Admin;

public static class Extensions
{
    public static IServiceCollection AddBlazorIdentityAdmin(this IServiceCollection services)
    {
        return services
            
            .AddBlazorIdentityAdminClients()
            .AddBlazorIdentityAdminClients();
    }
}