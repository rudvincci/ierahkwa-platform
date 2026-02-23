// Infrastructure/EF/Extensions.cs

using Mamey.Auth.Identity.Abstractions;
using Mamey.Auth.Identity.Abstractions.Entities;
using Mamey.Blazor.Identity;
using Mamey.Postgres;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mamey.Casino.Infrastructure.EF
{
    public static class Extensions
    {
        public static IMameyBuilder AddEf(this IMameyBuilder builder)
        {
            var services = builder.Services;
            var config   = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

            // --- persistence & UoW
            

            
            

            // --- Identity + EF stores + roles + tokens
            services
                .AddIdentity<ApplicationUser, ApplicationRole>(opts =>
                {
                    opts.SignIn.RequireConfirmedAccount = true;
                    // you can tweak password rules here if you like
                })
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<CasinoDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();
        
            // // --- Configure the Identity application cookie (no OIDC) ---
            services.ConfigureApplicationCookie(opts =>
            {
                // cookie settings
                opts.Cookie.Name         = "CasinoAuth";
                opts.Cookie.HttpOnly     = true;
                opts.Cookie.SameSite     = SameSiteMode.Lax;
#if DEBUG
                opts.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;  // works over HTTP & HTTPS
#else
                opts.Cookie.SecurePolicy = CookieSecurePolicy.Always;        // only HTTPS in prod
#endif
            
                // paths
                opts.LoginPath           = "/Account/Login";
                opts.LogoutPath          = "/Account/Logout";
                opts.AccessDeniedPath    = "/Account/AccessDenied";
                opts.ReturnUrlParameter  = CookieAuthenticationDefaults.ReturnUrlParameter;
            
                // expiration
                opts.ExpireTimeSpan      = TimeSpan.FromHours(8);
                opts.SlidingExpiration   = true;
            
                // different behavior for API vs UI
                opts.Events = new CookieAuthenticationEvents
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
            });

            // --- Authorization (add policies as needed) ---
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
            });

            // --- your initializer ---
            services.AddTransient<IdentityInitializer>();

            
            builder.Services.AddScoped<IIdentityUserAccessor, IdentityUserAccessor>();
            builder.Services.AddScoped<IIdentityRedirectManager,IdentityRedirectManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
            return builder;
        }

        /// <summary>
        /// Applies DB migrations/creation, then runs your IdentityInitializer.
        /// Also adds the Authentication & Authorization middleware.
        /// </summary>
        public static async Task<IApplicationBuilder> UsePostgresDbAsync(this IApplicationBuilder app)
        {
            // ensure DB + seed
            using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var svcProv    = scope.ServiceProvider;
            var db         = svcProv.GetRequiredService<CasinoDbContext>();
            var init       = svcProv.GetRequiredService<IdentityInitializer>();
            var logger     = svcProv.GetRequiredService<ILogger<CasinoDbContext>>();

    #if DEBUG
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();
    #else
            await db.Database.MigrateAsync();
    #endif
            await init.InitializeAsync();

            // insert Identity middleware
            app.UseAuthentication();
            app.UseAuthorization();
            // app.UseAntiforgery();
            return app;
        }
    }
    
}
