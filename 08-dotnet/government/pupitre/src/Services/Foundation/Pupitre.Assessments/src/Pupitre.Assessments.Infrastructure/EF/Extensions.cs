using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.Assessments.Domain.Repositories;
using Pupitre.Assessments.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Assessments.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddAssessmentPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<AssessmentDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(AssessmentDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_Assessment", "assessment");
            })
            .AddUnitOfWork<AssessmentUnitOfWork>();
        builder.Services.AddScoped<IAssessmentUnitOfWork>(provider => provider.GetRequiredService<AssessmentUnitOfWork>());
            
        builder.Services.AddTransient<AssessmentInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseAssessmentPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<AssessmentDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddAssessmentPostgres(this IServiceCollection services)
    {
        services.AddScoped<IAssessmentRepository, AssessmentPostgresRepository>();
        return services
            .AddPostgres<AssessmentDbContext>();
    }
}
