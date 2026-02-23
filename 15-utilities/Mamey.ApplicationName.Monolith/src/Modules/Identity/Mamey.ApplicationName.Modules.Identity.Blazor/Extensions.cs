using Mamey.ApplicationName.Modules.Identity.Blazor.Clients;
using Mamey.ApplicationName.Modules.Identity.Blazor.Services;
using Mamey.ApplicationName.Modules.Identity.Blazor.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ApplicationName.Modules.Identity.Blazor;

public static class Extensions
{
    public static IServiceCollection AddBlazorIdentity(this IServiceCollection services)
    {
        
        return services
                .AddMameyIdentityBlazorViewModels()
                .AddIdentityBlazorServices()
                .AddIdentityBlazorClients()
            ;
    }
}