using System.Runtime.CompilerServices;
using Mamey.Government.Modules.Tenant.Core.Composite;
using Mamey.Government.Modules.Tenant.Core.EF;
using Mamey.Government.Modules.Tenant.Core.Mongo;
using Mamey.Government.Modules.Tenant.Core.Seeding;
using Mamey.Government.Modules.Tenant.Core.Sync;
using Mamey.Government.Shared.Abstractions.Seeding;
using Mamey.MicroMonolith.Infrastructure;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Mamey.Postgres;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Mamey.Government.Modules.Tenant.Api")]
namespace Mamey.Government.Modules.Tenant.Core
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
                // .AddScoped<IModuleSeeder, TenantDataSeeder>()
                ;
        }

        public static async Task<IApplicationBuilder> UseCoreAsync(this IApplicationBuilder app)
        {
            await Task.CompletedTask;
            return app;
        }
    }
}
