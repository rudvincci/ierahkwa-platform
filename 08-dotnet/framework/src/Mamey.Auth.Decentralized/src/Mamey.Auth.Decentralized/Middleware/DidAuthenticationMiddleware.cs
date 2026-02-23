using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Auth.Decentralized.Core;
using Mamey.Auth.Decentralized.Handlers;
using Mamey.Auth.Decentralized.Options;
using System.Security.Claims;
using System.Text.Json;

namespace Mamey.Auth.Decentralized.Middleware;

/// <summary>
/// Middleware for handling DID-based authentication.
/// </summary>
public class DidAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DidAuthenticationMiddleware> _logger;
    private readonly DecentralizedOptions _options;

    public DidAuthenticationMiddleware(
        RequestDelegate next,
        ILogger<DidAuthenticationMiddleware> logger,
        IOptions<DecentralizedOptions> options)
    {
        _next = next;
        _logger = logger;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Check if DID authentication is required for this request
            if (!ShouldAuthenticate(context))
            {
                await _next(context);
                return;
            }

            // Extract DID from various sources
            var did = ExtractDidFromRequest(context);
            if (string.IsNullOrEmpty(did))
            {
                _logger.LogDebug("No DID found in request");
                await _next(context);
                return;
            }

            // Validate and resolve the DID
            var decentralizedHandler = context.RequestServices.GetRequiredService<IDecentralizedHandler>();
            var resolutionResult = await decentralizedHandler.ResolveDidAsync(did);
            
            if (!resolutionResult.IsSuccessful || resolutionResult.DidDocument == null)
            {
                _logger.LogWarning("DID document not found for DID: {Did}", did);
                await _next(context);
                return;
            }

            // Create authentication result
            var authenticationResult = await CreateAuthenticationResultAsync(context, did, resolutionResult.DidDocument);
            if (authenticationResult != null)
            {
                context.User = authenticationResult;
                _logger.LogDebug("DID authentication successful for: {Did}", did);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during DID authentication");
        }

        await _next(context);
    }

    private bool ShouldAuthenticate(HttpContext context)
    {
        // Skip authentication for certain paths
        var path = context.Request.Path.Value?.ToLowerInvariant();
        if (string.IsNullOrEmpty(path))
            return false;

        // Skip for health checks, swagger, etc.
        if (path.StartsWith("/health") || 
            path.StartsWith("/swagger") || 
            path.StartsWith("/metrics"))
            return false;

        // Check if the endpoint requires DID authentication
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<RequireDidAuthenticationAttribute>() != null)
            return true;

        // Check if DID authentication is enabled globally
        return _options.EnableDidAuthentication;
    }

    private string? ExtractDidFromRequest(HttpContext context)
    {
        // Try to extract DID from various sources in order of preference

        // 1. From Authorization header (Bearer DID)
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            if (IsValidDid(token))
                return token;
        }

        // 2. From X-DID header
        var didHeader = context.Request.Headers["X-DID"].FirstOrDefault();
        if (!string.IsNullOrEmpty(didHeader) && IsValidDid(didHeader))
            return didHeader;

        // 3. From query parameter
        var didQuery = context.Request.Query["did"].FirstOrDefault();
        if (!string.IsNullOrEmpty(didQuery) && IsValidDid(didQuery))
            return didQuery;

        // 4. From custom header
        var customHeader = context.Request.Headers[_options.DidHeaderName].FirstOrDefault();
        if (!string.IsNullOrEmpty(customHeader) && IsValidDid(customHeader))
            return customHeader;

        return null;
    }

    private bool IsValidDid(string value)
    {
        try
        {
            var did = Did.Parse(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<ClaimsPrincipal?> CreateAuthenticationResultAsync(
        HttpContext context, 
        string did, 
        DidDocument didDocument)
    {
        try
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, did),
                new("did", did),
                new(ClaimTypes.AuthenticationMethod, "DID")
            };

            // Add verification methods as claims
            if (didDocument.VerificationMethod != null)
            {
                foreach (var vm in didDocument.VerificationMethod)
                {
                    claims.Add(new Claim("verification_method", vm.Id));
                    claims.Add(new Claim("verification_method_type", vm.Type));
                }
            }

            // Add services as claims
            if (didDocument.Service != null)
            {
                foreach (var service in didDocument.Service)
                {
                    claims.Add(new Claim("service", service.Id));
                    claims.Add(new Claim("service_type", service.Type));
                }
            }

            // Add custom claims from DID document
            // Note: DidDocument doesn't have AdditionalProperties, so we'll skip this for now
            // This could be implemented if needed by adding AdditionalProperties to DidDocument

            var identity = new ClaimsIdentity(claims, "DID");
            var principal = new ClaimsPrincipal(identity);

            // Set authentication scheme
            context.User = principal;

            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating authentication result for DID: {Did}", did);
            return null;
        }
    }
}

/// <summary>
/// Attribute to mark endpoints that require DID authentication.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequireDidAuthenticationAttribute : Attribute
{
    public RequireDidAuthenticationAttribute()
    {
    }
}
