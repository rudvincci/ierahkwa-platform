using MameyNode.Portals.Infrastructure.Authentication;
using MameyNode.Portals.Infrastructure.Authorization;
using MameyNode.Portals.Infrastructure.StateManagement;
using MameyNode.Portals.Infrastructure.ErrorHandling;
using MameyNode.Portals.Infrastructure.Logging;
using MameyNode.Portals.Infrastructure.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MameyNode.Portals.Infrastructure;

/// <summary>
/// Infrastructure extension methods for dependency injection
/// </summary>
public static class InfrastructureExtensions
{
    /// <summary>
    /// Add portal infrastructure services
    /// </summary>
    public static IServiceCollection AddPortalInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Authentication
        services.AddScoped<PQCAuthenticationHandler>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddHttpContextAccessor();
        
        // Authorization
        services.AddScoped<IRoleBasedAuthorizationService, RoleBasedAuthorizationService>();
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("CitizenOrAgent", policy => policy.RequireRole("Citizen", "Agent"));
        });
        
        // State Management
        services.AddSingleton<IAppStateService, AppStateService>();
        
        // Logging
        services.AddScoped<IStructuredLoggingService, StructuredLoggingService>();
        
        // Configuration
        services.AddScoped<IPortalConfigurationService, PortalConfigurationService>();
        services.Configure<Dictionary<string, PortalSettings>>(configuration.GetSection("portals"));
        
        return services;
    }
    
    /// <summary>
    /// Use portal infrastructure middleware
    /// </summary>
    public static IApplicationBuilder UsePortalInfrastructure(this IApplicationBuilder app)
    {
        // Global error handling
        app.UseMiddleware<GlobalErrorHandlerMiddleware>();
        
        // Correlation ID middleware
        app.Use(async (context, next) =>
        {
            var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() 
                ?? Guid.NewGuid().ToString();
            context.Items["CorrelationId"] = correlationId;
            context.Response.Headers["X-Correlation-ID"] = correlationId;
            await next();
        });
        
        return app;
    }
}


