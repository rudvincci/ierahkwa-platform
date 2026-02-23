using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Mamey.Auth.Multi.Middlewares;

/// <summary>
/// Middleware for coordinating multi-authentication based on AuthenticationPolicy
/// </summary>
public class MultiAuthMiddleware : IMiddleware
{
    private readonly MultiAuthOptions _options;
    private readonly ILogger<MultiAuthMiddleware> _logger;

    public MultiAuthMiddleware(MultiAuthOptions options, ILogger<MultiAuthMiddleware> logger)
    {
        _options = options;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Skip authentication if no methods are enabled
        if (!_options.EnableJwt && !_options.EnableDid && !_options.EnableAzure && 
            !_options.EnableIdentity && !_options.EnableDistributed && !_options.EnableCertificate)
        {
            await next(context);
            return;
        }

        // Try to authenticate based on policy
        var authenticated = await TryAuthenticateAsync(context);

        if (authenticated)
        {
            _logger.LogDebug("Multi-authentication succeeded for request: {Path}", context.Request.Path);
        }
        else
        {
            _logger.LogDebug("Multi-authentication failed or not attempted for request: {Path}", context.Request.Path);
        }

        await next(context);
    }

    private async Task<bool> TryAuthenticateAsync(HttpContext context)
    {
        switch (_options.Policy)
        {
            case AuthenticationPolicy.JwtOnly:
                if (_options.EnableJwt)
                {
                    return await AuthenticateWithSchemeAsync(context, _options.JwtScheme);
                }
                break;

            case AuthenticationPolicy.DidOnly:
                if (_options.EnableDid)
                {
                    return await AuthenticateWithSchemeAsync(context, _options.DidScheme);
                }
                break;

            case AuthenticationPolicy.AzureOnly:
                if (_options.EnableAzure)
                {
                    // Delegate to Azure coordinator middleware
                    // Azure coordinator will handle its own policy (B2B, B2C, etc.)
                    return await AuthenticateWithSchemeAsync(context, _options.AzureScheme);
                }
                break;

            case AuthenticationPolicy.IdentityOnly:
                if (_options.EnableIdentity)
                {
                    return await AuthenticateWithSchemeAsync(context, _options.IdentityScheme);
                }
                break;

            case AuthenticationPolicy.DistributedOnly:
                if (_options.EnableDistributed)
                {
                    return await AuthenticateWithSchemeAsync(context, _options.DistributedScheme);
                }
                break;

            case AuthenticationPolicy.CertificateOnly:
                if (_options.EnableCertificate)
                {
                    return await AuthenticateWithSchemeAsync(context, _options.CertificateScheme);
                }
                break;

            case AuthenticationPolicy.EitherOr:
                // Try authentication methods in order until one succeeds
                if (_options.EnableJwt)
                {
                    if (await AuthenticateWithSchemeAsync(context, _options.JwtScheme))
                    {
                        return true;
                    }
                }

                if (_options.EnableDid)
                {
                    if (await AuthenticateWithSchemeAsync(context, _options.DidScheme))
                    {
                        return true;
                    }
                }

                if (_options.EnableAzure)
                {
                    // Delegate to Azure coordinator middleware
                    if (await AuthenticateWithSchemeAsync(context, _options.AzureScheme))
                    {
                        return true;
                    }
                }

                if (_options.EnableIdentity)
                {
                    if (await AuthenticateWithSchemeAsync(context, _options.IdentityScheme))
                    {
                        return true;
                    }
                }

                if (_options.EnableDistributed)
                {
                    if (await AuthenticateWithSchemeAsync(context, _options.DistributedScheme))
                    {
                        return true;
                    }
                }

                if (_options.EnableCertificate)
                {
                    if (await AuthenticateWithSchemeAsync(context, _options.CertificateScheme))
                    {
                        return true;
                    }
                }
                break;

            case AuthenticationPolicy.PriorityOrder:
                // Try authentication methods in priority order (JWT first, then DID, then Azure, etc.)
                var methods = new List<(bool enabled, string scheme)>
                {
                    (_options.EnableJwt, _options.JwtScheme),
                    (_options.EnableDid, _options.DidScheme),
                    (_options.EnableAzure, _options.AzureScheme),
                    (_options.EnableIdentity, _options.IdentityScheme),
                    (_options.EnableDistributed, _options.DistributedScheme),
                    (_options.EnableCertificate, _options.CertificateScheme)
                };

                foreach (var (enabled, scheme) in methods)
                {
                    if (enabled)
                    {
                        if (await AuthenticateWithSchemeAsync(context, scheme))
                        {
                            return true;
                        }
                    }
                }
                break;

            case AuthenticationPolicy.AllRequired:
                // All enabled methods must succeed (rare use case)
                var allSucceeded = true;

                if (_options.EnableJwt)
                {
                    allSucceeded = allSucceeded && await AuthenticateWithSchemeAsync(context, _options.JwtScheme);
                }

                if (_options.EnableDid)
                {
                    allSucceeded = allSucceeded && await AuthenticateWithSchemeAsync(context, _options.DidScheme);
                }

                if (_options.EnableAzure)
                {
                    allSucceeded = allSucceeded && await AuthenticateWithSchemeAsync(context, _options.AzureScheme);
                }

                if (_options.EnableIdentity)
                {
                    allSucceeded = allSucceeded && await AuthenticateWithSchemeAsync(context, _options.IdentityScheme);
                }

                if (_options.EnableDistributed)
                {
                    allSucceeded = allSucceeded && await AuthenticateWithSchemeAsync(context, _options.DistributedScheme);
                }

                if (_options.EnableCertificate)
                {
                    allSucceeded = allSucceeded && await AuthenticateWithSchemeAsync(context, _options.CertificateScheme);
                }

                return allSucceeded;
        }

        return false;
    }

    private async Task<bool> AuthenticateWithSchemeAsync(HttpContext context, string scheme)
    {
        try
        {
            // DID authentication is handled by DidAuthMiddleware, not by ASP.NET Core authentication scheme
            // Check if user is already authenticated by DidAuthMiddleware
            if (scheme == "DidBearer" || scheme == _options.DidScheme)
            {
                // DID authentication is handled by custom middleware (DidAuthMiddleware)
                // If user is already authenticated with DID, return true
                if (context.User?.Identity?.IsAuthenticated == true && 
                    context.User?.Identity?.AuthenticationType == "DidAuth")
                {
                    return true;
                }
                
                // Otherwise, DID authentication will be handled by DidAuthMiddleware later in the pipeline
                // Return false here to allow other authentication methods to be tried
                return false;
            }

            var result = await context.AuthenticateAsync(scheme);
            
            if (result.Succeeded && result.Principal != null)
            {
                context.User = result.Principal;
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to authenticate with scheme: {Scheme}", scheme);
            return false;
        }
    }
}

