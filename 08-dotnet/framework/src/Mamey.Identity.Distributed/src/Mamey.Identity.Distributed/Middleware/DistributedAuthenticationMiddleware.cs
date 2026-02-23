using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Mamey.Identity.Distributed.Services;

namespace Mamey.Identity.Distributed.Middleware;

/// <summary>
/// Middleware for handling distributed authentication.
/// </summary>
public class DistributedAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DistributedAuthenticationMiddleware> _logger;

    public DistributedAuthenticationMiddleware(
        RequestDelegate next,
        ILogger<DistributedAuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Check for distributed token in Authorization header
            var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                
                // Validate the distributed token
                var tokenValidationService = context.RequestServices.GetRequiredService<ITokenValidationService>();
                if (await tokenValidationService.ValidateDistributedTokenAsync(token))
                {
                    // Get user from token
                    var distributedTokenService = context.RequestServices.GetRequiredService<IDistributedTokenService>();
                    var user = await distributedTokenService.GetUserFromTokenAsync(token);
                    
                    if (user != null)
                    {
                        // Set user in context
                        context.Items["DistributedUser"] = user;
                        context.Items["DistributedToken"] = token;
                        
                        _logger.LogDebug("Distributed authentication successful for user {UserId}", user.UserId);
                    }
                }
            }

            // Check for session-based authentication
            var sessionId = context.Request.Headers["X-Session-Id"].FirstOrDefault();
            if (!string.IsNullOrEmpty(sessionId))
            {
                var sessionService = context.RequestServices.GetRequiredService<IDistributedSessionService>();
                var user = await sessionService.GetSessionAsync(sessionId);
                
                if (user != null)
                {
                    context.Items["DistributedUser"] = user;
                    context.Items["SessionId"] = sessionId;
                    
                    _logger.LogDebug("Distributed session authentication successful for user {UserId}", user.UserId);
                }
            }

            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in distributed authentication middleware");
            await _next(context);
        }
    }
}
