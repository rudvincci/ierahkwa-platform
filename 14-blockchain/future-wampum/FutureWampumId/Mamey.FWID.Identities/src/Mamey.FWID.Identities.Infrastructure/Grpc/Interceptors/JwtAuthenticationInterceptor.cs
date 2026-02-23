using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Mamey.FWID.Identities.Infrastructure.Grpc.Interceptors;

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
                        var userId = authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                            ?? authenticateResult.Principal.FindFirst("sub")?.Value;
                        if (userId != null)
                        {
                            context.UserState["UserId"] = userId;
                        }
                        
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
            _logger.LogError(ex, "Error during JWT authentication for gRPC call: {Method}", context.Method);
            throw new RpcException(new Status(StatusCode.Internal, "Authentication error"));
        }
    }

    private string? ExtractJwtToken(ServerCallContext context)
    {
        // Try to extract JWT from metadata
        var authHeader = context.RequestHeaders.Get("authorization")?.Value;
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }

        // Try alternative header name
        var jwtHeader = context.RequestHeaders.Get("x-jwt-token")?.Value;
        if (!string.IsNullOrEmpty(jwtHeader))
        {
            return jwtHeader;
        }

        return null;
    }

    private bool IsAnonymousAllowed(string method)
    {
        // Allow anonymous access for health checks and certain internal methods
        var anonymousMethods = new[]
        {
            "/grpc.health.v1.Health/Check",
            "/grpc.health.v1.Health/Watch"
        };

        return anonymousMethods.Any(m => method.Contains(m, StringComparison.OrdinalIgnoreCase));
    }
}

