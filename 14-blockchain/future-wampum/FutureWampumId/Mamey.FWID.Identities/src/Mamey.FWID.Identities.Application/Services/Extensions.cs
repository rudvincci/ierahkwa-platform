using Mamey.FWID.Identities.Application.Documents;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.FWID.Identities.Application.Services;

internal static class Extensions
{
    internal static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register authentication services
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IMultiFactorAuthService, MultiFactorAuthService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IEmailConfirmationService, EmailConfirmationService>();
        services.AddScoped<ISmsConfirmationService, SmsConfirmationService>();

        // Register document generation services
        services.AddDocumentGenerationServices();

        return services;
    }
}