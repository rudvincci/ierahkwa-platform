using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.AIRecommendations.Domain.Repositories;
using Pupitre.AIRecommendations.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AIRecommendations.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddAIRecommendationPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<AIRecommendationDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(AIRecommendationDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_AIRecommendation", "airecommendation");
            })
            .AddUnitOfWork<AIRecommendationUnitOfWork>();
        builder.Services.AddScoped<IAIRecommendationUnitOfWork>(provider => provider.GetRequiredService<AIRecommendationUnitOfWork>());
            
        builder.Services.AddTransient<AIRecommendationInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseAIRecommendationPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<AIRecommendationDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddAIRecommendationPostgres(this IServiceCollection services)
    {
        services.AddScoped<IAIRecommendationRepository, AIRecommendationPostgresRepository>();
        return services
            .AddPostgres<AIRecommendationDbContext>();
    }
}
