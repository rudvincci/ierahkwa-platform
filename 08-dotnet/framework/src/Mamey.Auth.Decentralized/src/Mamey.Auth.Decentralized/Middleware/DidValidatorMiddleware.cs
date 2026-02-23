using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Auth.Decentralized.Core;
using Mamey.Auth.Decentralized.Handlers;
using Mamey.Auth.Decentralized.Options;
using Mamey.Auth.Decentralized.Validation;
using System.Text.Json;

namespace Mamey.Auth.Decentralized.Middleware;

/// <summary>
/// Middleware for validating DID documents and requests.
/// </summary>
public class DidValidatorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DidValidatorMiddleware> _logger;
    private readonly DecentralizedOptions _options;

    public DidValidatorMiddleware(
        RequestDelegate next,
        ILogger<DidValidatorMiddleware> logger,
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
            // Check if validation is required for this request
            if (!ShouldValidate(context))
            {
                await _next(context);
                return;
            }

            // Validate DID in request
            var validationResult = await ValidateRequestAsync(context);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("DID validation failed: {Errors}", string.Join(", ", validationResult.Errors));
                
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";
                
                var errorResponse = new
                {
                    error = "DID_VALIDATION_FAILED",
                    message = "DID validation failed",
                    details = validationResult.Errors
                };
                
                await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
                return;
            }

            // Add validation metadata to context
            context.Items["DidValidationResult"] = validationResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during DID validation");
            
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            
            var errorResponse = new
            {
                error = "VALIDATION_ERROR",
                message = "An error occurred during DID validation"
            };
            
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
            return;
        }

        await _next(context);
    }

    private bool ShouldValidate(HttpContext context)
    {
        // Skip validation for certain paths
        var path = context.Request.Path.Value?.ToLowerInvariant();
        if (string.IsNullOrEmpty(path))
            return false;

        // Skip for health checks, swagger, etc.
        if (path.StartsWith("/health") || 
            path.StartsWith("/swagger") || 
            path.StartsWith("/metrics"))
            return false;

        // Check if the endpoint requires DID validation
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<RequireDidValidationAttribute>() != null)
            return true;

        // Check if DID validation is enabled globally
        return _options.EnableDidValidation;
    }

    private async Task<DidValidationResult> ValidateRequestAsync(HttpContext context)
    {
        var result = new DidValidationResult();

        try
        {
            // Extract DID from request
            var did = ExtractDidFromRequest(context);
            if (string.IsNullOrEmpty(did))
            {
                result.Errors.Add("No DID found in request");
                return result;
            }

            // Validate DID format
            if (!IsValidDidFormat(did))
            {
                result.Errors.Add($"Invalid DID format: {did}");
                return result;
            }

            // Resolve and validate DID document
            var decentralizedHandler = context.RequestServices.GetRequiredService<IDecentralizedHandler>();
            var resolutionResult = await decentralizedHandler.ResolveDidAsync(did);
            
            if (!resolutionResult.IsSuccessful || resolutionResult.DidDocument == null)
            {
                result.Errors.Add($"DID document not found for: {did}");
                return result;
            }

            // Validate DID document structure
            var documentValidation = await ValidateDidDocumentAsync(resolutionResult.DidDocument);
            if (!documentValidation.IsValid)
            {
                result.Errors.AddRange(documentValidation.Errors);
                return result;
            }

            // Validate request-specific requirements
            var requestValidation = await ValidateRequestSpecificAsync(context, did, resolutionResult.DidDocument);
            if (!requestValidation.IsValid)
            {
                result.Errors.AddRange(requestValidation.Errors);
                return result;
            }

            result.IsValid = true;
            result.Did = did;
            result.DidDocument = resolutionResult.DidDocument;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during DID validation");
            result.Errors.Add($"Validation error: {ex.Message}");
        }

        return result;
    }

    private string? ExtractDidFromRequest(HttpContext context)
    {
        // Try to extract DID from various sources in order of preference

        // 1. From Authorization header (Bearer DID)
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            if (IsValidDidFormat(token))
                return token;
        }

        // 2. From X-DID header
        var didHeader = context.Request.Headers["X-DID"].FirstOrDefault();
        if (!string.IsNullOrEmpty(didHeader) && IsValidDidFormat(didHeader))
            return didHeader;

        // 3. From query parameter
        var didQuery = context.Request.Query["did"].FirstOrDefault();
        if (!string.IsNullOrEmpty(didQuery) && IsValidDidFormat(didQuery))
            return didQuery;

        // 4. From custom header
        var customHeader = context.Request.Headers[_options.DidHeaderName].FirstOrDefault();
        if (!string.IsNullOrEmpty(customHeader) && IsValidDidFormat(customHeader))
            return customHeader;

        return null;
    }

    private bool IsValidDidFormat(string value)
    {
        try
        {
            Did.Parse(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<DidValidationResult> ValidateDidDocumentAsync(DidDocument didDocument)
    {
        var result = new DidValidationResult();

        try
        {
            // Basic structure validation
            if (string.IsNullOrEmpty(didDocument.Id))
            {
                result.Errors.Add("DID document must have an ID");
            }

            if (didDocument.Context == null || !didDocument.Context.Any())
            {
                result.Errors.Add("DID document must have at least one context");
            }

            // Validate verification methods
            if (didDocument.VerificationMethod != null)
            {
                foreach (var vm in didDocument.VerificationMethod)
                {
                    var vmValidation = ValidateVerificationMethod(vm);
                    if (!vmValidation.IsValid)
                    {
                        result.Errors.AddRange(vmValidation.Errors);
                    }
                }
            }

            // Validate services
            if (didDocument.Service != null)
            {
                foreach (var service in didDocument.Service)
                {
                    var serviceValidation = ValidateServiceEndpoint(service);
                    if (!serviceValidation.IsValid)
                    {
                        result.Errors.AddRange(serviceValidation.Errors);
                    }
                }
            }

            result.IsValid = result.Errors.Count == 0;
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Error validating DID document: {ex.Message}");
        }

        return result;
    }

    private DidValidationResult ValidateVerificationMethod(VerificationMethod vm)
    {
        var result = new DidValidationResult();

        if (string.IsNullOrEmpty(vm.Id))
        {
            result.Errors.Add("Verification method must have an ID");
        }

        if (string.IsNullOrEmpty(vm.Type))
        {
            result.Errors.Add("Verification method must have a type");
        }

        if (string.IsNullOrEmpty(vm.Controller))
        {
            result.Errors.Add("Verification method must have a controller");
        }

        result.IsValid = result.Errors.Count == 0;
        return result;
    }

    private DidValidationResult ValidateServiceEndpoint(ServiceEndpoint service)
    {
        var result = new DidValidationResult();

        if (string.IsNullOrEmpty(service.Id))
        {
            result.Errors.Add("Service endpoint must have an ID");
        }

        if (string.IsNullOrEmpty(service.Type))
        {
            result.Errors.Add("Service endpoint must have a type");
        }

        if (string.IsNullOrEmpty(service.ServiceEndpointUrl))
        {
            result.Errors.Add("Service endpoint must have a URL");
        }

        result.IsValid = result.Errors.Count == 0;
        return result;
    }

    private async Task<DidValidationResult> ValidateRequestSpecificAsync(
        HttpContext context, 
        string did, 
        DidDocument didDocument)
    {
        var result = new DidValidationResult();

        try
        {
            // Check if DID is allowed for this endpoint
            var endpoint = context.GetEndpoint();
            var allowedDids = endpoint?.Metadata?.GetMetadata<AllowedDidsAttribute>()?.Dids;
            if (allowedDids != null && !allowedDids.Contains(did))
            {
                result.Errors.Add($"DID {did} is not allowed for this endpoint");
            }

            // Check if verification method is required
            var requireVerificationMethod = endpoint?.Metadata?.GetMetadata<RequireVerificationMethodAttribute>();
            if (requireVerificationMethod != null)
            {
                var hasRequiredMethod = didDocument.VerificationMethod?.Any(vm => 
                    vm.Type == requireVerificationMethod.Type) ?? false;
                
                if (!hasRequiredMethod)
                {
                    result.Errors.Add($"Required verification method type {requireVerificationMethod.Type} not found");
                }
            }

            result.IsValid = result.Errors.Count == 0;
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Error validating request-specific requirements: {ex.Message}");
        }

        return result;
    }
}

/// <summary>
/// Result of DID validation.
/// </summary>
public class DidValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public string? Did { get; set; }
    public DidDocument? DidDocument { get; set; }
}

/// <summary>
/// Attribute to mark endpoints that require DID validation.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequireDidValidationAttribute : Attribute
{
    public RequireDidValidationAttribute()
    {
    }
}

/// <summary>
/// Attribute to specify allowed DIDs for an endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AllowedDidsAttribute : Attribute
{
    public string[] Dids { get; }

    public AllowedDidsAttribute(params string[] dids)
    {
        Dids = dids;
    }
}

/// <summary>
/// Attribute to require specific verification method types.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequireVerificationMethodAttribute : Attribute
{
    public string Type { get; }

    public RequireVerificationMethodAttribute(string type)
    {
        Type = type;
    }
}
