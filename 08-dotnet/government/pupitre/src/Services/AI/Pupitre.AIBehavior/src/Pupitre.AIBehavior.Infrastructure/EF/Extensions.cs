using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.AIBehavior.Domain.Repositories;
using Pupitre.AIBehavior.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AIBehavior.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddBehaviorPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<BehaviorDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(BehaviorDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_Behavior", "behavior");
            })
            .AddUnitOfWork<BehaviorUnitOfWork>();
        builder.Services.AddScoped<IBehaviorUnitOfWork>(provider => provider.GetRequiredService<BehaviorUnitOfWork>());
            
        builder.Services.AddTransient<BehaviorInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseBehaviorPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<BehaviorDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddBehaviorPostgres(this IServiceCollection services)
    {
        services.AddScoped<IBehaviorRepository, BehaviorPostgresRepository>();
        return services
            .AddPostgres<BehaviorDbContext>();
    }
}
