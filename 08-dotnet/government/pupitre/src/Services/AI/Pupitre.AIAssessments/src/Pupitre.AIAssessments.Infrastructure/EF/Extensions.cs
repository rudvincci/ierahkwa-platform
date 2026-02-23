using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.AIAssessments.Domain.Repositories;
using Pupitre.AIAssessments.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AIAssessments.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddAIAssessmentPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<AIAssessmentDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(AIAssessmentDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_AIAssessment", "aiassessment");
            })
            .AddUnitOfWork<AIAssessmentUnitOfWork>();
        builder.Services.AddScoped<IAIAssessmentUnitOfWork>(provider => provider.GetRequiredService<AIAssessmentUnitOfWork>());
            
        builder.Services.AddTransient<AIAssessmentInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseAIAssessmentPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<AIAssessmentDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddAIAssessmentPostgres(this IServiceCollection services)
    {
        services.AddScoped<IAIAssessmentRepository, AIAssessmentPostgresRepository>();
        return services
            .AddPostgres<AIAssessmentDbContext>();
    }
}
