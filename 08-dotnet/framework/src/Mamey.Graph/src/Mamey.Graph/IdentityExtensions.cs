using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Graph;

public static class IdentityExtensions
{
    public static IMameyBuilder AddAzureIdentity(this IMameyBuilder builder)
    {
        builder
            .Services
                .ConfigureAzureIdentityServices();
        return builder;
    }
    public static IServiceCollection ConfigureAzureIdentityServices(this IServiceCollection services)
    {
        return services;
    }

}

