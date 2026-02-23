using Mamey.Auth.Jwt;
using Mamey.Government.Identity.Application.Auth;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Application.Services;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Identity.Infrastructure.Auth;

internal class JwtProvider : IJwtProvider
{
    private readonly IJwtHandler _jwtHandler;
        
    private readonly UserManager<User> _userManager;
    public JwtProvider(IJwtHandler jwtHandler)
    {
        _jwtHandler = jwtHandler;
    }

    public AuthDto Create(Guid userId, string name, string email, string role, UserType type, UserStatus status,Constants.User.Permission permissions,  string? audience = null,
        IDictionary<string, string>? claims = null)
    {

        var jwt = _jwtHandler.CreateToken(userId.ToString("N"), null, audience, claims);

        return new AuthDto
        {
            UserId = userId,
            Email = email,
            AccessToken = jwt.AccessToken,
            RefreshToken = jwt.RefreshToken,
            Expires = jwt.Expires,
            Role = jwt.Role,
            Status = status.ToString(),
            Type = type.ToString(),
            Claims = jwt.Claims
        };
    }
        
}

internal static class Extensions
{
    public static IServiceCollection AddIdentityAuth(this IServiceCollection services)
    {
        services.AddSingleton<IJwtProvider, JwtProvider>();
        // services.AddScoped<IPasswordRecoveryHandler, PasswordRecoveryHandler>();
        // services.AddScoped<IPasswordRecoveryService, PasswordRecoveryService>();
        // services.AddTransient<PasswordRecoveryTokenMiddleware>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        // services.AddScoped<IPasswordHasher<IPasswordService>, PasswordHasher<IPasswordService>>();
        return services;
    }
}