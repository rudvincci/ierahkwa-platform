using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;
using MudBlazor.Services;
using Mamey.Persistence.Minio;
using Mamey.Persistence.Minio.Infrastructure.Resilience;
using Mamey.Portal.Citizenship.Application.Services;
using Mamey.Portal.Citizenship.Infrastructure.Storage;
using Mamey.Portal.Shared.Auth;
using Mamey.Portal.Shared.Storage;
using Mamey.Portal.Shared.Storage.DocumentNaming;
using Mamey.Portal.Shared.Storage.Templates;
using Mamey.Portal.Shared.Tenancy;
using Mamey.Portal.Tenant.Infrastructure.Services;
using Mamey.Portal.Web.Auth;
using Mamey.Portal.Web.Data;
using Mamey.Portal.Web.Public;
using Mamey.Portal.Web.Realtime;
using Mamey.Portal.Web.Storage.DocumentNaming;
using Mamey.Portal.Web.Storage.Templates;
using Mamey.Portal.Web.Tenancy;
using Mamey.Portal.Web.ViewModels.Citizenship;
using Mamey.Portal.Web.ViewModels.Gov;
using Mamey.Portal.Web.ViewModels.Public;

namespace Mamey.Portal.Web;

public static class PortalServiceRegistration
{
    public static bool AddPortalWebServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddAuthorization();
        builder.Services.AddSingleton<WeatherForecastService>();
        builder.Services.AddMudServices();
        builder.Services.AddHttpClient();
        builder.Services.AddSignalR();
        builder.Services.AddHttpContextAccessor();

        builder.Services.Configure<PortalAuthOptions>(builder.Configuration.GetSection("Auth"));

        var useOidc = ConfigureAuthentication(builder);

        RegisterDocumentNaming(builder.Services, builder.Configuration);
        RegisterStorage(builder.Services, builder.Configuration);
        RegisterTemplateStores(builder.Services, builder.Configuration);
        RegisterCaching(builder.Services, builder.Configuration);
        RegisterViewModels(builder.Services);
        RegisterRealtime(builder.Services);
        RegisterRateLimiting(builder.Services);

        return useOidc;
    }

    private static bool ConfigureAuthentication(WebApplicationBuilder builder)
    {
        var authMode = (builder.Configuration["Auth:Mode"] ?? "Mock").Trim();
        var useOidc = string.Equals(authMode, "Oidc", StringComparison.OrdinalIgnoreCase);

        if (useOidc)
        {
            var oidc = builder.Configuration.GetSection("Auth:Oidc").Get<OidcAuthOptions>() ?? new OidcAuthOptions();

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.Name = "mamey.portal.auth";
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

                    if (builder.Environment.IsDevelopment())
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

                    options.TokenValidationParameters.NameClaimType =
                        string.IsNullOrWhiteSpace(oidc.NameClaimType) ? "preferred_username" : oidc.NameClaimType;
                    options.TokenValidationParameters.RoleClaimType =
                        string.IsNullOrWhiteSpace(oidc.RoleClaimType) ? "roles" : oidc.RoleClaimType;

                    options.Events = new OpenIdConnectEvents
                    {
                        OnTokenValidated = context =>
                        {
                            var identity = context.Principal?.Identity as System.Security.Claims.ClaimsIdentity;
                            if (identity is null)
                            {
                                return Task.CompletedTask;
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

            builder.Services.AddScoped<ICurrentUserContext, ClaimsCurrentUserContext>();
            builder.Services.AddScoped<ITenantContext, ClaimsTenantContext>();
        }
        else
        {
            // SEC: Mock auth is only allowed in Development environment
            if (!builder.Environment.IsDevelopment())
            {
                throw new InvalidOperationException(
                    "Auth:Mode is not 'Oidc' but environment is not Development. " +
                    "Mock authentication MUST NOT be used in staging/production. " +
                    "Set Auth:Mode=Oidc and configure Auth:Oidc:* settings.");
            }

            builder.Services.AddSingleton<MockSessionStore>();
            builder.Services.AddScoped<MockAuthSession>();
            builder.Services.AddScoped<AuthenticationStateProvider, MockAuthStateProvider>();
            builder.Services.AddScoped<ICurrentUserContext, MockCurrentUserContext>();
            builder.Services.AddScoped<ITenantContext, MockTenantContext>();
        }

        return useOidc;
    }

    private static void RegisterDocumentNaming(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<InMemoryDocumentNamingStore>();
        services.AddScoped<DbDocumentNamingStore>();
        services.AddScoped<IDocumentNamingStore>(sp =>
        {
            var cs = sp.GetRequiredService<IConfiguration>().GetConnectionString("PortalDb");
            return string.IsNullOrWhiteSpace(cs)
                ? sp.GetRequiredService<InMemoryDocumentNamingStore>()
                : sp.GetRequiredService<DbDocumentNamingStore>();
        });
        services.AddSingleton<IDocumentNamingService, DefaultDocumentNamingService>();
    }

    private static void RegisterStorage(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IObjectStorage>(sp =>
        {
            var cfg = sp.GetRequiredService<IConfiguration>();
            var minioEndpoint = cfg["minio:Endpoint"];

            if (!string.IsNullOrWhiteSpace(minioEndpoint))
            {
                sp.GetRequiredService<IOptions<MinioOptions>>();
                return sp.GetRequiredService<MinioObjectStorage>();
            }

            var root = cfg["Storage:LocalRootPath"] ?? "App_Data/storage";
            return new LocalFileObjectStorage(root);
        });

        services.AddOptions<MinioOptions>().Bind(configuration.GetSection("minio"));
        services.AddScoped<IMinioClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MinioOptions>>().Value;
            if (string.IsNullOrWhiteSpace(options.Endpoint))
            {
                return new MinioClient().WithEndpoint("localhost:9000").WithCredentials("minioadmin", "minioadmin").Build();
            }

            var clientBuilder = new MinioClient()
                .WithEndpoint(options.Endpoint)
                .WithCredentials(options.AccessKey, options.SecretKey);

            if (options.UseSSL)
            {
                clientBuilder.WithSSL();
            }

            if (!string.IsNullOrEmpty(options.Region))
            {
                clientBuilder.WithRegion(options.Region);
            }

            if (!string.IsNullOrEmpty(options.SessionToken))
            {
                clientBuilder.WithSessionToken(options.SessionToken);
            }

            return clientBuilder.Build();
        });
        services.AddSingleton<IRetryPolicyExecutor, RetryPolicyExecutor>();
        services.AddScoped<IBucketService, Mamey.Persistence.Minio.Services.BucketService>();
        services.AddScoped<IObjectService, Mamey.Persistence.Minio.Services.ObjectService>();
        services.AddScoped<MinioObjectStorage>();
    }

    private static void RegisterTemplateStores(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<InMemoryDocumentTemplateStore>();
        services.AddScoped<DbDocumentTemplateStore>();
        services.AddScoped<IDocumentTemplateStore>(sp =>
        {
            var cs = sp.GetRequiredService<IConfiguration>().GetConnectionString("PortalDb");
            return string.IsNullOrWhiteSpace(cs)
                ? sp.GetRequiredService<InMemoryDocumentTemplateStore>()
                : sp.GetRequiredService<DbDocumentTemplateStore>();
        });
    }

    private static void RegisterCaching(IServiceCollection services, IConfiguration configuration)
    {
        var redis = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redis))
        {
            services.AddStackExchangeRedisCache(options => options.Configuration = redis);
        }
        else
        {
            services.AddDistributedMemoryCache();
        }
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        services.AddScoped<BecomeCitizenViewModel>();
        services.AddScoped<ValidateCitizenViewModel>();
        services.AddScoped<ApplicationStatusViewModel>();
        services.AddScoped<TenantsViewModel>();
        services.AddScoped<UserTenantMappingsViewModel>();
        services.AddScoped<TenantInvitesViewModel>();
        services.AddScoped<IPublicValidationApiClient, PublicValidationApiClient>();
    }

    private static void RegisterRealtime(IServiceCollection services)
    {
        services.AddSingleton<ICitizenshipRealtimeNotifier, CitizenshipRealtimeNotifier>();
        services.AddScoped<ICitizenshipRealtimeClient, CitizenshipRealtimeClient>();
    }

    private static void RegisterRateLimiting(IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddFixedWindowLimiter("public-validate", limiterOptions =>
            {
                limiterOptions.PermitLimit = 30;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueLimit = 0;
            });
        });
    }
}
