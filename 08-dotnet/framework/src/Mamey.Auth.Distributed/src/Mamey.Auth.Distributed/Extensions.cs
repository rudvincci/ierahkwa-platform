using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Auth.Distributed;

public static class Extensions
{
    private const string RegistryName = "auth.distributed";

    public static IMameyBuilder AddDistributedAccessTokenValidator(this IMameyBuilder builder)
    {
        if (!builder.TryRegister(RegistryName))
        {
            return builder;
        }

        builder.Services.AddSingleton<IAccessTokenService, DistributedAccessTokenService>();

        return builder;
    }
}