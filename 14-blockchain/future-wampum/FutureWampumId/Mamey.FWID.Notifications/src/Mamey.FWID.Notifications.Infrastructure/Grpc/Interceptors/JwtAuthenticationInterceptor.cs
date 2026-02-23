using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Mamey.FWID.Notifications.Infrastructure.Grpc.Interceptors;

/// <summary>
/// gRPC interceptor for JWT authentication (external clients)
/// </summary>
public class JwtAuthenticationInterceptor : Interceptor
{
    private readonly ILogger<JwtAuthenticationInterceptor> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtAuthenticationInterceptor(
        ILogger<JwtAuthenticationInterceptor> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            // Extract JWT token from metadata
            var token = ExtractJwtToken(context);
            
            if (!string.IsNullOrEmpty(token))
            {
                // Validate JWT token
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    // Set the authorization header for authentication
                    httpContext.Request.Headers["Authorization"] = $"Bearer {token}";
                    
                    var authenticateResult = await httpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
                    if (authenticateResult.Succeeded && authenticateResult.Principal != null)
                    {
                        // Set user in context
                        httpContext.User = authenticateResult.Principal;
                        
                        // Add user info to gRPC context
                        context.UserState["AuthenticatedUser"] = authenticateResult.Principal;
                        context.UserState["UserId"] = authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                            ?? authenticateResult.Principal.FindFirst("sub")?.Value;
                        
                        _logger.LogDebug("JWT authentication successful for gRPC call: {Method}", context.Method);
                    }
                    else
                    {
                        _logger.LogWarning("JWT authentication failed for gRPC call: {Method}", context.Method);
                        throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid or expired JWT token"));
                    }
                }
            }
            else
            {
                // Allow anonymous access for certain methods (e.g., health checks)
                if (!IsAnonymousAllowed(context.Method))
                {
                    _logger.LogWarning("No JWT token provided for gRPC call: {Method}", context.Method);
                    throw new RpcException(new Status(StatusCode.Unauthenticated, "JWT token required"));
                }
            }

            return await continuation(request, context);
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in JWT authentication interceptor for gRPC call: {Method}", context.Method);
            throw new RpcException(new Status(StatusCode.Internal, "Authentication error"));
        }
    }

    private static string? ExtractJwtToken(ServerCallContext context)
    {
        var authHeader = context.RequestHeaders.FirstOrDefault(h => h.Key == "authorization");
        if (authHeader == null)
        {
            return null;
        }

        var value = authHeader.Value;
        if (string.IsNullOrEmpty(value) || !value.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return value.Substring("Bearer ".Length).Trim();
    }

    private static bool IsAnonymousAllowed(string method)
    {
        // Allow anonymous access to health check methods
        return method.Contains("Health", StringComparison.OrdinalIgnoreCase) ||
               method.Contains("Ping", StringComparison.OrdinalIgnoreCase);
    }
}







