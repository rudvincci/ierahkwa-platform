using System;
using Mamey.Postgres;
using Mamey.ServiceName.Domain.Repositories;
using Mamey.ServiceName.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ServiceName.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddEntityNamePostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<EntityNameDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(EntityNameDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_EntityName", "entityname");
            })
            .AddUnitOfWork<EntityNameUnitOfWork>();
        builder.Services.AddScoped<IEntityNameUnitOfWork>(provider => provider.GetRequiredService<EntityNameUnitOfWork>());
            
        builder.Services.AddTransient<EntityNameInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseEntityNamePostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<EntityNameDbContext>();
            #if DEBUG
            dbContext.Database.EnsureCreated();
            #else
            dbContext.Database.Migrate();
            #endif
        }
		return builder;
	}
    private static IServiceCollection AddEntityNamePostgres(this IServiceCollection services)
    {
        services.AddScoped<IEntityNameRepository, EntityNamePostgresRepository>();
        return services
            .AddPostgres<EntityNameDbContext>();
    }
}
