using Mamey.Auth.Identity.Managers;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Blazor.Identity;

public static class Extensions
{
    private const string RegistryName = "blazor-identity";
    public static IMameyBuilder AddMameyBlazorIdentity(this IMameyBuilder builder)
    {
        if (!builder.TryRegister(RegistryName))
        {
            return builder;
        }
        builder.Services.AddScoped<IIdentityRedirectManager, IdentityRedirectManager>();
        // builder.Services.AddScoped<IIdentityUserAccessor, IdentityUserAccessor>();
        // builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
        return builder;
    }
}