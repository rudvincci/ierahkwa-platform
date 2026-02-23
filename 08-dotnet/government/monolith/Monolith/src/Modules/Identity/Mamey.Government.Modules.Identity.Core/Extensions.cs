using System.Runtime.CompilerServices;
using Mamey.Government.Modules.Identity.Core.Composite;
using Mamey.Government.Modules.Identity.Core.EF;
using Mamey.Government.Modules.Identity.Core.Mongo;
using Mamey.Government.Modules.Identity.Core.Sync;
using Mamey.MicroMonolith.Infrastructure;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Mamey.Postgres;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Mamey.Government.Modules.Identity.Api")]
namespace Mamey.Government.Modules.Identity.Core
{
    internal static class Extensions
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            return services
                .AddPostgresDb()
                .AddMongoDb()
                .AddCompositeRepositories()
                .AddReadModelSyncServices();
        }

        public static async Task<IApplicationBuilder> UseCoreAsync(this IApplicationBuilder app)
        {
            // Additional async initialization can be added here
            await Task.CompletedTask;
            return app;
        }
    }
}
