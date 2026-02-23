using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.AIVision.Domain.Repositories;
using Pupitre.AIVision.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AIVision.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddVisionAnalysisPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<VisionAnalysisDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(VisionAnalysisDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_VisionAnalysis", "visionanalysis");
            })
            .AddUnitOfWork<VisionAnalysisUnitOfWork>();
        builder.Services.AddScoped<IVisionAnalysisUnitOfWork>(provider => provider.GetRequiredService<VisionAnalysisUnitOfWork>());
            
        builder.Services.AddTransient<VisionAnalysisInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseVisionAnalysisPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<VisionAnalysisDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddVisionAnalysisPostgres(this IServiceCollection services)
    {
        services.AddScoped<IVisionAnalysisRepository, VisionAnalysisPostgresRepository>();
        return services
            .AddPostgres<VisionAnalysisDbContext>();
    }
}
