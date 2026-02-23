using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Mamey.Identity.Jwt.Handlers;
using Mamey.Identity.Jwt.Services;
using Mamey.Modules;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Mamey.Identity.Core;

[assembly: InternalsVisibleTo("Mamey.Identity.Azure")]
namespace Mamey.Identity.Jwt;

public static class Extensions
{
    private const string SectionName = "jwt";
    private const string RegistryName = "auth";
    private const string AccessTokenCookieName = "__access-token";
    private const string AuthorizationHeader = "authorization";

    public static IMameyBuilder AddJwt(this IMameyBuilder builder, string sectionName = SectionName,
        Action<JwtBearerOptions> optionsFactory = null, IList<IModule> modules = null)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        var options = builder.GetOptions<JwtOptions>(sectionName);
        return builder.AddJwt(options, optionsFactory, modules);
    }

    private static IMameyBuilder AddJwt(this IMameyBuilder builder, JwtOptions options,
        Action<JwtBearerOptions> optionsFactory = null, IList<IModule> modules = null)
    {
        if (!builder.TryRegister(RegistryName))
        {
            return builder;
        }
        
        builder.Services.AddSingleton(new CookieOptions
        {
            HttpOnly = options.Cookie.HttpOnly,
            Secure = options.Cookie.Secure,
            SameSite = options.Cookie.SameSite?.ToLowerInvariant() switch
            {
                "strict" => SameSiteMode.Strict,
                "lax" => SameSiteMode.Lax,
                "none" => SameSiteMode.None,
                "unspecified" => SameSiteMode.Unspecified,
                _ => SameSiteMode.Unspecified
            }
        });

        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Services.AddSingleton<IJwtHandler, JwtHandler>();
        if (options.AuthenticationType?.ToLower() == TokenStorageType.Redis.ToString().ToLower())
        {
            builder.Services.AddSingleton<IAccessTokenService, RedisAccessTokenService>();
        }
        else
        {
            builder.Services.AddSingleton<IAccessTokenService, InMemoryAccessTokenService>();
        }

        builder.Services.AddTransient<AccessTokenValidatorMiddleware>();

        if (options.AuthenticationDisabled)
        {
            builder.Services.AddSingleton<IPolicyEvaluator, DisabledAuthenticationPolicyEvaluator>();
        }

        var tokenValidationParameters = new TokenValidationParameters
        {
            RequireAudience = options.RequireAudience,
            ValidIssuer = options.ValidIssuer,
            ValidIssuers = options.ValidIssuers,
            ValidateActor = options.ValidateActor,
            ValidAudience = options.ValidAudience,
            ValidAudiences = options.ValidAudiences,
            ValidateAudience = options.ValidateAudience,
            ValidateIssuer = options.ValidateIssuer,
            ValidateLifetime = options.ValidateLifetime,
            ValidateTokenReplay = options.ValidateTokenReplay,
            ValidateIssuerSigningKey = options.ValidateIssuerSigningKey,
            SaveSigninToken = options.SaveSigninToken,
            RequireExpirationTime = options.RequireExpirationTime,
            RequireSignedTokens = options.RequireSignedTokens,
            ClockSkew = TimeSpan.Zero
        };

        if (!string.IsNullOrWhiteSpace(options.AuthenticationType))
        {
            tokenValidationParameters.AuthenticationType = options.AuthenticationType;
        }

        var hasCertificate = false;
        if (options.Certificate is not null)
        {
            X509Certificate2 certificate = null;
            var password = options.Certificate.Password;
            var hasPassword = !string.IsNullOrWhiteSpace(password);
            if (!string.IsNullOrWhiteSpace(options.Certificate.Location))
            {
                certificate = hasPassword
                    ? new X509Certificate2(options.Certificate.Location, password)
                    : new X509Certificate2(options.Certificate.Location);
                var keyType = certificate.HasPrivateKey ? "with private key" : "with public key only";
                Console.WriteLine($"Loaded X.509 certificate from location: '{options.Certificate.Location}' {keyType}.");
            }
                
            if (!string.IsNullOrWhiteSpace(options.Certificate.RawData))
            {
                var rawData = Convert.FromBase64String(options.Certificate.RawData);
                certificate = hasPassword
                    ? new X509Certificate2(rawData, password)
                    : new X509Certificate2(rawData);
                var keyType = certificate.HasPrivateKey ? "with private key" : "with public key only";
                Console.WriteLine($"Loaded X.509 certificate from raw data {keyType}.");
            }

            if (certificate is not null)
            {
                if (string.IsNullOrWhiteSpace(options.Algorithm))
                {
                    options.Algorithm = SecurityAlgorithms.RsaSha256;
                }

                hasCertificate = true;
                tokenValidationParameters.IssuerSigningKey = new X509SecurityKey(certificate);
                var actionType = certificate.HasPrivateKey ? "issuing" : "validating";
                Console.WriteLine($"Using X.509 certificate for {actionType} tokens.");
            }
        }

        if (!string.IsNullOrWhiteSpace(options.IssuerSigningKey) && !hasCertificate)
        {
            if (string.IsNullOrWhiteSpace(options.Algorithm) || hasCertificate)
            {
                options.Algorithm = SecurityAlgorithms.HmacSha256;
            }

            var rawKey = Encoding.UTF8.GetBytes(options.IssuerSigningKey);
            tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(rawKey);
            Console.WriteLine("Using symmetric encryption for issuing tokens.");
        }

        if (!string.IsNullOrWhiteSpace(options.NameClaimType))
        {
            tokenValidationParameters.NameClaimType = options.NameClaimType;
        }

        if (!string.IsNullOrWhiteSpace(options.RoleClaimType))
        {
            tokenValidationParameters.RoleClaimType = options.RoleClaimType;
        }

        builder.Services
            .AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.Authority = options.Authority;
                o.Audience = options.Audience;
                o.MetadataAddress = options.MetadataAddress;
                o.SaveToken = options.SaveToken;
                o.RefreshOnIssuerKeyNotFound = options.RefreshOnIssuerKeyNotFound;
                o.RequireHttpsMetadata = options.RequireHttpsMetadata;
                o.IncludeErrorDetails = options.IncludeErrorDetails;
                o.TokenValidationParameters = tokenValidationParameters;
                if (!string.IsNullOrWhiteSpace(options.Challenge))
                {
                    o.Challenge = options.Challenge;
                }

                optionsFactory?.Invoke(o);
            });

        builder.Services.AddSingleton(options);
        builder.Services.AddSingleton(options.Cookie);
        builder.Services.AddSingleton(tokenValidationParameters);
        
        var policies = modules?.SelectMany(x => x.Policies ?? Enumerable.Empty<string>()) ??
                       Enumerable.Empty<string>();
        builder.Services.AddAuthorization(authorization =>
        {
            foreach (var policy in policies)
            {
                authorization.AddPolicy(policy, x => x.RequireClaim("permissions", policy));
            }
        });
        return builder;
    }

    public static IApplicationBuilder UseJwt(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.Use(async (ctx, next) =>
        {
            if (ctx.Request.Headers.ContainsKey(AuthorizationHeader))
            {
                ctx.Request.Headers.Remove(AuthorizationHeader);
            }

            if (ctx.Request.Cookies.ContainsKey(AccessTokenCookieName))
            {
                var authenticateResult = await ctx.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
                if (authenticateResult.Succeeded && authenticateResult.Principal is not null)
                {
                    ctx.User = authenticateResult.Principal;
                }
            }

            await next();
        });
        return app;
    }
    // public static IApplicationBuilder UseAccessTokenValidator(this IApplicationBuilder app)
    //     => app.UseMiddleware<AccessTokenValidatorMiddleware>();
}
