using System.Runtime.CompilerServices;
using Mamey.Government.Modules.Certificates.Core.Composite;
using Mamey.Government.Modules.Certificates.Core.EF;
using Mamey.Government.Modules.Certificates.Core.Mongo;
using Mamey.Government.Modules.Certificates.Core.Seeding;
using Mamey.Government.Modules.Certificates.Core.Sync;
using Mamey.Government.Shared.Abstractions.Seeding;
using Mamey.MicroMonolith.Infrastructure;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Mamey.Postgres;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Mamey.Government.Modules.Certificates.Api")]
namespace Mamey.Government.Modules.Certificates.Core
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
                // .AddScoped<IModuleSeeder, CertificateDataSeeder>()
                ;
        }

        public static async Task<IApplicationBuilder> UseCoreAsync(this IApplicationBuilder app)
        {
            await Task.CompletedTask;
            return app;
        }
    }
}
