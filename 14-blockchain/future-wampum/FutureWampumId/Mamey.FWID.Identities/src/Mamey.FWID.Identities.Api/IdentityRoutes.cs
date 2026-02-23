using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Contracts.DTOs;
using Mamey.FWID.Identities.Contracts.Queries;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Microservice.Infrastructure.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Mamey.FWID.Identities.Api;

/// <summary>
/// Identity service API routes.
/// </summary>
public static class IdentityRoutes
{
    /// <summary>
    /// Maps identity service routes to the dispatcher endpoints builder.
    /// </summary>
    public static IDispatcherEndpointsBuilder AddIdentityRoutes(this IDispatcherEndpointsBuilder endpoints)
    {
        return endpoints?
            // Register Identity (POST /api/identities)
            // Note: This endpoint may be public for self-registration, but requires authentication for security
            .Post<AddIdentity>("/api/identities",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => 
                {
                    ctx.Response.StatusCode = 201;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Identity registered successfully" });
                }, 
                auth: true)  // Requires authentication (identities:write permission)
            
            // Get Identity (GET /api/identities/{id})
            // Requires identities:read permission
            .Get<GetIdentity, IdentityDto>("/api/identities/{identityId:guid}",
                beforeDispatch: (query, ctx) =>
                {
                    // Extract IdentityId from route and set in query using backing field
                    if (ctx.Request.RouteValues.TryGetValue("identityId", out var idValue) && 
                        Guid.TryParse(idValue?.ToString(), out var guid))
                    {
                        // Use reflection to set backing field (records use compiler-generated fields)
                        var field = typeof(GetIdentity).GetFields(
                            System.Reflection.BindingFlags.Instance | 
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("IdentityId", StringComparison.OrdinalIgnoreCase));
                        
                        if (field != null)
                        {
                            field.SetValue(query, new IdentityId(guid));
                        }
                    }
                    return Task.CompletedTask;
                },
                afterDispatch: (query, result, ctx) => ctx.Response.WriteJsonAsync(result),
                auth: true)  // Requires authentication (identities:read permission)
            
            // Find Identities (GET /api/identities)
            // Requires identities:read permission
            .Get<FindIdentities, List<IdentityDto>>("/api/identities",
                afterDispatch: (query, result, ctx) => ctx.Response.WriteJsonAsync(result),
                auth: true)  // Requires authentication (identities:read permission)
            
            // Verify Biometric (POST /api/identities/{id}/verify)
            // Requires identities:verify permission
            .Post<VerifyBiometric>("/api/identities/{identityId:guid}/verify",
                beforeDispatch: ([FromBody] cmd, ctx) =>
                {
                    // Extract IdentityId from route and set in command using backing field
                    if (ctx.Request.RouteValues.TryGetValue("identityId", out var idValue) && 
                        Guid.TryParse(idValue?.ToString(), out var guid))
                    {
                        var field = typeof(VerifyBiometric).GetFields(
                            System.Reflection.BindingFlags.Instance | 
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("IdentityId", StringComparison.OrdinalIgnoreCase));
                        
                        if (field != null)
                        {
                            field.SetValue(cmd, new IdentityId(guid));
                        }
                    }
                    return Task.CompletedTask;
                },
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Biometric verified successfully" });
                },
                auth: true)  // Requires authentication (identities:verify permission)
            
            // Update Biometric (PUT /api/identities/{id}/biometric)
            // Requires identities:write permission
            .Put<UpdateBiometric>("/api/identities/{identityId:guid}/biometric",
                beforeDispatch: ([FromBody] cmd, ctx) =>
                {
                    // Extract IdentityId from route and set in command using backing field
                    if (ctx.Request.RouteValues.TryGetValue("identityId", out var idValue) && 
                        Guid.TryParse(idValue?.ToString(), out var guid))
                    {
                        var field = typeof(UpdateBiometric).GetFields(
                            System.Reflection.BindingFlags.Instance | 
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("IdentityId", StringComparison.OrdinalIgnoreCase));
                        
                        if (field != null)
                        {
                            field.SetValue(cmd, new IdentityId(guid));
                        }
                    }
                    return Task.CompletedTask;
                },
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Biometric updated successfully" });
                },
                auth: true)  // Requires authentication (identities:write permission)
            
            // Revoke Identity (POST /api/identities/{id}/revoke)
            // Requires identities:write or identities:admin permission
            .Post<RevokeIdentity>("/api/identities/{identityId:guid}/revoke",
                beforeDispatch: ([FromBody] cmd, ctx) =>
                {
                    // Extract IdentityId from route and set in command using backing field
                    if (ctx.Request.RouteValues.TryGetValue("identityId", out var idValue) && 
                        Guid.TryParse(idValue?.ToString(), out var guid))
                    {
                        var field = typeof(RevokeIdentity).GetFields(
                            System.Reflection.BindingFlags.Instance | 
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("IdentityId", StringComparison.OrdinalIgnoreCase));
                        
                        if (field != null)
                        {
                            field.SetValue(cmd, new IdentityId(guid));
                        }
                    }
                    return Task.CompletedTask;
                },
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Identity revoked successfully" });
                },
                auth: true)  // Requires authentication (identities:write or identities:admin permission)
            
            // Update Zone (PUT /api/identities/{id}/zone)
            // Requires identities:write permission
            .Put<UpdateZone>("/api/identities/{identityId:guid}/zone",
                beforeDispatch: ([FromBody] cmd, ctx) =>
                {
                    // Extract IdentityId from route and set in command using backing field
                    if (ctx.Request.RouteValues.TryGetValue("identityId", out var idValue) && 
                        Guid.TryParse(idValue?.ToString(), out var guid))
                    {
                        var field = typeof(UpdateZone).GetFields(
                            System.Reflection.BindingFlags.Instance | 
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("IdentityId", StringComparison.OrdinalIgnoreCase));
                        
                        if (field != null)
                        {
                            field.SetValue(cmd, new IdentityId(guid));
                        }
                    }
                    return Task.CompletedTask;
                },
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Zone updated successfully" });
                },
                auth: true)  // Requires authentication (identities:write permission)
            
            // Update Contact Information (PUT /api/identities/{id}/contact)
            // Requires identities:write permission
            .Put<UpdateContactInformation>("/api/identities/{identityId:guid}/contact",
                beforeDispatch: ([FromBody] cmd, ctx) =>
                {
                    // Extract IdentityId from route and set in command using backing field
                    if (ctx.Request.RouteValues.TryGetValue("identityId", out var idValue) && 
                        Guid.TryParse(idValue?.ToString(), out var guid))
                    {
                        var field = typeof(UpdateContactInformation).GetFields(
                            System.Reflection.BindingFlags.Instance | 
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("IdentityId", StringComparison.OrdinalIgnoreCase));
                        
                        if (field != null)
                        {
                            field.SetValue(cmd, new IdentityId(guid));
                        }
                    }
                    return Task.CompletedTask;
                },
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Contact information updated successfully" });
                },
                auth: true)  // Requires authentication (identities:write permission)
            
            // Sync Permissions (POST /api/permissions/sync)
            // Requires identities:admin permission (service-to-service with certificate authentication)
            .Post<SyncPermissions>("/api/permissions/sync",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Permissions synchronized successfully" });
                },
                auth: true)
            // Sign In (POST /api/auth/sign-in)
            // Public endpoint - no authentication required
            .Post<SignIn>("/api/auth/sign-in",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Sign-in successful" });
                },
                auth: false)  // Public endpoint
            
            // Sign In with Biometric (POST /api/auth/sign-in/biometric)
            // Requires authentication (user must be authenticated first)
            .Post<SignInWithBiometric>("/api/auth/sign-in/biometric",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Biometric sign-in successful" });
                },
                auth: true)  // Requires authentication
            
            // Sign Out (POST /api/auth/sign-out)
            // Requires authentication
            .Post<SignOut>("/api/auth/sign-out",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Sign-out successful" });
                },
                auth: true)  // Requires authentication
            
            // Refresh Token (POST /api/auth/refresh)
            // Public endpoint - uses refresh token for authentication
            .Post<RefreshToken>("/api/auth/refresh",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Token refreshed successfully" });
                },
                auth: false)  // Public endpoint (uses refresh token)
            
            // ============================================
            // MULTI-FACTOR AUTHENTICATION ENDPOINTS
            // ============================================
            
            // Setup MFA (POST /api/auth/mfa/setup)
            // Requires authentication
            .Post<SetupMfa>("/api/auth/mfa/setup",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "MFA setup initiated" });
                },
                auth: true)  // Requires authentication
            
            // Enable MFA (POST /api/auth/mfa/enable)
            // Requires authentication
            .Post<EnableMfa>("/api/auth/mfa/enable",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "MFA enabled successfully" });
                },
                auth: true)  // Requires authentication
            
            // Disable MFA (POST /api/auth/mfa/disable)
            // Requires authentication
            .Post<DisableMfa>("/api/auth/mfa/disable",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "MFA disabled successfully" });
                },
                auth: true)  // Requires authentication
            
            // Create MFA Challenge (POST /api/auth/mfa/challenge)
            // Requires authentication
            .Post<CreateMfaChallenge>("/api/auth/mfa/challenge",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "MFA challenge created" });
                },
                auth: true)  // Requires authentication
            
            // Verify MFA Challenge (POST /api/auth/mfa/verify)
            // Requires authentication
            .Post<VerifyMfaChallenge>("/api/auth/mfa/verify",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "MFA challenge verified" });
                },
                auth: true)  // Requires authentication
            
            // Generate Backup Codes (POST /api/auth/mfa/backup-codes)
            // Requires authentication
            .Post<GenerateBackupCodes>("/api/auth/mfa/backup-codes",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Backup codes generated" });
                },
                auth: true)  // Requires authentication
            
            // Verify Backup Code (POST /api/auth/mfa/backup-codes/verify)
            // Requires authentication
            .Post<VerifyBackupCode>("/api/auth/mfa/backup-codes/verify",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Backup code verified" });
                },
                auth: true)  // Requires authentication
            
            // Get MFA Status (GET /api/auth/mfa/status/{identityId})
            // Requires authentication
            .Get<GetIdentityMfaStatus, MfaStatusDto>("/api/auth/mfa/status/{identityId:guid}",
                beforeDispatch: (query, ctx) =>
                {
                    if (ctx.Request.RouteValues.TryGetValue("identityId", out var idValue) &&
                        Guid.TryParse(idValue?.ToString(), out var guid))
                    {
                        var field = typeof(GetIdentityMfaStatus).GetFields(
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("IdentityId", StringComparison.OrdinalIgnoreCase));
                        
                        if (field != null)
                        {
                            field.SetValue(query, guid);
                        }
                    }
                    return Task.CompletedTask;
                },
                afterDispatch: (query, result, ctx) => ctx.Response.WriteJsonAsync(result),
                auth: true)  // Requires authentication
            
            // ============================================
            // PERMISSION ENDPOINTS
            // ============================================
            
            // Create Permission (POST /api/auth/permissions)
            // Requires authentication (permissions:write permission)
            .Post<CreatePermission>("/api/auth/permissions",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 201;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Permission created successfully" });
                },
                auth: true)  // Requires authentication (permissions:write permission)
            
            // Update Permission (PUT /api/auth/permissions/{permissionId})
            // Requires authentication (permissions:write permission)
            .Put<UpdatePermission>("/api/auth/permissions/{permissionId:guid}",
                beforeDispatch: ([FromBody] cmd, ctx) =>
                {
                    if (ctx.Request.RouteValues.TryGetValue("permissionId", out var idValue) &&
                        Guid.TryParse(idValue?.ToString(), out var guid))
                    {
                        var field = typeof(UpdatePermission).GetFields(
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("PermissionId", StringComparison.OrdinalIgnoreCase));
                        
                        if (field != null)
                        {
                            field.SetValue(cmd, guid);
                        }
                    }
                    return Task.CompletedTask;
                },
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Permission updated successfully" });
                },
                auth: true)  // Requires authentication (permissions:write permission)
            
            // Delete Permission (DELETE /api/auth/permissions/{permissionId})
            // Requires authentication (permissions:write permission)
            .Post<DeletePermission>("/api/auth/permissions/{permissionId:guid}/delete",
                beforeDispatch: (cmd, ctx) =>
                {
                    if (ctx.Request.RouteValues.TryGetValue("permissionId", out var idValue) &&
                        Guid.TryParse(idValue?.ToString(), out var guid))
                    {
                        var field = typeof(DeletePermission).GetFields(
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("PermissionId", StringComparison.OrdinalIgnoreCase));
                        
                        if (field != null)
                        {
                            field.SetValue(cmd, guid);
                        }
                    }
                    return Task.CompletedTask;
                },
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Permission deleted successfully" });
                },
                auth: true)  // Requires authentication (permissions:write permission)
            
            // Assign Permission to Identity (POST /api/auth/identities/{identityId}/permissions/{permissionId})
            // Requires authentication (permissions:write permission)
            .Post<AssignPermissionToIdentity>("/api/auth/identities/{identityId:guid}/permissions/{permissionId:guid}",
                beforeDispatch: (cmd, ctx) =>
                {
                    if (ctx.Request.RouteValues.TryGetValue("identityId", out var identityIdValue) &&
                        Guid.TryParse(identityIdValue?.ToString(), out var identityGuid))
                    {
                        var identityField = typeof(AssignPermissionToIdentity).GetFields(
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("IdentityId", StringComparison.OrdinalIgnoreCase));
                        
                        if (identityField != null)
                        {
                            identityField.SetValue(cmd, identityGuid);
                        }
                    }
                    
                    if (ctx.Request.RouteValues.TryGetValue("permissionId", out var permissionIdValue) &&
                        Guid.TryParse(permissionIdValue?.ToString(), out var permissionGuid))
                    {
                        var permissionField = typeof(AssignPermissionToIdentity).GetFields(
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("PermissionId", StringComparison.OrdinalIgnoreCase));
                        
                        if (permissionField != null)
                        {
                            permissionField.SetValue(cmd, permissionGuid);
                        }
                    }
                    return Task.CompletedTask;
                },
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Permission assigned successfully" });
                },
                auth: true)  // Requires authentication (permissions:write permission)
            
            // Remove Permission from Identity (DELETE /api/auth/identities/{identityId}/permissions/{permissionId})
            // Requires authentication (permissions:write permission)
            .Post<RemovePermissionFromIdentity>("/api/auth/identities/{identityId:guid}/permissions/{permissionId:guid}/remove",
                beforeDispatch: (cmd, ctx) =>
                {
                    if (ctx.Request.RouteValues.TryGetValue("identityId", out var identityIdValue) &&
                        Guid.TryParse(identityIdValue?.ToString(), out var identityGuid))
                    {
                        var identityField = typeof(RemovePermissionFromIdentity).GetFields(
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("IdentityId", StringComparison.OrdinalIgnoreCase));
                        
                        if (identityField != null)
                        {
                            identityField.SetValue(cmd, identityGuid);
                        }
                    }
                    
                    if (ctx.Request.RouteValues.TryGetValue("permissionId", out var permissionIdValue) &&
                        Guid.TryParse(permissionIdValue?.ToString(), out var permissionGuid))
                    {
                        var permissionField = typeof(RemovePermissionFromIdentity).GetFields(
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("PermissionId", StringComparison.OrdinalIgnoreCase));
                        
                        if (permissionField != null)
                        {
                            permissionField.SetValue(cmd, permissionGuid);
                        }
                    }
                    return Task.CompletedTask;
                },
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Permission removed successfully" });
                },
                auth: true)  // Requires authentication (permissions:write permission)
            
            // Get Identity Permissions (GET /api/auth/identities/{identityId}/permissions)
            // Requires authentication (permissions:read permission)
            .Get<GetIdentityPermissions, List<Guid>>("/api/auth/identities/{identityId:guid}/permissions",
                beforeDispatch: (query, ctx) =>
                {
                    if (ctx.Request.RouteValues.TryGetValue("identityId", out var idValue) &&
                        Guid.TryParse(idValue?.ToString(), out var guid))
                    {
                        var field = typeof(GetIdentityPermissions).GetFields(
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("IdentityId", StringComparison.OrdinalIgnoreCase));
                        
                        if (field != null)
                        {
                            field.SetValue(query, guid);
                        }
                    }
                    return Task.CompletedTask;
                },
                afterDispatch: (query, result, ctx) => ctx.Response.WriteJsonAsync(result),
                auth: true)  // Requires authentication (permissions:read permission)
            
            // ============================================
            // ROLE ENDPOINTS
            // ============================================
            
            // Create Role (POST /api/auth/roles)
            // Requires authentication (roles:write permission)
            .Post<CreateRole>("/api/auth/roles",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 201;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Role created successfully" });
                },
                auth: true)  // Requires authentication (roles:write permission)
            
            // Update Role (PUT /api/auth/roles/{roleId})
            // Requires authentication (roles:write permission)
            .Put<UpdateRole>("/api/auth/roles/{roleId:guid}",
                beforeDispatch: ([FromBody] cmd, ctx) =>
                {
                    if (ctx.Request.RouteValues.TryGetValue("roleId", out var idValue) &&
                        Guid.TryParse(idValue?.ToString(), out var guid))
                    {
                        var field = typeof(UpdateRole).GetFields(
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("RoleId", StringComparison.OrdinalIgnoreCase));
                        
                        if (field != null)
                        {
                            field.SetValue(cmd, guid);
                        }
                    }
                    return Task.CompletedTask;
                },
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Role updated successfully" });
                },
                auth: true)  // Requires authentication (roles:write permission)
            
            // Delete Role (DELETE /api/auth/roles/{roleId})
            // Requires authentication (roles:write permission)
            .Post<DeleteRole>("/api/auth/roles/{roleId:guid}/delete",
                beforeDispatch: (cmd, ctx) =>
                {
                    if (ctx.Request.RouteValues.TryGetValue("roleId", out var idValue) &&
                        Guid.TryParse(idValue?.ToString(), out var guid))
                    {
                        var field = typeof(DeleteRole).GetFields(
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("RoleId", StringComparison.OrdinalIgnoreCase));
                        
                        if (field != null)
                        {
                            field.SetValue(cmd, guid);
                        }
                    }
                    return Task.CompletedTask;
                },
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Role deleted successfully" });
                },
                auth: true)  // Requires authentication (roles:write permission)
            
            // Assign Role to Identity (POST /api/auth/identities/{identityId}/roles/{roleId})
            // Requires authentication (roles:write permission)
            .Post<AssignRoleToIdentity>("/api/auth/identities/{identityId:guid}/roles/{roleId:guid}",
                beforeDispatch: (cmd, ctx) =>
                {
                    if (ctx.Request.RouteValues.TryGetValue("identityId", out var identityIdValue) &&
                        Guid.TryParse(identityIdValue?.ToString(), out var identityGuid))
                    {
                        var identityField = typeof(AssignRoleToIdentity).GetFields(
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("IdentityId", StringComparison.OrdinalIgnoreCase));
                        
                        if (identityField != null)
                        {
                            identityField.SetValue(cmd, identityGuid);
                        }
                    }
                    
                    if (ctx.Request.RouteValues.TryGetValue("roleId", out var roleIdValue) &&
                        Guid.TryParse(roleIdValue?.ToString(), out var roleGuid))
                    {
                        var roleField = typeof(AssignRoleToIdentity).GetFields(
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("RoleId", StringComparison.OrdinalIgnoreCase));
                        
                        if (roleField != null)
                        {
                            roleField.SetValue(cmd, roleGuid);
                        }
                    }
                    return Task.CompletedTask;
                },
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Role assigned successfully" });
                },
                auth: true)  // Requires authentication (roles:write permission)
            
            // Remove Role from Identity (DELETE /api/auth/identities/{identityId}/roles/{roleId})
            // Requires authentication (roles:write permission)
            .Post<RemoveRoleFromIdentity>("/api/auth/identities/{identityId:guid}/roles/{roleId:guid}/remove",
                beforeDispatch: (cmd, ctx) =>
                {
                    if (ctx.Request.RouteValues.TryGetValue("identityId", out var identityIdValue) &&
                        Guid.TryParse(identityIdValue?.ToString(), out var identityGuid))
                    {
                        var identityField = typeof(RemoveRoleFromIdentity).GetFields(
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("IdentityId", StringComparison.OrdinalIgnoreCase));
                        
                        if (identityField != null)
                        {
                            identityField.SetValue(cmd, identityGuid);
                        }
                    }
                    
                    if (ctx.Request.RouteValues.TryGetValue("roleId", out var roleIdValue) &&
                        Guid.TryParse(roleIdValue?.ToString(), out var roleGuid))
                    {
                        var roleField = typeof(RemoveRoleFromIdentity).GetFields(
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("RoleId", StringComparison.OrdinalIgnoreCase));
                        
                        if (roleField != null)
                        {
                            roleField.SetValue(cmd, roleGuid);
                        }
                    }
                    return Task.CompletedTask;
                },
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Role removed successfully" });
                },
                auth: true)  // Requires authentication (roles:write permission)
            
            // Get Identity Roles (GET /api/auth/identities/{identityId}/roles)
            // Requires authentication (roles:read permission)
            .Get<GetIdentityRoles, List<Guid>>("/api/auth/identities/{identityId:guid}/roles",
                beforeDispatch: (query, ctx) =>
                {
                    if (ctx.Request.RouteValues.TryGetValue("identityId", out var idValue) &&
                        Guid.TryParse(idValue?.ToString(), out var guid))
                    {
                        var field = typeof(GetIdentityRoles).GetFields(
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("IdentityId", StringComparison.OrdinalIgnoreCase));
                        
                        if (field != null)
                        {
                            field.SetValue(query, guid);
                        }
                    }
                    return Task.CompletedTask;
                },
                afterDispatch: (query, result, ctx) => ctx.Response.WriteJsonAsync(result),
                auth: true)  // Requires authentication (roles:read permission)
            
            // Add Permission to Role (POST /api/auth/roles/{roleId}/permissions/{permissionId})
            // Requires authentication (roles:write permission)
            .Post<AddPermissionToRole>("/api/auth/roles/{roleId:guid}/permissions/{permissionId:guid}",
                beforeDispatch: (cmd, ctx) =>
                {
                    if (ctx.Request.RouteValues.TryGetValue("roleId", out var roleIdValue) &&
                        Guid.TryParse(roleIdValue?.ToString(), out var roleGuid))
                    {
                        var roleField = typeof(AddPermissionToRole).GetFields(
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("RoleId", StringComparison.OrdinalIgnoreCase));
                        
                        if (roleField != null)
                        {
                            roleField.SetValue(cmd, roleGuid);
                        }
                    }
                    
                    if (ctx.Request.RouteValues.TryGetValue("permissionId", out var permissionIdValue) &&
                        Guid.TryParse(permissionIdValue?.ToString(), out var permissionGuid))
                    {
                        var permissionField = typeof(AddPermissionToRole).GetFields(
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("PermissionId", StringComparison.OrdinalIgnoreCase));
                        
                        if (permissionField != null)
                        {
                            permissionField.SetValue(cmd, permissionGuid);
                        }
                    }
                    return Task.CompletedTask;
                },
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Permission added to role successfully" });
                },
                auth: true)  // Requires authentication (roles:write permission)
            
            // Remove Permission from Role (DELETE /api/auth/roles/{roleId}/permissions/{permissionId})
            // Requires authentication (roles:write permission)
            .Post<RemovePermissionFromRole>("/api/auth/roles/{roleId:guid}/permissions/{permissionId:guid}/remove",
                beforeDispatch: (cmd, ctx) =>
                {
                    if (ctx.Request.RouteValues.TryGetValue("roleId", out var roleIdValue) &&
                        Guid.TryParse(roleIdValue?.ToString(), out var roleGuid))
                    {
                        var roleField = typeof(RemovePermissionFromRole).GetFields(
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("RoleId", StringComparison.OrdinalIgnoreCase));
                        
                        if (roleField != null)
                        {
                            roleField.SetValue(cmd, roleGuid);
                        }
                    }
                    
                    if (ctx.Request.RouteValues.TryGetValue("permissionId", out var permissionIdValue) &&
                        Guid.TryParse(permissionIdValue?.ToString(), out var permissionGuid))
                    {
                        var permissionField = typeof(RemovePermissionFromRole).GetFields(
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("PermissionId", StringComparison.OrdinalIgnoreCase));
                        
                        if (permissionField != null)
                        {
                            permissionField.SetValue(cmd, permissionGuid);
                        }
                    }
                    return Task.CompletedTask;
                },
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Permission removed from role successfully" });
                },
                auth: true)  // Requires authentication (roles:write permission)
            
            // ============================================
            // EMAIL CONFIRMATION ENDPOINTS
            // ============================================
            
            // Create Email Confirmation (POST /api/auth/confirmations/email)
            // Requires authentication
            .Post<CreateEmailConfirmation>("/api/auth/confirmations/email",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Email confirmation sent" });
                },
                auth: true)  // Requires authentication
            
            // Confirm Email (POST /api/auth/confirmations/email/confirm)
            // Public endpoint - uses token for authentication
            .Post<ConfirmEmail>("/api/auth/confirmations/email/confirm",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Email confirmed successfully" });
                },
                auth: false)  // Public endpoint (uses token)
            
            // Resend Email Confirmation (POST /api/auth/confirmations/email/resend)
            // Requires authentication
            .Post<ResendEmailConfirmation>("/api/auth/confirmations/email/resend",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Email confirmation resent" });
                },
                auth: true)  // Requires authentication
            
            // Get Email Confirmation Status (GET /api/auth/confirmations/email/status/{identityId})
            // Requires authentication
            .Get<GetEmailConfirmationStatus, bool>("/api/auth/confirmations/email/status/{identityId:guid}",
                beforeDispatch: (query, ctx) =>
                {
                    if (ctx.Request.RouteValues.TryGetValue("identityId", out var idValue) &&
                        Guid.TryParse(idValue?.ToString(), out var guid))
                    {
                        var field = typeof(GetEmailConfirmationStatus).GetFields(
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("IdentityId", StringComparison.OrdinalIgnoreCase));
                        
                        if (field != null)
                        {
                            field.SetValue(query, guid);
                        }
                    }
                    return Task.CompletedTask;
                },
                afterDispatch: (query, result, ctx) => ctx.Response.WriteJsonAsync(result),
                auth: true)  // Requires authentication
            
            // ============================================
            // SMS CONFIRMATION ENDPOINTS
            // ============================================
            
            // Create SMS Confirmation (POST /api/auth/confirmations/sms)
            // Requires authentication
            .Post<CreateSmsConfirmation>("/api/auth/confirmations/sms",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "SMS confirmation sent" });
                },
                auth: true)  // Requires authentication
            
            // Confirm SMS (POST /api/auth/confirmations/sms/confirm)
            // Requires authentication
            .Post<ConfirmSms>("/api/auth/confirmations/sms/confirm",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "SMS confirmed successfully" });
                },
                auth: true)  // Requires authentication
            
            // Resend SMS Confirmation (POST /api/auth/confirmations/sms/resend)
            // Requires authentication
            .Post<ResendSmsConfirmation>("/api/auth/confirmations/sms/resend",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: (cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 200;
                    return ctx.Response.WriteAsJsonAsync(new { message = "SMS confirmation resent" });
                },
                auth: true)  // Requires authentication
            
            // Get SMS Confirmation Status (GET /api/auth/confirmations/sms/status/{identityId})
            // Requires authentication
            .Get<GetSmsConfirmationStatus, bool>("/api/auth/confirmations/sms/status/{identityId:guid}",
                beforeDispatch: (query, ctx) =>
                {
                    if (ctx.Request.RouteValues.TryGetValue("identityId", out var idValue) &&
                        Guid.TryParse(idValue?.ToString(), out var guid))
                    {
                        var field = typeof(GetSmsConfirmationStatus).GetFields(
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("IdentityId", StringComparison.OrdinalIgnoreCase));
                        
                        if (field != null)
                        {
                            field.SetValue(query, guid);
                        }
                    }
                    return Task.CompletedTask;
                },
                afterDispatch: (query, result, ctx) => ctx.Response.WriteJsonAsync(result),
                auth: true)  // Requires authentication
            
            // ============================================
            // SESSION ENDPOINTS
            // ============================================
            
            // Get Active Sessions (GET /api/auth/sessions/{identityId})
            // Requires authentication
            .Get<GetActiveSessions, List<SessionDto>>("/api/auth/sessions/{identityId:guid}",
                beforeDispatch: (query, ctx) =>
                {
                    if (ctx.Request.RouteValues.TryGetValue("identityId", out var idValue) &&
                        Guid.TryParse(idValue?.ToString(), out var guid))
                    {
                        var field = typeof(GetActiveSessions).GetFields(
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("IdentityId", StringComparison.OrdinalIgnoreCase));
                        
                        if (field != null)
                        {
                            field.SetValue(query, guid);
                        }
                    }
                    return Task.CompletedTask;
                },
                afterDispatch: (query, result, ctx) => ctx.Response.WriteJsonAsync(result),
                auth: true)  // Requires authentication
            
            // Get Session (GET /api/auth/sessions/{sessionId})
            // Requires authentication
            .Get<GetSession, SessionDto>("/api/auth/sessions/{sessionId:guid}",
                beforeDispatch: (query, ctx) =>
                {
                    if (ctx.Request.RouteValues.TryGetValue("sessionId", out var idValue) &&
                        Guid.TryParse(idValue?.ToString(), out var guid))
                    {
                        var field = typeof(GetSession).GetFields(
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic)
                            .FirstOrDefault(f => f.Name.Contains("SessionId", StringComparison.OrdinalIgnoreCase));
                        
                        if (field != null)
                        {
                            field.SetValue(query, guid);
                        }
                    }
                    return Task.CompletedTask;
                },
                afterDispatch: (query, result, ctx) => ctx.Response.WriteJsonAsync(result),
                auth: true);  // Requires authentication (identities:admin permission via certificate)
    }
}

