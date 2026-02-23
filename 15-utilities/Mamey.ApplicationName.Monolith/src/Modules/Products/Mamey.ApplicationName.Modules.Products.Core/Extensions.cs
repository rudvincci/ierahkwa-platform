using System.Runtime.CompilerServices;
using Mamey.ApplicationName.Modules.Products.Core.EF;
using Mamey.ApplicationName.Modules.Products.Core.Services;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Mamey.ApplicationName.Modules.Products.Api")]
namespace Mamey.ApplicationName.Modules.Products.Core
{
    internal static class Extensions
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
            => services
                .AddScoped<IBankingProductService, BankingProductService>()
                .AddPostgresDb();

        // public static async Task<IApplicationBuilder> UseCore(this IApplicationBuilder app)
        // {
        //     await app.UsePostgresDbAsync();
        //     return app;
        // }
    }
}