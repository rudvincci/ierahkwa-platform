using System.Runtime.CompilerServices;
using Mamey.Government.Modules.CitizenshipApplications.Core.Clients;
using Mamey.Government.Modules.CitizenshipApplications.Core.Composite;
using Mamey.Government.Modules.CitizenshipApplications.Core.EF;
using Mamey.Government.Modules.CitizenshipApplications.Core.Mongo;
using Mamey.Government.Modules.CitizenshipApplications.Core.Services;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Mamey.Postgres;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Mamey.Government.Modules.CitizenshipApplications.Api")]
namespace Mamey.Government.Modules.CitizenshipApplications.Core
{
    internal static class Extensions
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            services.AddScoped<IApplicationNumberService, ApplicationNumberService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<ICitizenApplicationsService, CitizenApplicationsService>();
            return services
                .AddHttpContextAccessor()
                .AddScoped<IContext, Context>()
                .AddPostgresDb()
                .AddMongoDb()
                .AddCompositeRepositories()
                .AddGeoLocation()
                .AddClients()
                // .AddScoped<IModuleSeeder, ApplicationDataSeeder>()
                ;
        }

        public static async Task<IApplicationBuilder> UseCoreAsync(this IApplicationBuilder app)
        {
            await Task.CompletedTask;
            return app;
        }

        private static IServiceCollection AddClients(this IServiceCollection services)
        {
            services.AddScoped<INotificationsClient, NotificationsClient>();
            return services;
        }

        private static IServiceCollection AddGeoLocation(this IServiceCollection services)
        {
            services
                .AddOptions<GeoLocationOptions>()
                .BindConfiguration("citizenship-applications:geo-location");
            services.AddHttpClient<IGeoLocationService, GeoLocationService>();
            return services;
        }
    }
}
