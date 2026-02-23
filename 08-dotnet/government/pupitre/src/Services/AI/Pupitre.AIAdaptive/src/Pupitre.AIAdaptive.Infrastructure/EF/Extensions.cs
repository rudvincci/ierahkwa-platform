using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.AIAdaptive.Domain.Repositories;
using Pupitre.AIAdaptive.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AIAdaptive.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddAdaptiveLearningPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<AdaptiveLearningDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(AdaptiveLearningDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_AdaptiveLearning", "adaptivelearning");
            })
            .AddUnitOfWork<AdaptiveLearningUnitOfWork>();
        builder.Services.AddScoped<IAdaptiveLearningUnitOfWork>(provider => provider.GetRequiredService<AdaptiveLearningUnitOfWork>());
            
        builder.Services.AddTransient<AdaptiveLearningInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseAdaptiveLearningPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<AdaptiveLearningDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddAdaptiveLearningPostgres(this IServiceCollection services)
    {
        services.AddScoped<IAdaptiveLearningRepository, AdaptiveLearningPostgresRepository>();
        return services
            .AddPostgres<AdaptiveLearningDbContext>();
    }
}
