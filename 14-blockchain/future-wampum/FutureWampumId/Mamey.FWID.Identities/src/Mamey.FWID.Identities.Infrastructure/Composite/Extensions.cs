using Microsoft.Extensions.DependencyInjection;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Infrastructure.EF.Repositories;
using Mamey.Microservice.Infrastructure;

namespace Mamey.FWID.Identities.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        // Register composite repositories as the interfaces
        builder.Services.AddScoped<IIdentityRepository, CompositeIdentityRepository>();
        builder.Services.AddScoped<ISessionRepository, CompositeSessionRepository>();
        builder.Services.AddScoped<IMfaConfigurationRepository, CompositeMfaConfigurationRepository>();
        builder.Services.AddScoped<IPermissionRepository, PermissionPostgresRepository>();
        builder.Services.AddScoped<IRoleRepository, RolePostgresRepository>();
        builder.Services.AddScoped<IIdentityPermissionRepository, IdentityPermissionPostgresRepository>();
        builder.Services.AddScoped<IIdentityRoleRepository, IdentityRolePostgresRepository>();
        builder.Services.AddScoped<IEmailConfirmationRepository, EmailConfirmationPostgresRepository>();
        builder.Services.AddScoped<ISmsConfirmationRepository, SmsConfirmationPostgresRepository>();
        
        return builder;
    }
}



