// using System.Security.Cryptography.X509Certificates;
// using System.Text;
// using Mamey.ApplicationName.Modules.Identity.Core.Validators;
// using Mamey.Auth.Identity.Abstractions.Authorization;
// using Mamey.Auth.Identity.Abstractions.EF;
// using Mamey.Auth.Identity.Abstractions.Handlers;
// using Mamey.Auth.Identity.Abstractions.Middleware;
// using Mamey.Auth.Identity.Abstractions.Services;
// using Mamey.Auth.Identity.Abstractions.Stores;
// using Mamey.Auth.Identity.Abstractions.Validators;
// using Mamey.Auth.Identity.Configuration;
// using Mamey.Auth.Identity.Data;
// using Mamey.Auth.Identity.Entities;
// using Mamey.Auth.Identity.Managers;
// using Mamey.Auth.Identity.Middleware;
// using Mamey.Auth.Identity.Redis;
// using Mamey.Auth.Identity.Server;
// using Mamey.Auth.Identity.Stores;
// using Mamey.Modules;
// using Mamey.Postgres;
// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Authorization.Policy;
// using Microsoft.AspNetCore.Builder;
// using Microsoft.AspNetCore.Components.Authorization;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;
// using Microsoft.IdentityModel.Tokens;
//
// namespace Mamey.Auth.Identity.Abstractions;
//
// /// <summary>
// /// Registers IdentitySystem services: DbContext, caching, eventing,
// /// authentication handlers, application services, and middleware.
// /// </summary>
// public static class Extensions
// {
//     private const string SectionName = "auth";
//     private const string RegistryName = "auth";
//     private const string CookieName = "MameyIdentity";
//     private const string AuthorizationHeader = "authorization";
//     
//     public static IMameyBuilder AddIdentityAuthentication(this IMameyBuilder builder, AuthOptions options, string sectionName = SectionName,
//         Action<JwtBearerOptions> optionsFactory = null, IList<IModule> modules = null)
//     {
//         if (!builder.TryRegister(RegistryName))
//         {
//             return builder;
//         }
//
//         builder.Services.AddSingleton(options);
//         
//  
//             
//             builder.Services.AddScoped<ITokenRevocationStore, TokenRevocationStore>();
//             builder.Services.AddScoped<UserClaimsCache>();
//             
//             // 7) Validators
//             builder.Services.AddScoped<TenantUserValidator>();
//             builder.Services.AddScoped<StrongPasswordValidator>();
//
//             // 8) Authorization
//             builder.Services.AddSingleton<IAuthorizationPolicyProvider, MameyPolicyProvider>();
//             builder.Services.AddScoped<IAuthorizationHandler, PermissionRequirementHandler>();
//         return builder;
//     }
//     
//     private static class ServiceCollectionExtensions
//     {
//         public static IServiceCollection AddIdentitySystem(
//             this IServiceCollection services,
//             IConfiguration configuration)
//         {
//             
//             
//             // 4) Authentication schemes
//             // 4a) JWT
//             var jwtKey   = configuration["Jwt:SigningKey"]!;
//             var issuer   = configuration["Jwt:Issuer"]!;
//             var audience = configuration["Jwt:Audience"]!;
//             var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
//             services.AddSingleton(new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));
//             services.AddSingleton<JwtFactory>();
//             services.AddSingleton(sp =>
//                 new TokenValidationParameters
//                 {
//                     ValidateIssuer           = true,
//                     ValidIssuer              = issuer,
//                     ValidateAudience         = true,
//                     ValidAudience            = audience,
//                     ValidateIssuerSigningKey = true,
//                     IssuerSigningKey         = signingKey,
//                     ValidateLifetime         = true,
//                     ClockSkew                = TimeSpan.FromSeconds(30)
//                 });
//             
//             
//
//
//             // 4b) Certificate
//             services.AddAuthentication(options =>
//                 {
//                     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//                     options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
//                 })
//                 .AddJwtBearer()
//                 ;
//
//             
//
//             
//
//             
//
//             // 9) Controllers & Filters
//             
//             
//
//             return services;
//         }
//     }
//     /// <summary>
//     /// Applies DB migrations/creation, then runs your IdentityInitializer.
//     /// Also adds the Authentication & Authorization middleware.
//     /// </summary>
//     public static async Task<IApplicationBuilder> UseIdentityAuthenticationAsync(this IApplicationBuilder app)
//     {
//         app.UseAuthentication();
//         // ensure DB + seed
//         using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
//         var svcProv    = scope.ServiceProvider;
//         var db         = svcProv.GetRequiredService<MameyIdentityDbContext>();
//         var init       = svcProv.GetRequiredService<IdentityInitializer>();
//         var logger     = svcProv.GetRequiredService<ILogger<MameyIdentityDbContext>>();
//
// #if DEBUG
//         await db.Database.EnsureDeletedAsync();
//         await db.Database.EnsureCreatedAsync();
// #else
//             await db.Database.MigrateAsync();
// #endif
//         await init.InitializeAsync();
//
//         // insert Identity middleware
//         app.UseAuthentication();
//         app.UseAuthorization();
//         app.Use(async (ctx, next) =>
//         {
//             if (ctx.Request.Headers.ContainsKey(AuthorizationHeader))
//             {
//                 ctx.Request.Headers.Remove(AuthorizationHeader);
//             }
//
//             if (ctx.Request.Cookies.ContainsKey(CookieName))
//             {
//                 var authenticateResult = await ctx.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
//                 if (authenticateResult.Succeeded && authenticateResult.Principal is not null)
//                 {
//                     ctx.User = authenticateResult.Principal;
//                 }
//             }
//
//             await next();
//         });
//         // app.UseAntiforgery();
//         return app;
//     }
//     
// }

