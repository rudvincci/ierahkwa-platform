using Mamey.Government.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Identity.Application.Services;

internal static class Extensions
{
    internal static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IEmailConfirmationService, EmailConfirmationService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IMultiFactorAuthService, MultiFactorAuthService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<ITwoFactorAuthService, TwoFactorAuthService>();
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}