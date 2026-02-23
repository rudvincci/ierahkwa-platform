using System.Runtime.CompilerServices;
using Mamey.Government.Modules.TravelIdentities.Core.Clients;
using Mamey.Government.Modules.TravelIdentities.Core.Composite;
using Mamey.Government.Modules.TravelIdentities.Core.EF;
using Mamey.Government.Modules.TravelIdentities.Core.Mongo;
using Mamey.Government.Modules.TravelIdentities.Core.Mongo.Documents;
using Mamey.Government.Modules.TravelIdentities.Core.Seeding;
using Mamey.Government.Modules.TravelIdentities.Core.Services;
using Mamey.Government.Modules.TravelIdentities.Core.Sync;
using Mamey.Government.Shared.Abstractions.Seeding;
using Mamey.MicroMonolith.Infrastructure;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Mamey.Postgres;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Mamey.Government.Modules.TravelIdentities.Api")]
namespace Mamey.Government.Modules.TravelIdentities.Core
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
                // .AddScoped<IModuleSeeder, TravelIdentityDataSeeder>()
                ;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAamvaBarcodeService, AamvaBarcodeService>();
            services.AddScoped<IDocumentNumberService, DocumentNumberService>();
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
