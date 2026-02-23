using System.Collections;
using System.Security.Claims;
using Mamey.Auth.DecentralizedIdentifiers.VC;
using Mamey.Auth.DecentralizedIdentifiers.Handlers;
using Mamey.Auth.DecentralizedIdentifiers.Services;
using Mamey.Auth.DecentralizedIdentifiers.Caching;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Audit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mamey.Auth.DecentralizedIdentifiers.Middlewares;

/// <summary>
/// Enhanced middleware that extracts and validates DID tokens and VC/VP from HTTP requests, 
/// and sets user claims. Supports both JWT-like DID tokens and Verifiable Presentations.
/// </summary>
public class DidAuthMiddleware : IMiddleware
{
    private readonly ILogger<DidAuthMiddleware> _logger;
    private readonly IVerifiableCredentialValidator _vcValidator;
    private readonly IDidHandler _didHandler;
    private readonly IAccessTokenService _accessTokenService;
    private readonly IDidDocumentCache _didCache;
    private readonly IDidResolver _didResolver;
    private readonly IDidAuditService _auditService;
    private readonly DidAuthOptions _options;

    public DidAuthMiddleware(
        ILogger<DidAuthMiddleware> logger,
        IVerifiableCredentialValidator vcValidator,
        IDidHandler didHandler,
        IAccessTokenService accessTokenService,
        IDidDocumentCache didCache,
        IDidResolver didResolver,
        IDidAuditService auditService,
        IOptions<DidAuthOptions> options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _vcValidator = vcValidator ?? throw new ArgumentNullException(nameof(vcValidator));
        _didHandler = didHandler ?? throw new ArgumentNullException(nameof(didHandler));
        _accessTokenService = accessTokenService ?? throw new ArgumentNullException(nameof(accessTokenService));
        _didCache = didCache ?? throw new ArgumentNullException(nameof(didCache));
        _didResolver = didResolver ?? throw new ArgumentNullException(nameof(didResolver));
        _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Check if this endpoint allows anonymous access
        if (IsAnonymousEndpoint(context))
        {
            await next(context);
            return;
        }

        try
        {
            // 1. Try to extract DID token from Authorization header or cookie
            var didToken = ExtractDidToken(context);
            
            if (!string.IsNullOrEmpty(didToken))
            {
                var authenticated = await AuthenticateWithDidToken(context, didToken);
                if (authenticated)
                {
                    await next(context);
                    return;
                }
            }

            // 2. Try to extract Verifiable Presentation
            var vpToken = ExtractVerifiablePresentation(context);
            
            if (!string.IsNullOrEmpty(vpToken))
            {
                var authenticated = await AuthenticateWithVerifiablePresentation(context, vpToken);
                if (authenticated)
                {
                    await next(context);
                    return;
                }
            }

            // 3. Try legacy VC/VP validation
            var legacyToken = ExtractLegacyVcToken(context);
            
            if (!string.IsNullOrEmpty(legacyToken))
            {
                var authenticated = await AuthenticateWithLegacyVc(context, legacyToken);
                if (authenticated)
                {
                    await next(context);
                    return;
                }
            }

            // No valid authentication found
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "Authentication required" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during DID authentication");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new { error = "Internal authentication error" });
        }
    }

    private bool IsAnonymousEndpoint(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;
        return _options.AllowAnonymousEndpoints.Any(endpoint => 
            path.Contains(endpoint.ToLowerInvariant()));
    }

    private string ExtractDidToken(HttpContext context)
    {
        // Try Authorization header first
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            return authHeader.Substring("Bearer ".Length);
        }

        // Try cookie
        if (context.Request.Cookies.TryGetValue(_options.CookieOptions.Name, out var cookieToken))
        {
            return cookieToken;
        }

        return null;
    }

    private string ExtractVerifiablePresentation(HttpContext context)
    {
        return context.Request.Headers["X-VC-Presentation"].FirstOrDefault() ??
               context.Request.Headers["X-VP"].FirstOrDefault();
    }

    private string ExtractLegacyVcToken(HttpContext context)
    {
        return context.Request.Headers["X-VC"].FirstOrDefault() ??
            context.Request.Headers["Authorization"].FirstOrDefault()
                ?.Replace("DIDVC ", "", StringComparison.OrdinalIgnoreCase);
    }

    private async Task<bool> AuthenticateWithDidToken(HttpContext context, string token)
    {
        var correlationId = context.TraceIdentifier;
        var requestId = context.Request.Headers["X-Request-ID"].FirstOrDefault() ?? correlationId;
        
        try
        {
            // Validate DID token
            var isValid = await _didHandler.ValidateDidToken(token);
            if (!isValid)
            {
                _logger.LogWarning("Invalid DID token");
                
                // Log failed authentication attempt
                await _auditService.LogAuthenticationEventAsync(
                    DidAuditEventTypes.DID_AUTHENTICATION,
                    DidAuditCategories.AUTHENTICATION,
                    DidAuditStatus.FAILURE,
                    "Unknown",
                    correlationId,
                    metadata: new Dictionary<string, object>
                    {
                        ["reason"] = "Invalid DID token",
                        ["requestId"] = requestId,
                        ["userAgent"] = context.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown"
                    });
                
                return false;
            }

            // Extract payload
            var payload = await _didHandler.GetDidPayload(token);
            if (payload == null)
            {
                _logger.LogWarning("Could not extract DID token payload");
                
                // Log failed authentication attempt
                await _auditService.LogAuthenticationEventAsync(
                    DidAuditEventTypes.DID_AUTHENTICATION,
                    DidAuditCategories.AUTHENTICATION,
                    DidAuditStatus.FAILURE,
                    "Unknown",
                    correlationId,
                    metadata: new Dictionary<string, object>
                    {
                        ["reason"] = "Could not extract DID token payload",
                        ["requestId"] = requestId,
                        ["userAgent"] = context.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown"
                    });
                
                return false;
            }

            // Create claims
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, payload.Subject),
                new("sub", payload.Subject),
                new("role", payload.Role)
            };

            // Add custom claims
            foreach (var claim in payload.Claims)
            {
                foreach (var value in claim.Value)
                {
                    claims.Add(new Claim(claim.Key, value));
                }
            }

            // Set user principal
            var identity = new ClaimsIdentity(claims, "DidAuth");
            context.User = new ClaimsPrincipal(identity);

            _logger.LogDebug("Successfully authenticated with DID token for {Did}", payload.Subject);
            
            // Log successful authentication
            await _auditService.LogAuthenticationEventAsync(
                DidAuditEventTypes.DID_AUTHENTICATION,
                DidAuditCategories.AUTHENTICATION,
                DidAuditStatus.SUCCESS,
                payload.Subject,
                correlationId,
                metadata: new Dictionary<string, object>
                {
                    ["did"] = payload.Subject,
                    ["role"] = payload.Role,
                    ["requestId"] = requestId,
                    ["userAgent"] = context.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown",
                    ["endpoint"] = context.Request.Path.Value ?? "Unknown"
                });
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating with DID token");
            
            // Log failed authentication attempt
            await _auditService.LogAuthenticationEventAsync(
                DidAuditEventTypes.DID_AUTHENTICATION,
                DidAuditCategories.AUTHENTICATION,
                DidAuditStatus.FAILURE,
                "Unknown",
                correlationId,
                metadata: new Dictionary<string, object>
                {
                    ["reason"] = "Exception during authentication",
                    ["error"] = ex.Message,
                    ["requestId"] = requestId,
                    ["userAgent"] = context.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown"
                });
            
            return false;
        }
    }

    private async Task<bool> AuthenticateWithVerifiablePresentation(HttpContext context, string vpToken)
    {
        try
        {
            // Parse VP
            var vp = System.Text.Json.JsonSerializer.Deserialize<VerifiablePresentation>(vpToken);
            if (vp == null)
            {
                _logger.LogWarning("Could not parse Verifiable Presentation");
                return false;
            }

            // Validate VP
            var validationResult = await _didHandler.ValidatePresentation(vp);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Invalid Verifiable Presentation: {Errors}", 
                    string.Join(", ", validationResult.Errors));
                return false;
            }

            // Create claims from VP
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, vp.Holder),
                new("sub", vp.Holder),
                new("vc:presentation", "true")
            };

            // Extract claims from credentials
            if (vp.VerifiableCredential != null)
            {
                var credentials = vp.VerifiableCredential is IEnumerable<VerifiableCredential> credentialList 
                    ? credentialList 
                    : new[] { (VerifiableCredential)vp.VerifiableCredential };

                foreach (var credential in credentials)
                {
                    if (credential.Issuer != null)
                    {
                        var issuerString = credential.Issuer is string issuer ? issuer : credential.Issuer.ToString();
                        if (!string.IsNullOrEmpty(issuerString))
                            claims.Add(new Claim("vc:issuer", issuerString));
                    }

                    if (credential.Type != null)
                    {
                        if (credential.Type is IEnumerable<string> typeList)
                        {
                            foreach (var type in typeList)
                            {
                                claims.Add(new Claim("vc:type", type));
                            }
                        }
                        else
                        {
                            claims.Add(new Claim("vc:type", credential.Type.ToString()));
                        }
                    }

                    // Extract credential subject claims
                    if (credential.CredentialSubject is Dictionary<string, object> credentialSubject)
                    {
                        foreach (var subject in credentialSubject)
                        {
                            if (subject.Value != null)
                            {
                                claims.Add(new Claim($"vc:subject:{subject.Key}", subject.Value.ToString()));
                            }
                        }
                    }
                }
            }

            // Set user principal
            var identity = new ClaimsIdentity(claims, "DidAuth");
            context.User = new ClaimsPrincipal(identity);

            _logger.LogDebug("Successfully authenticated with Verifiable Presentation for {Holder}", vp.Holder);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating with Verifiable Presentation");
            return false;
        }
    }

    private async Task<bool> AuthenticateWithLegacyVc(HttpContext context, string vcToken)
        {
            try
            {
                var validationResult = await _vcValidator.ValidateAsync(vcToken);

                if (validationResult.IsValid)
                {
                // Map VC claims to ClaimsPrincipal
                    var claims = validationResult.Claims.ToList();

                    // Standardize some common claim types for easier policy writing:
                    if (!string.IsNullOrEmpty(validationResult.CredentialType))
                        claims.Add(new Claim("vc:type", validationResult.CredentialType));
                    if (!string.IsNullOrEmpty(validationResult.IssuerDid))
                        claims.Add(new Claim("vc:issuer", validationResult.IssuerDid));
                    if (!string.IsNullOrEmpty(validationResult.SubjectDid))
                        claims.Add(new Claim("vc:subject", validationResult.SubjectDid));

                    // Mark if this was a VP (Presentation)
                    if (vcToken.Contains("VerifiablePresentation"))
                        claims.Add(new Claim("vc:presentation", "true"));

                    // Optionally add status/revocation
                    if (validationResult.IsRevoked)
                        claims.Add(new Claim("vc:revoked", "true"));

                    var identity = new ClaimsIdentity(claims, "DidAuth");
                    context.User = new ClaimsPrincipal(identity);

                    _logger.LogDebug("DID/VC claims successfully loaded into HttpContext.User.");
                return true;
                }
                else
                {
                    _logger.LogWarning("Invalid VC/VP: {Errors}",
                    string.Join("; ", validationResult.Errors ?? new List<string>()));
                return false;
            }
            }
            catch (Exception ex)
            {
            _logger.LogError(ex, "Error during legacy VC/VP validation.");
            return false;
        }
    }
}