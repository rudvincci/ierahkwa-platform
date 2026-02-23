using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Mamey.WebApi.Security;

namespace Mamey.FWID.Identities.Infrastructure.Security;

/// <summary>
/// Custom permission validator for Identity service that implements permission hierarchy
/// and maps HTTP routes and gRPC methods to required permissions.
/// </summary>
internal sealed class IdentityPermissionValidator : ICertificatePermissionValidator
{
    private readonly ILogger<IdentityPermissionValidator> _logger;
    
    // Permission hierarchy: admin > write > verify > read
    private static readonly Dictionary<string, HashSet<string>> PermissionHierarchy = new()
    {
        { "identities:admin", new HashSet<string> { "identities:admin", "identities:write", "identities:verify", "identities:read", "identities:revoke" } },
        { "identities:write", new HashSet<string> { "identities:write", "identities:verify", "identities:read" } },
        { "identities:verify", new HashSet<string> { "identities:verify", "identities:read" } },
        { "identities:read", new HashSet<string> { "identities:read" } },
        { "identities:revoke", new HashSet<string> { "identities:revoke", "identities:read" } }
    };
    
    // HTTP route to permission mapping
    private static readonly Dictionary<string, HashSet<string>> RoutePermissions = new(StringComparer.OrdinalIgnoreCase)
    {
        { "GET /api/identities", new HashSet<string> { "identities:read" } },
        { "GET /api/identities/{id}", new HashSet<string> { "identities:read" } },
        { "POST /api/identities", new HashSet<string> { "identities:write" } },
        { "POST /api/identities/{id}/verify", new HashSet<string> { "identities:verify" } },
        { "PUT /api/identities/{id}/biometric", new HashSet<string> { "identities:write" } },
        { "POST /api/identities/{id}/revoke", new HashSet<string> { "identities:revoke" } },
        { "PUT /api/identities/{id}/zone", new HashSet<string> { "identities:write" } },
        { "PUT /api/identities/{id}/contact", new HashSet<string> { "identities:write" } },
        { "POST /api/permissions/sync", new HashSet<string> { "identities:admin" } }
    };
    
    // gRPC method to permission mapping
    private static readonly Dictionary<string, HashSet<string>> GrpcMethodPermissions = new(StringComparer.OrdinalIgnoreCase)
    {
        { "VerifyBiometric", new HashSet<string> { "identities:verify" } },
        { "GetBiometric", new HashSet<string> { "identities:read" } },
        { "SyncPermissions", new HashSet<string> { "identities:admin" } },
        { "GetPermissions", new HashSet<string> { "identities:read" } },
        { "UpdatePermissions", new HashSet<string> { "identities:admin" } }
    };

    public IdentityPermissionValidator(ILogger<IdentityPermissionValidator> logger)
    {
        _logger = logger;
    }

    public bool HasAccess(X509Certificate2 certificate, IEnumerable<string> grantedPermissions, HttpContext context)
    {
        if (certificate == null)
        {
            _logger.LogWarning("Certificate is null");
            return false;
        }

        if (grantedPermissions == null || !grantedPermissions.Any())
        {
            _logger.LogWarning("No granted permissions provided for certificate: {Subject}", certificate.Subject);
            return false;
        }

        // Get required permissions from HTTP context
        var requiredPermissions = GetRequiredPermissions(context);
        
        if (requiredPermissions == null || !requiredPermissions.Any())
        {
            // No specific permissions required for this endpoint
            _logger.LogDebug("No specific permissions required for endpoint: {Path}", context.Request.Path);
            return true;
        }

        // Check if any granted permission satisfies the required permissions
        var grantedPermissionsSet = grantedPermissions.ToHashSet(StringComparer.OrdinalIgnoreCase);
        
        foreach (var requiredPermission in requiredPermissions)
        {
            var hasAccess = grantedPermissionsSet.Any(granted => 
                HasPermission(granted, requiredPermission));
            
            if (hasAccess)
            {
                _logger.LogDebug(
                    "Access granted: Certificate {Subject} has permission {RequiredPermission} via {GrantedPermissions}",
                    certificate.Subject, requiredPermission, string.Join(", ", grantedPermissions));
                return true;
            }
        }

        _logger.LogWarning(
            "Access denied: Certificate {Subject} does not have required permissions {RequiredPermissions}. Granted: {GrantedPermissions}",
            certificate.Subject, string.Join(", ", requiredPermissions), string.Join(", ", grantedPermissions));
        
        return false;
    }

    private HashSet<string>? GetRequiredPermissions(HttpContext context)
    {
        // Try to get from route data (set by middleware)
        if (context.Items.TryGetValue("RequiredPermissions", out var permissions) && 
            permissions is HashSet<string> requiredPermissions)
        {
            return requiredPermissions;
        }

        // Extract from HTTP route
        var method = context.Request.Method;
        var path = context.Request.Path.Value ?? string.Empty;
        
        // Normalize path (remove GUIDs and other dynamic parts)
        var normalizedPath = NormalizePath(path);
        var routeKey = $"{method} {normalizedPath}";
        
        if (RoutePermissions.TryGetValue(routeKey, out var routePerms))
        {
            return routePerms;
        }

        // Try gRPC method
        if (context.Request.Headers.TryGetValue("grpc-method", out var grpcMethod))
        {
            var methodName = grpcMethod.ToString();
            if (GrpcMethodPermissions.TryGetValue(methodName, out var grpcPerms))
            {
                return grpcPerms;
            }
        }

        // Check if it's a gRPC request
        if (context.Request.ContentType?.Contains("application/grpc") == true)
        {
            // Extract gRPC method from path
            var grpcPath = path.TrimStart('/');
            var methodName = grpcPath.Split('/').LastOrDefault();
            if (methodName != null && GrpcMethodPermissions.TryGetValue(methodName, out var grpcPathPerms))
            {
                return grpcPathPerms;
            }
        }

        return null;
    }

    private string NormalizePath(string path)
    {
        // Replace GUIDs and IDs with placeholders
        var normalized = path;
        
        // Replace GUID patterns
        normalized = System.Text.RegularExpressions.Regex.Replace(
            normalized, 
            @"[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}", 
            "{id}",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        
        return normalized;
    }

    private bool HasPermission(string grantedPermission, string requiredPermission)
    {
        // Exact match
        if (string.Equals(grantedPermission, requiredPermission, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // Check hierarchy
        if (PermissionHierarchy.TryGetValue(grantedPermission, out var impliedPermissions))
        {
            return impliedPermissions.Contains(requiredPermission, StringComparer.OrdinalIgnoreCase);
        }

        return false;
    }
}

