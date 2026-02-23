using Microsoft.Extensions.DependencyInjection;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        // Register composite repositories as the interfaces
        builder.Services.AddScoped<IUserRepository, CompositeUserRepository>();
        builder.Services.AddScoped<IRoleRepository, CompositeRoleRepository>();
        builder.Services.AddScoped<IPermissionRepository, CompositePermissionRepository>();
        builder.Services.AddScoped<ISubjectRepository, CompositeSubjectRepository>();
        // ISessionRepository is registered in Redis/Extensions.cs
        builder.Services.AddScoped<IEmailConfirmationRepository, CompositeEmailConfirmationRepository>();
        builder.Services.AddScoped<ITwoFactorAuthRepository, CompositeTwoFactorAuthRepository>();
        builder.Services.AddScoped<IMultiFactorAuthRepository, CompositeMultiFactorAuthRepository>();
        builder.Services.AddScoped<IMfaChallengeRepository, CompositeMfaChallengeRepository>();
        builder.Services.AddScoped<ICredentialRepository, CompositeCredentialRepository>();
        
        // Register cross-aggregate composite repositories
        builder.Services.AddScoped<IAuditRepository, CompositeAuditRepository>();
        builder.Services.AddScoped<IAuthenticationRepository, CompositeAuthenticationRepository>();
        builder.Services.AddScoped<IAuthorizationRepository, CompositeAuthorizationRepository>();
        
        return builder;
    }
}
