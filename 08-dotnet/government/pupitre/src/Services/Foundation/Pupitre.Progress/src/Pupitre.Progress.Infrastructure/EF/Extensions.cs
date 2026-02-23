using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.Progress.Domain.Repositories;
using Pupitre.Progress.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Progress.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddLearningProgressPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<LearningProgressDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(LearningProgressDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_LearningProgress", "learningprogress");
            })
            .AddUnitOfWork<LearningProgressUnitOfWork>();
        builder.Services.AddScoped<ILearningProgressUnitOfWork>(provider => provider.GetRequiredService<LearningProgressUnitOfWork>());
            
        builder.Services.AddTransient<LearningProgressInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseLearningProgressPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<LearningProgressDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddLearningProgressPostgres(this IServiceCollection services)
    {
        services.AddScoped<ILearningProgressRepository, LearningProgressPostgresRepository>();
        return services
            .AddPostgres<LearningProgressDbContext>();
    }
}
