using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.Rewards.Domain.Repositories;
using Pupitre.Rewards.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Rewards.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddRewardPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<RewardDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(RewardDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_Reward", "reward");
            })
            .AddUnitOfWork<RewardUnitOfWork>();
        builder.Services.AddScoped<IRewardUnitOfWork>(provider => provider.GetRequiredService<RewardUnitOfWork>());
            
        builder.Services.AddTransient<RewardInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseRewardPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<RewardDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddRewardPostgres(this IServiceCollection services)
    {
        services.AddScoped<IRewardRepository, RewardPostgresRepository>();
        return services
            .AddPostgres<RewardDbContext>();
    }
}
