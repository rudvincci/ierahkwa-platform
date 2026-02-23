using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Mamey.Auth.Identity.Middleware;

/// <summary>
/// Picks an authentication scheme per request based on:
/// 1) Endpoint metadata ( <see cref="AuthSchemeAttribute"/> or <see cref="RequiredAuthProtocolAttribute"/> )
/// 2) Explicit query/header override (auth-scheme / X-Auth-Scheme)
/// 3) Heuristics: Authorization header (Bearer), client cert, cookies, DID headers, etc.
/// It then authenticates with that scheme and sets <see cref="HttpContext.User"/>.
/// </summary>
public sealed class AuthenticationProtocolResolverMiddleware : IMiddleware
{
    // Adjust to whatever you registered in AddAuthentication()
    public const string CookieScheme      = "AppCookie";
    public const string JwtScheme         = JwtBearerDefaults.AuthenticationScheme;
    // public const string CertScheme        = CertificateAuthenticationDefaults.AuthenticationScheme;
    public const string DidScheme         = "DidAuth";          // your custom DID handler
    public const string HeaderOverrideKey = "X-Auth-Scheme";
    public const string QueryOverrideKey  = "auth-scheme";

    private readonly IAuthenticationService _auth;
    private readonly ILogger<AuthenticationProtocolResolverMiddleware> _log;

    public AuthenticationProtocolResolverMiddleware(
        IAuthenticationService auth,
        ILogger<AuthenticationProtocolResolverMiddleware> log)
    {
        _auth = auth ?? throw new ArgumentNullException(nameof(auth));
        _log  = log  ?? throw new ArgumentNullException(nameof(log));
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Already authenticated? skip (e.g., external middleware)
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            await next(context);
            return;
        }

        var scheme = ResolveScheme(context);
        if (scheme is null)
        {
            _log.LogDebug("No scheme resolved, letting default auth middleware handle it.");
            await next(context);
            return;
        }

        var result = await _auth.AuthenticateAsync(context, scheme);
        if (result.Succeeded && result.Principal is not null)
        {
            context.User = result.Principal;
            _log.LogDebug("Authenticated with scheme {Scheme}", scheme);
        }
        else
        {
            // Don't short-circuit here; let AuthorizationMiddleware challenge
            _log.LogDebug("Authentication failed for scheme {Scheme}. Error: {Error}", scheme, result.Failure?.Message);
        }

        // Stash scheme for later (optional)
        context.Items["auth.scheme"] = scheme;

        await next(context);
    }

    private static string? ResolveScheme(HttpContext ctx)
    {
        // 1) Endpoint metadata
        var ep = ctx.GetEndpoint();
        if (ep != null)
        {
            if (ep.Metadata.GetMetadata<AuthSchemeAttribute>() is { } attr && !string.IsNullOrWhiteSpace(attr.Scheme))
                return attr.Scheme;

            if (ep.Metadata.GetMetadata<RequiredAuthProtocolAttribute>() is { } protoAttr)
                return MapProtocolToScheme(protoAttr.Protocol);
        }

        // 2) Override via query/header
        if (ctx.Request.Query.TryGetValue(QueryOverrideKey, out var qs) && qs.Count > 0)
            return qs[0];

        if (ctx.Request.Headers.TryGetValue(HeaderOverrideKey, out var hs) && hs.Count > 0)
            return hs[0];

        // 3) Heuristics
        // Bearer token?
        if (ctx.Request.Headers.TryGetValue("Authorization", out var authz)
            && authz.Any(v => v.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)))
            return JwtScheme;

        // DID auth header?
        if (ctx.Request.Headers.ContainsKey("X-DID") || ctx.Request.Headers.ContainsKey("DID"))
            return DidScheme;

        // // Client certificate?
        // if (ctx.Connection.ClientCertificate is not null)
        //     return CertScheme;

        // Cookie present?
        if (ctx.Request.Cookies.Count > 0 &&
            ctx.Request.Cookies.Keys.Any(k => k.Contains("auth", StringComparison.OrdinalIgnoreCase)))
            return CookieScheme;

        return null;
    }

    private static string MapProtocolToScheme(AuthProtocol protocol) =>
        protocol switch
        {
            AuthProtocol.Cookie      => CookieScheme,
            AuthProtocol.Jwt         => JwtScheme,
            // AuthProtocol.Certificate => CertScheme,
            AuthProtocol.Did         => DidScheme,
            _                        => CookieScheme
        };
}

/// <summary>Marks an endpoint with a concrete authentication scheme name.</summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public sealed class AuthSchemeAttribute : Attribute
{
    public AuthSchemeAttribute(string scheme) => Scheme = scheme;
    public string Scheme { get; }
}

/// <summary>Marks an endpoint with a higher-level protocol enum.</summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public sealed class RequiredAuthProtocolAttribute : Attribute
{
    public RequiredAuthProtocolAttribute(AuthProtocol protocol) => Protocol = protocol;
    public AuthProtocol Protocol { get; }
}

public enum AuthProtocol
{
    Cookie,
    Jwt,
    Certificate,
    Did
}
