using System.Collections;
using System.Security.Claims;
using Mamey.Identity.Decentralized.VC;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Mamey.Identity.Decentralized.Middlewares;

/// <summary>
/// Middleware that extracts and validates a VC/VP from HTTP requests, and sets user claims.
/// Looks for a VC/VP in "X-VC", "X-VC-Presentation", or "Authorization" headers.
/// </summary>
public class DidAuthMiddleware : IMiddleware
{
    // private readonly RequestDelegate _next;
    private readonly ILogger<DidAuthMiddleware> _logger;
    private readonly IVerifiableCredentialValidator _vcValidator;

    public DidAuthMiddleware(ILogger<DidAuthMiddleware> logger,
        IVerifiableCredentialValidator vcValidator)
    {
        // _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _vcValidator = vcValidator ?? throw new ArgumentNullException(nameof(vcValidator));
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // 1. Try to find a VC or VP in headers
        var vcToken =
            context.Request.Headers["X-VC"].FirstOrDefault() ??
            context.Request.Headers["X-VC-Presentation"].FirstOrDefault() ??
            context.Request.Headers["Authorization"].FirstOrDefault()
                ?.Replace("DIDVC ", "", StringComparison.OrdinalIgnoreCase);

        if (!string.IsNullOrEmpty(vcToken))
        {
            try
            {
                var validationResult = await _vcValidator.ValidateAsync(vcToken);

                if (validationResult.IsValid)
                {
                    // 2. Map VC claims to ClaimsPrincipal
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
                }
                else
                {
                    _logger.LogWarning("Invalid VC/VP: {Errors}",
                        string.Join("; ", (IEnumerable)validationResult.Errors ?? Array.Empty<string>()));
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new { error = "Invalid VC/VP", validationResult.Errors });
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during VC/VP validation.");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { error = ex.Message });
                return;
            }
        }

        await next(context);
    }
}