using System.Runtime.CompilerServices;
using Mamey.Government.Modules.Citizens.Core.Composite;
using Mamey.Government.Modules.Citizens.Core.EF;
using Mamey.Government.Modules.Citizens.Core.Mongo;
using Mamey.Government.Modules.Citizens.Core.Seeding;
using Mamey.Government.Modules.Citizens.Core.Sync;
using Mamey.Government.Shared.Abstractions.Seeding;
using Mamey.MicroMonolith.Infrastructure;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Mamey.Postgres;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Mamey.Government.Modules.Citizens.Api")]
namespace Mamey.Government.Modules.Citizens.Core
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
                // .AddScoped<IModuleSeeder, CitizenDataSeeder>()
                ;
        }

        public static async Task<IApplicationBuilder> UseCoreAsync(this IApplicationBuilder app)
        {
            await Task.CompletedTask;
            return app;
        }
    }
}
