using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Mamey.Auth.Azure.Middlewares;

/// <summary>
/// Middleware for coordinating Azure authentication based on AzureAuthenticationPolicy
/// </summary>
public class AzureAuthMiddleware : IMiddleware
{
    private readonly AzureMultiAuthOptions _options;
    private readonly ILogger<AzureAuthMiddleware> _logger;

    public AzureAuthMiddleware(AzureMultiAuthOptions options, ILogger<AzureAuthMiddleware> logger)
    {
        _options = options;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Skip authentication if no Azure methods are enabled
        if (!_options.EnableAzure && !_options.EnableAzureB2B && !_options.EnableAzureB2C)
        {
            await next(context);
            return;
        }

        // Try to authenticate based on policy
        var authenticated = await TryAuthenticateAsync(context);

        if (authenticated)
        {
            _logger.LogDebug("Azure authentication succeeded for request: {Path}", context.Request.Path);
        }
        else
        {
            _logger.LogDebug("Azure authentication failed or not attempted for request: {Path}", context.Request.Path);
        }

        await next(context);
    }

    private async Task<bool> TryAuthenticateAsync(HttpContext context)
    {
        switch (_options.Policy)
        {
            case AzureAuthenticationPolicy.AzureOnly:
                if (_options.EnableAzure)
                {
                    return await AuthenticateWithSchemeAsync(context, _options.AzureScheme);
                }
                break;

            case AzureAuthenticationPolicy.B2BOnly:
                if (_options.EnableAzureB2B)
                {
                    return await AuthenticateWithSchemeAsync(context, _options.AzureB2BScheme);
                }
                break;

            case AzureAuthenticationPolicy.B2COnly:
                if (_options.EnableAzureB2C)
                {
                    return await AuthenticateWithSchemeAsync(context, _options.AzureB2CScheme);
                }
                break;

            case AzureAuthenticationPolicy.EitherOr:
                // Try Azure AD first, then B2B, then B2C
                if (_options.EnableAzure)
                {
                    if (await AuthenticateWithSchemeAsync(context, _options.AzureScheme))
                    {
                        return true;
                    }
                }

                if (_options.EnableAzureB2B)
                {
                    if (await AuthenticateWithSchemeAsync(context, _options.AzureB2BScheme))
                    {
                        return true;
                    }
                }

                if (_options.EnableAzureB2C)
                {
                    if (await AuthenticateWithSchemeAsync(context, _options.AzureB2CScheme))
                    {
                        return true;
                    }
                }
                break;

            case AzureAuthenticationPolicy.AllRequired:
                // All enabled methods must succeed (rare use case)
                var allSucceeded = true;

                if (_options.EnableAzure)
                {
                    allSucceeded = allSucceeded && await AuthenticateWithSchemeAsync(context, _options.AzureScheme);
                }

                if (_options.EnableAzureB2B)
                {
                    allSucceeded = allSucceeded && await AuthenticateWithSchemeAsync(context, _options.AzureB2BScheme);
                }

                if (_options.EnableAzureB2C)
                {
                    allSucceeded = allSucceeded && await AuthenticateWithSchemeAsync(context, _options.AzureB2CScheme);
                }

                return allSucceeded;
        }

        return false;
    }

    private async Task<bool> AuthenticateWithSchemeAsync(HttpContext context, string scheme)
    {
        try
        {
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

