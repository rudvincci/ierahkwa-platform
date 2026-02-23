using Mamey.MicroMonolith.Infrastructure;
// using Mamey.ApplicationName.Modules.Identity.Core.EF.Storage;
using Mamey.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ApplicationName.Modules.Identity.Core.EF
{
    public static class Extensions
    {
        public static IServiceCollection AddPostgresDb(this IServiceCollection services)
        {
            
            services
                .AddPostgres<ApplicationIdentityDbContext>(builder =>
                    {
                        builder.MigrationsAssembly(typeof(ApplicationIdentityDbContext).Assembly.FullName);
                        builder.MigrationsHistoryTable("__EFMigrationsHistory_Identity", "identity");
                    })
                .AddUnitOfWork<ApplicationIdentityUnitOfWork>();
            
            services.AddTransient<IdentityInitializer>();
            return services;
        }
        // public static async Task<IApplicationBuilder> UsePostgresDbAsync(this IApplicationBuilder app)
        // {
        //     using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
        //     if (serviceScope != null)
        //     {
        //         var serviceProvider = serviceScope.ServiceProvider;
        //         var dbContext =  serviceProvider.GetRequiredService<ApplicationIdentityDbContext>();
        //         var initializer = serviceProvider.GetRequiredService<IdentityInitializer>(); 
        //         await dbContext.Database.MigrateAsync();
        //         await initializer.InitAsync(serviceProvider);
        //     }
        //
        //     return app;
        // }
        
    }
}
