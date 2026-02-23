using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mamey;

namespace Mamey.Portal.Auth.Infrastructure;

public static class Extensions
{
    public static IMameyBuilder AddAuthInfrastructure(
        this IMameyBuilder builder)
    {
        // 1. Ensure logging is registered
        if (!builder.Services.Any(s => s.ServiceType == typeof(ILoggerFactory)))
        {
            builder.Services.AddLogging();
        }

        // 2. Register Application Services
        RegisterApplicationServices(builder.Services);

        return builder;
    }

    private static void RegisterApplicationServices(IServiceCollection services)
    {
        // TODO: Auth services will be registered here when implemented
        // Currently using Authentik OIDC for authentication (configured in Program.cs)
        // Future: Move mock auth services and Authentik configuration here
    }
}
