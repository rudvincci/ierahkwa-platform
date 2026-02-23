using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.Parents.Domain.Repositories;
using Pupitre.Parents.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Parents.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddParentPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<ParentDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(ParentDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_Parent", "parent");
            })
            .AddUnitOfWork<ParentUnitOfWork>();
        builder.Services.AddScoped<IParentUnitOfWork>(provider => provider.GetRequiredService<ParentUnitOfWork>());
            
        builder.Services.AddTransient<ParentInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseParentPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<ParentDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddParentPostgres(this IServiceCollection services)
    {
        services.AddScoped<IParentRepository, ParentPostgresRepository>();
        return services
            .AddPostgres<ParentDbContext>();
    }
}
