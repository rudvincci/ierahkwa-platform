using System.Runtime.CompilerServices;
using Mamey.ApplicationName.Modules.Identity.Core.EF;
using Mamey.ApplicationName.Modules.Identity.Core.Mongo;
using Mamey.ApplicationName.Modules.Identity.Core.Services;
using Mamey.ApplicationName.Modules.Identity.Core.Storage;
using Mamey.Auth.Identity.Entities;
using Mamey.MicroMonolith.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: InternalsVisibleTo("Mamey.ApplicationName.Modules.Identity.Api")]
namespace Mamey.ApplicationName.Modules.Identity.Core
{
    internal static class Extensions
    {

        public static IServiceCollection AddCore(this IServiceCollection services)
        {

            services
                .AddInitializer<IdentityInitializer>();
            
            
            // Register custom stores
            // services.AddScoped<IUserStore<ApplicationUser>, UserStore<ApplicationUser, ApplicationRole, ApplicationIdentityDbContext, Guid>>();
            // services.AddScoped<IRoleStore<ApplicationRole>, RoleStore<ApplicationRole, ApplicationIdentityDbContext, Guid>>();
   
            
            // Register identity and specify managers
            // services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            //     {
            //         // options.Password.RequiredLength = 6;
            //         // options.Password.RequireNonAlphanumeric = false;
            //         // options.Password.RequireUppercase = true;
            //     })
            //     .AddSignInManager()
            //     .AddDefaultTokenProviders();
            // services.AddAuthentication(options =>
            //     {
            //         options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //         options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //         options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            //     })
            //     .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            //     {
            //         options.LoginPath = "/Account/Login";
            //         options.AccessDeniedPath = "/Account/AccessDenied";
            //         options.SlidingExpiration = true;
            //     });
            // // Authorization Policies
            // services.AddAuthorization(options =>
            // {
            //     options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            //     options.AddPolicy("UserAccess", policy => policy.RequireAuthenticatedUser());
            // });
            
            // services.AddInitializer<IdentityInitializer>();    

            return services
                .AddStorage()
                .AddPostgresDb()
                .AddMongo()
                .AddIdentityServices();
        }

        // public static IServiceCollection AddApplicationRolesAuthorization(this IServiceCollection services)
        // {
        //     var policies = modules?.SelectMany(x => x.Policies ?? Enumerable.Empty<string>()) ??
        //                    Enumerable.Empty<string>();
        //     foreach (var VARIABLE in Mamey.Bank.Shared)
        //     {
        //         
        //     }
        //     services.AddAuthorization(authorization =>
        //     {
        //         foreach (var policy in policies)
        //         {
        //             authorization.AddPolicy(policy, x => x.RequireClaim("permissions", policy));
        //         }
        //     });
        // }
        public static async Task<IApplicationBuilder> UseCoreAsync(this IApplicationBuilder app)
        {
            
            // await app.UsePostgresDbAsync();
            await Task.CompletedTask;
            return app;
        }
    }
}

