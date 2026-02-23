using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Mamey.Identity.Distributed.Services;

namespace Mamey.Identity.Distributed.Middleware;

/// <summary>
/// Middleware for handling microservice-to-microservice authentication.
/// </summary>
public class MicroserviceAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<MicroserviceAuthMiddleware> _logger;

    public MicroserviceAuthMiddleware(
        RequestDelegate next,
        ILogger<MicroserviceAuthMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Check for service token in Authorization header
            var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                
                // Validate the service token
                var microserviceAuthService = context.RequestServices.GetRequiredService<IMicroserviceAuthService>();
                var serviceInfo = await microserviceAuthService.GetServiceInfoAsync(token);
                
                if (serviceInfo != null)
                {
                    // Set service info in context
                    context.Items["ServiceInfo"] = serviceInfo;
                    context.Items["ServiceToken"] = token;
                    
                    _logger.LogDebug("Microservice authentication successful for service {ServiceId}", serviceInfo.ServiceId);
                }
            }

            // Check for service credentials in headers
            var serviceId = context.Request.Headers["X-Service-Id"].FirstOrDefault();
            var serviceSecret = context.Request.Headers["X-Service-Secret"].FirstOrDefault();
            
            if (!string.IsNullOrEmpty(serviceId) && !string.IsNullOrEmpty(serviceSecret))
            {
                var microserviceAuthService = context.RequestServices.GetRequiredService<IMicroserviceAuthService>();
                if (await microserviceAuthService.AuthenticateServiceAsync(serviceId, serviceSecret))
                {
                    context.Items["ServiceId"] = serviceId;
                    _logger.LogDebug("Service credential authentication successful for service {ServiceId}", serviceId);
                }
            }

            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in microservice authentication middleware");
            await _next(context);
        }
    }
}
