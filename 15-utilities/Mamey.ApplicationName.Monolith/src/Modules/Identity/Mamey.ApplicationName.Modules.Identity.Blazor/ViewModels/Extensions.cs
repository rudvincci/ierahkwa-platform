using Mamey.ApplicationName.Modules.Identity.Blazor.ViewModels.Auth;
using Mamey.ApplicationName.Modules.Identity.Blazor.ViewModels.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ApplicationName.Modules.Identity.Blazor.ViewModels;

public static class Extensions
{
    public static IServiceCollection AddMameyIdentityBlazorViewModels(this IServiceCollection services)
    {
        services.AddScoped<LoginViewModelProvider>();
        services.AddScoped<LoginViewModel>();
        return services;
    }
}