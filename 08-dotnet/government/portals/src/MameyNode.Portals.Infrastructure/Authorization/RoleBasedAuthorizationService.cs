using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MameyNode.Portals.Infrastructure.Authorization;

/// <summary>
/// Role-based access control service
/// </summary>
public interface IRoleBasedAuthorizationService
{
    /// <summary>
    /// Check if user has required role
    /// </summary>
    Task<bool> HasRoleAsync(string role, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if user has any of the required roles
    /// </summary>
    Task<bool> HasAnyRoleAsync(IEnumerable<string> roles, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if user has all required roles
    /// </summary>
    Task<bool> HasAllRolesAsync(IEnumerable<string> roles, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get user roles
    /// </summary>
    Task<IEnumerable<string>> GetUserRolesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Authorize user for resource
    /// </summary>
    Task<AuthorizationResult> AuthorizeAsync(string resource, string action, CancellationToken cancellationToken = default);
}

/// <summary>
/// Role-based authorization service implementation
/// </summary>
public class RoleBasedAuthorizationService : IRoleBasedAuthorizationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<RoleBasedAuthorizationService> _logger;

    public RoleBasedAuthorizationService(
        IHttpContextAccessor httpContextAccessor,
        IAuthorizationService authorizationService,
        ILogger<RoleBasedAuthorizationService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _authorizationService = authorizationService;
        _logger = logger;
    }

    public async Task<bool> HasRoleAsync(string role, CancellationToken cancellationToken = default)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User == null)
            return false;

        return httpContext.User.IsInRole(role);
    }

    public async Task<bool> HasAnyRoleAsync(IEnumerable<string> roles, CancellationToken cancellationToken = default)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User == null)
            return false;

        return roles.Any(role => httpContext.User.IsInRole(role));
    }

    public async Task<bool> HasAllRolesAsync(IEnumerable<string> roles, CancellationToken cancellationToken = default)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User == null)
            return false;

        return roles.All(role => httpContext.User.IsInRole(role));
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(CancellationToken cancellationToken = default)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User == null)
            return Enumerable.Empty<string>();

        return httpContext.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value);
    }

    public async Task<AuthorizationResult> AuthorizeAsync(string resource, string action, CancellationToken cancellationToken = default)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User == null)
        {
            return AuthorizationResult.Failed();
        }

        var requirement = new OperationAuthorizationRequirement { Name = action };
        var resource_obj = new { Type = resource };
        
        var result = await _authorizationService.AuthorizeAsync(
            httpContext.User,
            resource_obj,
            requirement);

        return result;
    }
}


