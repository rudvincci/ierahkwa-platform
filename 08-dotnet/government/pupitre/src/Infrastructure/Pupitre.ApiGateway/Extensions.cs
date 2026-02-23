using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Pupitre.ApiGateway;

public static class Extensions
{
    public static IServiceCollection AddApiGatewayServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add YARP Reverse Proxy
        services.AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"));

        // Add Authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:SigningKey"]
                            ?? throw new InvalidOperationException("Jwt:SigningKey is required. Set it via environment variable or user-secrets.")))
                };
            });

        services.AddAuthorization();

        // Add Rate Limiting
        services.AddMemoryCache();
        services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
        services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));
        services.AddInMemoryRateLimiting();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        // Add Health Checks
        services.AddHealthChecks();

        // Add CORS — always use origin whitelist; AllowAll removed for security (OWASP A05)
        services.AddCors(options =>
        {
            options.AddPolicy("Default", builder =>
            {
                var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
                if (origins.Length == 0)
                {
                    // Fallback for local dev only — requires explicit config in production
                    origins = new[] { "http://localhost:5000", "https://localhost:5001", "http://localhost:3000" };
                }
                builder
                    .WithOrigins(origins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        return services;
    }

    public static WebApplication UseApiGatewayMiddleware(this WebApplication app)
    {
        // Exception handling
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/error");
            app.UseHsts();
        }

        // Rate limiting
        app.UseIpRateLimiting();

        // CORS — single policy for all environments
        app.UseCors("Default");

        // Authentication & Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        // Health checks
        app.MapHealthChecks("/health");
        app.MapHealthChecks("/health/ready");
        app.MapHealthChecks("/health/live");

        // Reverse Proxy
        app.MapReverseProxy();

        return app;
    }
}
