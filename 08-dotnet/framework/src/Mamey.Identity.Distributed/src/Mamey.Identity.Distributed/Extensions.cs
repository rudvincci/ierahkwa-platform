using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Mamey.Identity.Distributed.Configuration;
using Mamey.Identity.Distributed.Services;
using Mamey.Identity.Distributed.Middleware;
using Mamey.Identity.Core;

namespace Mamey.Identity.Distributed;

/// <summary>
/// Extension methods for configuring distributed identity services.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds distributed identity services to the service collection.
    /// </summary>
    /// <param name="builder">The Mamey builder.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The Mamey builder.</returns>
    public static IMameyBuilder AddDistributedIdentity(this IMameyBuilder builder, IConfiguration configuration)
    {
        builder.Services.Configure<DistributedIdentityOptions>(configuration.GetSection(DistributedIdentityOptions.SectionName));
        
        var options = configuration.GetSection(DistributedIdentityOptions.SectionName).Get<DistributedIdentityOptions>() ?? new DistributedIdentityOptions();
        
        // Register distributed services
        builder.Services.AddScoped<IDistributedTokenService, DistributedTokenService>();
        builder.Services.AddScoped<IDistributedSessionService, DistributedSessionService>();
        builder.Services.AddScoped<IMicroserviceAuthService, MicroserviceAuthService>();
        builder.Services.AddScoped<ITokenValidationService, TokenValidationService>();
        
        // Register middleware
        builder.Services.AddScoped<DistributedAuthenticationMiddleware>();
        builder.Services.AddScoped<MicroserviceAuthMiddleware>();
        
        // Configure JWT authentication for distributed scenarios
        if (options.EnableJwtAuthentication)
        {
            builder.Services.AddAuthentication("DistributedJwt")
                .AddJwtBearer("DistributedJwt", jwtOptions =>
                {
                    jwtOptions.TokenValidationParameters = options.JwtValidationParameters;
                    jwtOptions.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            var tokenService = context.HttpContext.RequestServices.GetRequiredService<IDistributedTokenService>();
                            var jwtToken = context.SecurityToken as JwtSecurityToken;
                            if (jwtToken != null)
                            {
                                var isValid = await tokenService.ValidateTokenAsync(jwtToken.RawData);
                                if (!isValid)
                                {
                                    context.Fail("Token validation failed");
                                }
                            }
                        }
                    };
                });
        }
        
        return builder;
    }
    
    /// <summary>
    /// Adds distributed identity services with custom configuration.
    /// </summary>
    /// <param name="builder">The Mamey builder.</param>
    /// <param name="configureOptions">Action to configure options.</param>
    /// <returns>The Mamey builder.</returns>
    public static IMameyBuilder AddDistributedIdentity(this IMameyBuilder builder, Action<DistributedIdentityOptions> configureOptions)
    {
        builder.Services.Configure(configureOptions);
        
        var options = new DistributedIdentityOptions();
        configureOptions(options);
        
        // Register distributed services
        builder.Services.AddScoped<IDistributedTokenService, DistributedTokenService>();
        builder.Services.AddScoped<IDistributedSessionService, DistributedSessionService>();
        builder.Services.AddScoped<IMicroserviceAuthService, MicroserviceAuthService>();
        builder.Services.AddScoped<ITokenValidationService, TokenValidationService>();
        
        // Register middleware
        builder.Services.AddScoped<DistributedAuthenticationMiddleware>();
        builder.Services.AddScoped<MicroserviceAuthMiddleware>();
        
        return builder;
    }
}
