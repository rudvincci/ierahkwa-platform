using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Mamey.Auth.Identity.Configuration;
using Mamey.Auth.Identity.Jwt;
using Mamey.Auth.Identity.Managers;
using Mamey.Auth.Identity.Middleware;
using Mamey.Auth.Identity.Providers;
using Mamey.Modules;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;

namespace Mamey.Auth.Identity;

public static class Extensions
{
    private const string SectionName = "auth";
    private const string RegistryName = "auth";
    public const string CookieName = "MameyIdentity";

    public static IMameyBuilder AddMameyAuthIdentity(this IMameyBuilder builder, string sectionName = SectionName,
        Action<JwtBearerOptions> optionsFactory = null, IList<IModule> modules = null,
        AuthorizationOptions? authorizationOptions = null, AuthenticationOptions? authenticationOptions = null)
    {
        if (!builder.TryRegister(RegistryName))
            return builder;

        var options = builder.Services.GetOptions<AuthOptions>(sectionName);
        builder.Services.AddSingleton(options);

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Services.AddSingleton<IJwtHandler, JwtHandler>();
        builder.Services
            .ConfigureJwt(builder.Configuration, options)
            .ConfigureCookie(options, out var cookieOptions)
            .ConfigureEFIdentity(builder.Configuration)
            
            .ConfigureServices(builder.Configuration)
            ;

        // // --- ASP.NET Core Identity setup ---
        // builder.Services
        //     .AddIdentityCore<ApplicationUser>(opts =>
        //     {
        //         opts.SignIn.RequireConfirmedAccount = true;
        //         // Password policy
        //         // // // opts.Password.RequireDigit           = true;
        //         // // // opts.Password.RequiredLength         = 8;
        //         // // // opts.Password.RequireNonAlphanumeric = true;
        //         // // // opts.Password.RequireUppercase       = true;
        //         // // // opts.Password.RequireLowercase       = true;
        //
        //         // // // // Lockout policy
        //         // // // opts.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(15);
        //         // // // opts.Lockout.MaxFailedAccessAttempts = 5;
        //         // // // opts.Lockout.AllowedForNewUsers      = true;
        //
        //         // // // // User settings
        //         // // // opts.User.RequireUniqueEmail = true;
        //     })
        //     .AddRoles<ApplicationRole>()
        //     .AddUserManager<MameyUserManager>()
        //     .AddRoleManager<MameyRoleManager>()
        //     .AddSignInManager<MameySignInManager>()
        //     .AddUserStore<MameyUserStore>()
        //     .AddRoleStore<MameyRoleStore>()
        //     .AddClaimsPrincipalFactory<MameyClaimsPrincipalFactory>()
        //     .AddDefaultTokenProviders();

        // --- Configure the single cookie under IdentityConstants.ApplicationScheme ---
        builder.Services
            .AddAuthentication(options =>
            {
                // options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                // options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
                // options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                // options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                // options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                // options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;

            })
            .AddCookie(IdentityConstants.ApplicationScheme, o =>
            {
                o.Cookie.Name = CookieName;
                o.Cookie.HttpOnly = cookieOptions.HttpOnly;
               
                o.Cookie.SameSite = cookieOptions.SameSite;
      
#if DEBUG
                o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                o.Cookie.Domain = null;
#else
                o.Cookie.Domain = cookieOptions.Domain;
                o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
#endif
                o.LoginPath = "/Account/Login";
                o.LogoutPath = "/Account/Logout";
                o.AccessDeniedPath = "/Account/AccessDenied";
                o.ExpireTimeSpan = TimeSpan.FromHours(8);
                o.SlidingExpiration = true;
                o.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = ctx =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
                        {
                            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return Task.CompletedTask;
                        }
                
                        ctx.Response.Redirect(ctx.RedirectUri);
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = ctx =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
                        {
                            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                            return Task.CompletedTask;
                        }
                
                        ctx.Response.Redirect(ctx.RedirectUri);
                        return Task.CompletedTask;
                    }
                };
            })
            .AddJwtBearer(opt =>
             {
        
                 //var jwt = builder.Configuration.GetSection("Identity:Jwt");
                 opt.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidIssuer = options.Issuer,
                     ValidateAudience = true,
                     ValidAudience = options.Audience,
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.IssuerSigningKey)),
                     ValidateLifetime = true,
                     ClockSkew = TimeSpan.FromSeconds(30)
                 };
            
                 opt.Events = new JwtBearerEvents
                 {
                     OnTokenValidated = ctx =>
                     {
                         Console.WriteLine("[JWT] Token validated.");
                         var name = ctx.Principal?.Identity?.Name ?? "(no name)";
                         Console.WriteLine($"[JWT] User: {name}");
                         foreach (var c in ctx.Principal!.Claims)
                             Console.WriteLine($"[JWT] Claim: {c.Type} = {c.Value}");
                         return Task.CompletedTask;
                     },
                     OnMessageReceived = ctx =>
                     {
                         // Inspect incoming Authorization header for debugging
                         // var auth = ctx.Request.Headers.Authorization.ToString();
                         // if (string.IsNullOrEmpty(auth))
                         //     ctx.Response.Headers["x-missing-auth-header"] = "true";
                         return Task.CompletedTask;
                     },
                     OnAuthenticationFailed = ctx =>
                     {
                         // Log details about the failure
                         var ex = ctx.Exception;
                         Console.WriteLine($"[JWT] Authentication failed: {ex.GetType().Name} - {ex.Message}");
                         if (ex is SecurityTokenExpiredException stex)
                         {
                             Console.WriteLine($"[JWT] Token expired at: {stex.Expires}");
                         }

                         ctx.NoResult(); // prevent default response handling
                         ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                         ctx.Response.ContentType = "application/json";
                         return ctx.Response.WriteAsync($"{{\"error\":\"auth_failed\",\"message\":\"{ex.Message}\"}}");
                     },

                     OnChallenge = ctx =>
                     {
                         Console.WriteLine("[JWT] Challenge triggered");
                         Console.WriteLine($"[JWT] Scheme: {ctx.Scheme.Name}");
                         Console.WriteLine($"[JWT] Error: {ctx.Error ?? "(none)"}");
                         Console.WriteLine($"[JWT] Description: {ctx.ErrorDescription ?? "(none)"}");

                         ctx.HandleResponse(); // skip default behavior
                         ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                         ctx.Response.ContentType = "application/json";
                         return ctx.Response.WriteAsync("{\"error\":\"unauthorized\"}");
                     },

                     OnForbidden = ctx =>
                     {
                         Debug.WriteLine("[JWT] Forbidden triggered");
                         if (ctx.Principal != null)
                         {
                             Debug.WriteLine($"[JWT] User: {ctx.Principal.Identity?.Name}");
                             foreach (var claim in ctx.Principal.Claims)
                             {
                                 Debug.WriteLine($"[JWT] Claim: {claim.Type} = {claim.Value}");
                             }
                         }

                         ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                         ctx.Response.ContentType = "application/json";
                         return ctx.Response.WriteAsync("{\"error\":\"forbidden\"}");
                     }
                 };
            
                 optionsFactory?.Invoke(opt);
             });

        builder.Services.AddAuthorization(options =>
        {
            // — Permission‑based policies —  
            // e.g. decorate an API endpoint that reads identity data:
            // [Authorize(Policy = "RequireIdentityRead")]
            options.AddPolicy("RequireIdentityRead",
                p => p.RequireClaim(ClaimCategory.Permission, ClaimValues.Permission.IdentityRead));
            // e.g. protect a write operation on identity entities:
            // [Authorize(Policy = "RequireIdentityWrite")]
            options.AddPolicy("RequireIdentityWrite",
                p => p.RequireClaim(ClaimCategory.Permission, ClaimValues.Permission.IdentityWrite));
            // e.g. full administrative tasks:
            // [Authorize(Policy = "RequireIdentityAdmin")]
            options.AddPolicy("RequireIdentityAdmin",
                p => p.RequireClaim(ClaimCategory.Permission, ClaimValues.Permission.IdentityAdmin));
            // e.g. system‑wide controls:
            // [Authorize(Policy = "RequireSystemAll")]
            options.AddPolicy("RequireSystemAll",
                p => p.RequireClaim(ClaimCategory.Permission, ClaimValues.Permission.SystemAll));
            // e.g. background/daemon operations:
            // [Authorize(Policy = "RequireDaemonExecute")]
            options.AddPolicy("RequireDaemonExecute",
                p => p.RequireClaim(ClaimCategory.Permission, ClaimValues.Permission.DaemonExecute));

            // — Role‑based policies —  
            // e.g. only system admins can access this page:
            // [Authorize(Policy = "AdminOnly")]
            options.AddPolicy("AdminOnly", p => p.RequireRole(ClaimValues.Role.Admin));
            // e.g. tenant‑scoped admin actions:
            // [Authorize(Policy = "TenantAdminOnly")]
            options.AddPolicy("TenantAdminOnly", p => p.RequireRole(ClaimValues.Role.TenantAdmin));
            // e.g. support staff dashboard:
            // [Authorize(Policy = "SupportOnly")]
            options.AddPolicy("SupportOnly", p => p.RequireRole(ClaimValues.Role.Support));
            // e.g. regular user pages:
            // [Authorize(Policy = "UserOnly")]
            options.AddPolicy("UserOnly", p => p.RequireRole(ClaimValues.Role.User));
            // e.g. daemon‑only endpoints:
            // [Authorize(Policy = "DaemonOnly")]
            options.AddPolicy("DaemonOnly", p => p.RequireRole(ClaimValues.Role.Daemon));

            // — Feature‑flag policies —  
            // e.g. show beta dashboard if enabled:
            // [Authorize(Policy = "FeatureBetaX")]
            options.AddPolicy("FeatureBetaX",
                p => p.RequireClaim(ClaimCategory.Feature, ClaimValues.Feature.BetaFeatureX));
            // e.g. grant access to new reports preview:
            // [Authorize(Policy = "FeatureReportsPreview")]
            options.AddPolicy("FeatureReportsPreview",
                p => p.RequireClaim(ClaimCategory.Feature, ClaimValues.Feature.ReportsPreview));

            // — Scope policies —  
            // e.g. for OIDC openid scope:
            // [Authorize(Policy = "ScopeOpenId")]
            options.AddPolicy("ScopeOpenId", p => p.RequireClaim(ClaimCategory.Scope, ClaimValues.Scope.OpenId));
            // e.g. for email scope:
            // [Authorize(Policy = "ScopeEmail")]
            options.AddPolicy("ScopeEmail", p => p.RequireClaim(ClaimCategory.Scope, ClaimValues.Scope.Email));

            // — Department policies —  
            // e.g. restrict to HR department:
            // [Authorize(Policy = "DeptHR")]
            options.AddPolicy("DeptHR", p => p.RequireClaim(ClaimCategory.Department, ClaimValues.Department.HR));
            // e.g. finance reports:
            // [Authorize(Policy = "DeptFinance")]
            options.AddPolicy("DeptFinance",
                p => p.RequireClaim(ClaimCategory.Department, ClaimValues.Department.Finance));

            // — System policies —  
            // e.g. maintenance mode pages:
            // [Authorize(Policy = "SystemMaintenance")]
            options.AddPolicy("SystemMaintenance",
                p => p.RequireClaim(ClaimCategory.System, ClaimValues.System.MaintenanceMode));

            // — Preference policies —  
            // e.g. dark‑theme preview:
            // [Authorize(Policy = "PrefThemeDark")]
            options.AddPolicy("PrefThemeDark",
                p => p.RequireClaim(ClaimCategory.Preference, ClaimValues.Preference.ThemeDark));

            // — Group policies —  
            // e.g. only members of ProjectX:
            // [Authorize(Policy = "GroupProjectX")]
            options.AddPolicy("GroupProjectX",
                p => p.RequireClaim(ClaimCategory.Group, ClaimValues.Group.ProjectXTeam));

            // — Resource policies —  
            // e.g. customer creation endpoint:
            // [Authorize(Policy = "ResourceCustomerCreate")]
            options.AddPolicy("ResourceCustomerCreate",
                p => p.RequireClaim(ClaimCategory.Resource, ClaimValues.Resource.CustomerCreate));

            // — Environment policies —  
            // e.g. restrict to production environment:
            // [Authorize(Policy = "EnvProduction")]
            options.AddPolicy("EnvProduction",
                p => p.RequireClaim(ClaimCategory.Environment, ClaimValues.Environment.Production));

            // — Authentication method policies —  
            // e.g. require two‑factor authentication:
            // [Authorize(Policy = "Auth2FA")]
            options.AddPolicy("Auth2FA",
                p => p.RequireClaim(ClaimCategory.Authentication, ClaimValues.Authentication.TwoFactor));

            // — Identity provider policies —  
            // e.g. only Azure AD users:
            // [Authorize(Policy = "IdP_AzureAD")]
            options.AddPolicy("IdP_AzureAD",
                p => p.RequireClaim(ClaimCategory.IdentityProvider, ClaimValues.IdentityProvider.AzureAD));

            // — Locale policies —  
            // e.g. restrict to US English:
            // [Authorize(Policy = "LocaleEnUS")]
            options.AddPolicy("LocaleEnUS", p => p.RequireClaim(ClaimCategory.Locale, ClaimValues.Locale.EnUS));

            // — Time policies —  
            // e.g. ensure issued‑at claim is present:
            // [Authorize(Policy = "TimeIssuedAt")]
            options.AddPolicy("TimeIssuedAt", p => p.RequireClaim(ClaimCategory.Time, ClaimValues.Time.IssuedAt));

         
        });


        return builder;
    }

    private static IServiceCollection ConfigureJwt(this IServiceCollection services, IConfiguration config,
        AuthOptions opts)
    {
        // var jwt = config.GetSection("Identity:Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(opts.IssuerSigningKey!));
        services.AddSingleton(new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        if (opts.AuthenticationDisabled)
            services.AddSingleton<IPolicyEvaluator, DisabledAuthenticationPolicyEvaluator>();

        services.AddSingleton(_ => new TokenValidationParameters
        {
            ValidateIssuer = opts.ValidateIssuer,
            ValidIssuer = opts.Issuer,
            ValidateAudience = opts.ValidateAudience,
            ValidAudience = opts.Audience,
            ValidateIssuerSigningKey = opts.ValidateIssuerSigningKey,
            IssuerSigningKey = key,
            ValidateLifetime = opts.ValidateLifetime,
            ClockSkew = TimeSpan.FromSeconds(30)
        });

        services.AddScoped<JwtFactory>();
        
        return services;
    }

    private static IServiceCollection ConfigureCookie(this IServiceCollection services, AuthOptions opts, out CookieOptions cookieOptions)
    {
        cookieOptions = new CookieOptions
        {
            HttpOnly = opts.Cookie.HttpOnly,
            Secure = opts.Cookie.Secure,
            SameSite = opts.Cookie.SameSite.ToLowerInvariant() switch
            {
                "strict" => SameSiteMode.Strict,
                "lax" => SameSiteMode.Lax,
                "none" => SameSiteMode.None,
                "unspecified" => SameSiteMode.Unspecified,
                _ => SameSiteMode.Unspecified
            },
            Domain = $".{opts.Cookie.Domain}",
            MaxAge = TimeSpan.FromHours(8)
        };
        services.AddSingleton(cookieOptions);
        return services;
    }

    private static IServiceCollection ConfigureEFIdentity(this IServiceCollection services, IConfiguration config)
    {
        // services.AddScoped<ITenantProvider, HttpContextTenantProvider>();
        // services.AddScoped<ITenantProviderSetter, TenantProvider>();
        return services;
    }

    private static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration config)
    {
        // services.AddScoped<TenantIsolationMiddleware>();
        
        services.AddScoped<AuthenticationProtocolResolverMiddleware>();
        services.AddAntiforgery();
        return services;
    }
    
    private static IApplicationBuilder UseMiddleware(this IApplicationBuilder app)
    {
        // app.UseMiddleware<TenantIsolationMiddleware>();
        
        app.UseMiddleware<AuthenticationProtocolResolverMiddleware>();
        return app;
    }

    public static IApplicationBuilder UseMameyAuthIdentity(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware();
        app.UseAntiforgery();
        return app;
    }

    public static class RolePermissionMapper
    {
        public static Dictionary<Type, Dictionary<string, long>> GenerateRolePermissionMapping<TRole>()
            where TRole : AppRole
        {
            return new()
            {
                [typeof(TRole)] = MapRoleToPermission(typeof(TRole))
            };
        }

        private static Dictionary<string, long> MapRoleToPermission(Type roleType)
        {
            var dict = new Dictionary<string, long>();
            foreach (var f in roleType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (f.GetValue(null) is Enum e) dict[f.Name] = Convert.ToInt64(e);
            }

            return dict;
        }
        
    }
    public static IEnumerable<Claim> ParseClaimsFromJwt(this string jwt)
    {
        if (string.IsNullOrEmpty(jwt.Trim()))
        {
            throw new ArgumentException($"'{nameof(jwt)}' cannot be null or empty.", nameof(jwt));
        }

        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        return keyValuePairs?.Select(kvp => new Claim(kvp.Key, kvp.Value?.ToString()));
    }
    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        return Convert.FromBase64String(base64);
    }
}

public abstract class AppRole
{
    public const ApplicationPermission Admin = ApplicationPermission.All;
    public const ApplicationPermission User = ApplicationPermission.None;

    [Flags]
    public enum ApplicationPermission : long
    {
        None = 0L,
        ViewAllAccounts = 1L << 0,
        ViewIndividualAccounts = 1L << 1,
        All = 1L << 999
    }
}
// using System.Reflection;
// using System.Text;
// using Mamey.Auth.Identity.Configuration;
// using Mamey.Auth.Identity.Data;
// using Mamey.Auth.Identity.Entities;
// using Mamey.Auth.Identity.Managers;
// using Mamey.Auth.Identity.Middleware;
// using Mamey.Auth.Identity.Providers;
// using Mamey.Auth.Identity.Redis;
// using Mamey.Auth.Identity.Server;
// using Mamey.Auth.Identity.Stores;
// using Mamey.Modules;
// using Mamey.Persistence.SQL;
// using Mamey.Postgres;
// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Authorization.Policy;
// using Microsoft.AspNetCore.Builder;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.IdentityModel.Tokens;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.EntityFrameworkCore;
//
// namespace Mamey.Auth.Identity;
//
// public static class Extensions
// {
//     private const string SectionName = "auth";
//     private const string RegistryName = "auth";
//
//     public static IMameyBuilder AddMameyAuthIdentity(this IMameyBuilder builder, string sectionName = SectionName,
//         Action<JwtBearerOptions> optionsFactory = null, IList<IModule> modules = null,
//         AuthorizationOptions? authorizationOptions = null, AuthenticationOptions? authenticationOptions = null)
//     {
//         if (!builder.TryRegister(RegistryName))
//             return builder;
//
//         var options = builder.Services.GetOptions<AuthOptions>(sectionName);
//         builder.Services.AddSingleton(options);
//
//         builder.Services.AddHttpContextAccessor();
//         builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//
//         builder.Services
//             .ConfigureJwt(builder.Configuration, options)
//             .ConfigureEFIdentity(builder.Configuration)
//             .ConfigureRedis(builder.Configuration, options)
//             .ConfigureServices(builder.Configuration)
//             .ConfigureAuthProviders(builder.Configuration);
//
//         // --- ASP.NET Core Identity setup ---
//         builder.Services
//             .AddIdentityCore<ApplicationUser>(opts =>
//             {
//                 opts.SignIn.RequireConfirmedAccount = true;
//             })
//             .AddRoles<ApplicationRole>()
//             .AddUserManager<MameyUserManager>()
//             .AddRoleManager<MameyRoleManager>()
//             .AddSignInManager<MameySignInManager>()
//             .AddUserStore<MameyUserStore>()
//             .AddRoleStore<MameyRoleStore>()
//             .AddClaimsPrincipalFactory<MameyClaimsPrincipalFactory>()
//             .AddDefaultTokenProviders();
//
//         // --- Configure JWT authentication ---
//         builder.Services
//             .AddAuthentication(options =>
//             {
//                 options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//                 options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//                 options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
//             })
//             .AddJwtBearer(opt =>
//             {
//                 //var jwt = builder.Configuration.GetSection("Identity:Jwt");
//                 opt.TokenValidationParameters = new TokenValidationParameters
//                 {
//                     ValidateIssuer = true,
//                     ValidIssuer = opt.ClaimsIssuer,
//                     ValidateAudience = true,
//                     ValidAudience = opt.Audience,
//                     ValidateIssuerSigningKey = true,
//                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.IssuerSigningKey!)),
//                     ValidateLifetime = true,
//                     ClockSkew = TimeSpan.FromSeconds(30)
//                 };
//
//                 opt.Events = new JwtBearerEvents
//                 {
//                     OnAuthenticationFailed = ctx =>
//                     {
//                         ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
//                         return Task.CompletedTask;
//                     },
//                     OnChallenge = ctx =>
//                     {
//                         ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
//                         return Task.CompletedTask;
//                     },
//                     OnForbidden = ctx =>
//                     {
//                         ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
//                         return Task.CompletedTask;
//                     }
//                 };
//
//                 optionsFactory?.Invoke(opt);
//             });
//
//         builder.Services.AddAuthorization(options =>
//         {
//             // Permission-based policies
//             options.AddPolicy("RequireIdentityRead",
//                 p => p.RequireClaim(ClaimCategory.Permission, ClaimValues.Permission.IdentityRead));
//             options.AddPolicy("RequireIdentityWrite",
//                 p => p.RequireClaim(ClaimCategory.Permission, ClaimValues.Permission.IdentityWrite));
//             options.AddPolicy("RequireIdentityAdmin",
//                 p => p.RequireClaim(ClaimCategory.Permission, ClaimValues.Permission.IdentityAdmin));
//             options.AddPolicy("RequireSystemAll",
//                 p => p.RequireClaim(ClaimCategory.Permission, ClaimValues.Permission.SystemAll));
//             options.AddPolicy("RequireDaemonExecute",
//                 p => p.RequireClaim(ClaimCategory.Permission, ClaimValues.Permission.DaemonExecute));
//
//             // Role-based policies
//             options.AddPolicy("AdminOnly", p => p.RequireRole(ClaimValues.Role.Admin));
//             options.AddPolicy("TenantAdminOnly", p => p.RequireRole(ClaimValues.Role.TenantAdmin));
//             options.AddPolicy("SupportOnly", p => p.RequireRole(ClaimValues.Role.Support));
//             options.AddPolicy("UserOnly", p => p.RequireRole(ClaimValues.Role.User));
//             options.AddPolicy("DaemonOnly", p => p.RequireRole(ClaimValues.Role.Daemon));
//
//             // Feature-flag policies
//             options.AddPolicy("FeatureBetaX",
//                 p => p.RequireClaim(ClaimCategory.Feature, ClaimValues.Feature.BetaFeatureX));
//             options.AddPolicy("FeatureReportsPreview",
//                 p => p.RequireClaim(ClaimCategory.Feature, ClaimValues.Feature.ReportsPreview));
//
//             // Scope policies
//             options.AddPolicy("ScopeOpenId", p => p.RequireClaim(ClaimCategory.Scope, ClaimValues.Scope.OpenId));
//             options.AddPolicy("ScopeEmail", p => p.RequireClaim(ClaimCategory.Scope, ClaimValues.Scope.Email));
//
//             // Department policies
//             options.AddPolicy("DeptHR", p => p.RequireClaim(ClaimCategory.Department, ClaimValues.Department.HR));
//             options.AddPolicy("DeptFinance",
//                 p => p.RequireClaim(ClaimCategory.Department, ClaimValues.Department.Finance));
//
//             // System policies
//             options.AddPolicy("SystemMaintenance",
//                 p => p.RequireClaim(ClaimCategory.System, ClaimValues.System.MaintenanceMode));
//
//             // Preference policies
//             options.AddPolicy("PrefThemeDark",
//                 p => p.RequireClaim(ClaimCategory.Preference, ClaimValues.Preference.ThemeDark));
//
//             // Group policies
//             options.AddPolicy("GroupProjectX",
//                 p => p.RequireClaim(ClaimCategory.Group, ClaimValues.Group.ProjectXTeam));
//
//             // Resource policies
//             options.AddPolicy("ResourceCustomerCreate",
//                 p => p.RequireClaim(ClaimCategory.Resource, ClaimValues.Resource.CustomerCreate));
//
//             // Environment policies
//             options.AddPolicy("EnvProduction",
//                 p => p.RequireClaim(ClaimCategory.Environment, ClaimValues.Environment.Production));
//
//             // Authentication method policies
//             options.AddPolicy("Auth2FA",
//                 p => p.RequireClaim(ClaimCategory.Authentication, ClaimValues.Authentication.TwoFactor));
//
//             // Identity provider policies
//             options.AddPolicy("IdP_AzureAD",
//                 p => p.RequireClaim(ClaimCategory.IdentityProvider, ClaimValues.IdentityProvider.AzureAD));
//
//             // Locale policies
//             options.AddPolicy("LocaleEnUS", p => p.RequireClaim(ClaimCategory.Locale, ClaimValues.Locale.EnUS));
//
//             // Time policies
//             options.AddPolicy("TimeIssuedAt", p => p.RequireClaim(ClaimCategory.Time, ClaimValues.Time.IssuedAt));
//         });
//
//         return builder;
//     }
//
//     private static IServiceCollection ConfigureJwt(this IServiceCollection services, IConfiguration config,
//         AuthOptions opts)
//     {
//         var jwt = config.GetSection("Identity:Jwt");
//         var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(opts.IssuerSigningKey!));
//         services.AddSingleton(new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
//
//         if (opts.AuthenticationDisabled)
//             services.AddSingleton<IPolicyEvaluator, DisabledAuthenticationPolicyEvaluator>();
//
//         services.AddScoped<JwtFactory>();
//         services.AddScoped<IJwtTokenValidator, JwtTokenValidator>();
//         return services;
//     }
//
//     private static IServiceCollection ConfigureEFIdentity(this IServiceCollection services, IConfiguration config)
//     {
//         services.AddScoped<ITenantProvider, HttpContextTenantProvider>();
//         services.AddScoped<ITenantProviderSetter, TenantProvider>();
//
//         services
//             .AddPostgres<MameyIdentityDbContext>(opts =>
//                 opts.MigrationsAssembly(typeof(MameyIdentityDbContext).Assembly.FullName))
//             .AddUnitOfWork<MameyIdentityUnitOfWork>();
//
//         services.AddScoped<IUserStore<ApplicationUser>, MameyUserStore>();
//         services.AddScoped<IRoleStore<ApplicationRole>, MameyRoleStore>();
//         services.AddScoped<IUserAuthenticationTokenStore<ApplicationUser>, MameyTokenStore>();
//         services.AddScoped<IRefreshTokenStore, RefreshTokenStore>();
//
//         return services;
//     }
//
//     private static IServiceCollection ConfigureRedis(this IServiceCollection services, IConfiguration config,
//         AuthOptions opts)
//     {
//         services.AddScoped<ITokenRevocationStore, TokenRevocationStore>();
//         services.AddScoped<UserClaimsCache>();
//         return services;
//     }
//
//     private static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration config)
//     {
//         services.AddScoped<IUserManager, MameyUserManager>();
//         services.AddScoped<IRoleManager, MameyRoleManager>();
//         services.AddScoped<ISignInManager, MameySignInManager>();
//         services.AddScoped<IMfaHandler, MfaHandler>();
//         services.AddScoped<ILoginAttemptService, LoginAttemptService>();
//         services.AddScoped<ITokenService, JwtTokenService>();
//         services.AddScoped<IPermissionEvaluator, PermissionEvaluator>();
//         services.AddScoped<MameyClaimsPrincipalFactory>();
//         services.AddScoped<ClaimEnricher>();
//         services.AddScoped<IImpersonationContext, ImpersonationService>();
//         services.AddSingleton(new RolePermissions(RolePermissionMapper.GenerateRolePermissionMapping<AppRole>()));
//         services.AddScoped<JwtIdentityMiddleware>();
//         services.AddScoped<AuthenticationProtocolResolverMiddleware>();
//         return services;
//     }
//
//     private static IServiceCollection ConfigureAuthProviders(this IServiceCollection services, IConfiguration config)
//     {
//         services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationPolicyProvider, MameyPolicyProvider>();
//         services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, PermissionRequirementHandler>();
//         return services;
//     }
//
//     private static IApplicationBuilder UseMiddleware(this IApplicationBuilder app)
//     {
//         app.UseMiddleware<JwtIdentityMiddleware>();
//         app.UseMiddleware<AuthenticationProtocolResolverMiddleware>();
//         return app;
//     }
//
//     public static IApplicationBuilder UseMameyAuthIdentity(this IApplicationBuilder app)
//     {
//         app.UseAuthentication();
//         app.UseAuthorization();
//         app.UseMiddleware();
//         return app;
//     }
//
//     public static class RolePermissionMapper
//     {
//         public static Dictionary<Type, Dictionary<string, long>> GenerateRolePermissionMapping<TRole>()
//             where TRole : AppRole
//         {
//             return new()
//             {
//                 [typeof(TRole)] = MapRoleToPermission(typeof(TRole))
//             };
//         }
//
//         private static Dictionary<string, long> MapRoleToPermission(Type roleType)
//         {
//             var dict = new Dictionary<string, long>();
//             foreach (var f in roleType.GetFields(BindingFlags.Public | BindingFlags.Static))
//             {
//                 if (f.GetValue(null) is Enum e) dict[f.Name] = Convert.ToInt64(e);
//             }
//
//             return dict;
//         }
//     }
// }
//
// public abstract class AppRole
// {
//     public const ApplicationPermission Admin = ApplicationPermission.All;
//     public const ApplicationPermission User = ApplicationPermission.None;
//
//     [Flags]
//     public enum ApplicationPermission : long
//     {
//         None = 0L,
//         ViewAllAccounts = 1L << 0,
//         ViewIndividualAccounts = 1L << 1,
//         All = 1L << 999
//     }
// }