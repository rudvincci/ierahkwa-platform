using System.Runtime.CompilerServices;
using Mamey.Government.Modules.Documents.Core.Composite;
using Mamey.Government.Modules.Documents.Core.EF;
using Mamey.Government.Modules.Documents.Core.Mongo;
using Mamey.Government.Modules.Documents.Core.Sync;
using Mamey.MicroMonolith.Infrastructure;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Mamey.Postgres;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Mamey.Government.Modules.Documents.Api")]
namespace Mamey.Government.Modules.Documents.Core
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
            await Task.CompletedTask;
            return app;
        }
    }
}
