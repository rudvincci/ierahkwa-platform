using System.Runtime.CompilerServices;
// using Mamey.ApplicationName.Modules.Customers.Core.Mongo;
using Mamey.Bank.Modules.Customers.Infrastructure.EF;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Mamey.ApplicationName.Modules.Customers.Api")]
namespace Mamey.ApplicationName.Modules.Customers.Core
{
    internal static class Extensions
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
            => services
                .AddPostgres()
                // .AddMongo()
                ;

    }
}