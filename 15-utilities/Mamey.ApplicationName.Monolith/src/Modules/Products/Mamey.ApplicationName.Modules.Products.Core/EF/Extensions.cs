using System.Threading.Tasks;
using Mamey.ApplicationName.Modules.Products.Core.EF.Repositories;
using Mamey.ApplicationName.Modules.Products.Core.Repositories;
using Mamey.MicroMonolith.Infrastructure;
using Mamey.Postgres;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ApplicationName.Modules.Products.Core.EF;

public static class Extensions
{
    public static IServiceCollection AddPostgresDb(this IServiceCollection services)
    {
        services
            .AddPostgres()
            .AddPostgres<BankingProductDbContext>()
            .AddScoped<IBankingProductRepository, BankingProductRepository>()
                
            .AddUnitOfWork<BankingProductUnitOfWork>()
            .AddInitializer<BankingProductInitializer>()
            ;
            
        return services;
    }

    public static async Task<IApplicationBuilder> UsePostgresDbAsync(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
        if (serviceScope != null)
        {
            var serviceProvider = serviceScope.ServiceProvider;
            var dbContext =  serviceProvider.GetRequiredService<BankingProductDbContext>();
            var initializer = serviceProvider.GetRequiredService<BankingProductInitializer>(); 
            await dbContext.Database.MigrateAsync();
            await initializer.InitAsync(serviceProvider);
        }
    
        return app;
    }
}