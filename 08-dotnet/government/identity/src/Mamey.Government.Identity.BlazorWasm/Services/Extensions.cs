using Mamey.Government.Identity.BlazorWasm.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Bank.Accounts.BlazorWasm.Services;

public static class Extensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICookieBridge, CookieBridge>();
        return services;
    }

}

