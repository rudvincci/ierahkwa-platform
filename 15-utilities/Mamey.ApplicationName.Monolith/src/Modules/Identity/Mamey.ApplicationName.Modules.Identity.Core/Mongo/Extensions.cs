// using Mamey.ApplicationName.Modules.Identity.Core.Mongo.Documents;
// using Mamey.ApplicationName.Modules.Identity.Core.Mongo.Repositories;
// using Mamey.ApplicationName.Modules.Identity.Core.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ApplicationName.Modules.Identity.Core.Mongo
{
    internal static class Extensions
    {
        private static readonly string Schema = "identity-module";

        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
            // services.AddScoped<IIdentityRepository, IdentityMongoRepository>();
            // services.AddMongoRepository<ApplicationUserDocument, Guid>($"{Schema}.identity");
            return services;
        }
    }
}
