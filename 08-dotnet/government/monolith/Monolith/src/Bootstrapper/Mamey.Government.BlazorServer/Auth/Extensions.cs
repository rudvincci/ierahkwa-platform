using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Government.BlazorServer.Auth;

namespace Mamey.Government.BlazorServer.Auth;

internal static class Extensions
{
    public static bool AddGovernmentAuthentication(this IServiceCollection services, IConfiguration configuration, Microsoft.AspNetCore.Hosting.IWebHostEnvironment? environment = null)
    {
        services.Configure<GovernmentAuthOptions>(configuration.GetSection("Auth"));

        var authMode = (configuration["Auth:Mode"] ?? "Mock").Trim();
        var useOidc = string.Equals(authMode, "Oidc", StringComparison.OrdinalIgnoreCase);

        if (useOidc)
        {
            var oidc = configuration.GetSection("Auth:Oidc").Get<OidcAuthOptions>() ?? new OidcAuthOptions();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.Name = "mamey.government.auth";
                    options.LoginPath = "/auth/login";
                    options.LogoutPath = "/auth/logout";
                })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = oidc.Authority;
                    options.ClientId = oidc.ClientId;
                    options.ClientSecret = oidc.ClientSecret;
                    options.ResponseType = "code";
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.RequireHttpsMetadata = oidc.RequireHttpsMetadata;
                    options.RemoteAuthenticationTimeout = TimeSpan.FromMinutes(5);

                    var isDevelopment = environment?.IsDevelopment() ?? false;
                    if (isDevelopment)
                    {
                        options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                        options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

                        options.NonceCookie.SameSite = SameSiteMode.Lax;
                        options.NonceCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    }

                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                    options.Scope.Add("roles");   // Request role claims from Authentik
                    options.Scope.Add("tenant");  // Request tenant claim from Authentik

                    options.TokenValidationParameters.NameClaimType =
                        string.IsNullOrWhiteSpace(oidc.NameClaimType) ? "preferred_username" : oidc.NameClaimType;
                    options.TokenValidationParameters.RoleClaimType =
                        string.IsNullOrWhiteSpace(oidc.RoleClaimType) ? "roles" : oidc.RoleClaimType;

                    // Configure post-logout redirect
                    options.SignedOutCallbackPath = "/auth/signout-callback-oidc";
                    options.SignedOutRedirectUri = "/";
                    
                    options.Events = new OpenIdConnectEvents
                    {
                        OnRedirectToIdentityProviderForSignOut = context =>
                        {
                            // Ensure post_logout_redirect_uri is set correctly
                            var postLogoutUri = context.Request.Scheme + "://" + context.Request.Host + "/";
                            context.ProtocolMessage.PostLogoutRedirectUri = postLogoutUri;
                            
                            var logger = context.HttpContext.RequestServices.GetService<ILoggerFactory>()?.CreateLogger("OIDC");
                            logger?.LogInformation("OIDC Logout: Redirecting to IdP, post_logout_redirect_uri={Uri}", postLogoutUri);
                            
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetService<ILoggerFactory>()?.CreateLogger("OIDC");
                            
                            var identity = context.Principal?.Identity as System.Security.Claims.ClaimsIdentity;
                            if (identity is null)
                            {
                                logger?.LogWarning("OnTokenValidated: Identity is null");
                                return Task.CompletedTask;
                            }

                            // Log all received claims for debugging
                            logger?.LogInformation("OnTokenValidated: Received {ClaimCount} claims", identity.Claims.Count());
                            foreach (var claim in identity.Claims)
                            {
                                logger?.LogDebug("  Claim: {Type} = {Value}", claim.Type, claim.Value);
                            }

                            var roleClaimType = identity.RoleClaimType;

                            var roleClaims = identity.Claims
                                .Where(c => c.Type is "roles" or "role" or System.Security.Claims.ClaimTypes.Role)
                                .ToList();

                            static IEnumerable<string> Expand(string value)
                            {
                                value = (value ?? string.Empty).Trim();
                                if (string.IsNullOrWhiteSpace(value)) yield break;

                                if (value.StartsWith("[", StringComparison.Ordinal) && value.EndsWith("]", StringComparison.Ordinal))
                                {
                                    string[]? arr = null;
                                    try { arr = System.Text.Json.JsonSerializer.Deserialize<string[]>(value); } catch { }
                                    if (arr is not null)
                                    {
                                        foreach (var x in arr)
                                        {
                                            var v = (x ?? string.Empty).Trim();
                                            if (!string.IsNullOrWhiteSpace(v)) yield return v;
                                        }
                                        yield break;
                                    }
                                }

                                if (value.Contains(',', StringComparison.Ordinal))
                                {
                                    foreach (var x in value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                                    {
                                        var v = (x ?? string.Empty).Trim();
                                        if (!string.IsNullOrWhiteSpace(v)) yield return v;
                                    }
                                    yield break;
                                }

                                yield return value;
                            }

                            foreach (var c in roleClaims)
                            {
                                identity.TryRemoveClaim(c);
                            }

                            var distinct = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                            foreach (var c in roleClaims)
                            {
                                foreach (var v in Expand(c.Value))
                                {
                                    if (distinct.Add(v))
                                    {
                                        identity.AddClaim(new System.Security.Claims.Claim(roleClaimType, v));
                                    }
                                }
                            }
                            
                            // Log the final roles
                            logger?.LogInformation("OnTokenValidated: Final roles = [{Roles}]", string.Join(", ", distinct));
                            
                            // Log tenant claim
                            var tenantClaim = identity.FindFirst("tenant")?.Value;
                            logger?.LogInformation("OnTokenValidated: Tenant = {Tenant}", tenantClaim ?? "(not set)");

                            return Task.CompletedTask;
                        },
                        OnRemoteFailure = context =>
                        {
                            context.Response.Redirect("/auth/login?error=oidc_remote_failure");
                            context.HandleResponse();
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            context.Response.Redirect("/auth/login?error=oidc_auth_failed");
                            context.HandleResponse();
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization();
        }
        else
        {
            // Mock authentication for development - still needs cookie auth
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.Name = "mamey.government.auth";
                    options.LoginPath = "/auth/login";
                    options.LogoutPath = "/auth/logout";
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                });
            services.AddAuthorization();
        }

        return useOidc;
    }
}
