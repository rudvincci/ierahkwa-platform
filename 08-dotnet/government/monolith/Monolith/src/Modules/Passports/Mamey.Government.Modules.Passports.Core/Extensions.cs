using System.Runtime.CompilerServices;
using Mamey.Government.Modules.Passports.Core.Clients;
using Mamey.Government.Modules.Passports.Core.Composite;
using Mamey.Government.Modules.Passports.Core.EF;
using Mamey.Government.Modules.Passports.Core.Mongo;
using Mamey.Government.Modules.Passports.Core.Seeding;
using Mamey.Government.Modules.Passports.Core.Services;
using Mamey.Government.Modules.Passports.Core.Sync;
using Mamey.Government.Shared.Abstractions.Seeding;
using Mamey.MicroMonolith.Infrastructure;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Mamey.Postgres;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Mamey.Government.Modules.Passports.Api")]
namespace Mamey.Government.Modules.Passports.Core
{
    internal static class Extensions
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            return services
                .AddPostgresDb()
                .AddMongoDb()
                .AddCompositeRepositories()
                .AddReadModelSyncServices()
                .AddServices()
                .AddClients()
                // .AddScoped<IModuleSeeder, PassportDataSeeder>()
                ;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IMrzGeneratorService, MrzGeneratorService>();
            return services;
        }

        private static IServiceCollection AddClients(this IServiceCollection services)
        {
            services.AddScoped<ICitizensClient, CitizensClient>();
            return services;
        }

        public static async Task<IApplicationBuilder> UseCoreAsync(this IApplicationBuilder app)
        {
            await Task.CompletedTask;
            return app;
        }
    }
}
