using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.Analytics.Domain.Repositories;
using Pupitre.Analytics.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Analytics.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddAnalyticPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<AnalyticDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(AnalyticDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_Analytic", "analytic");
            })
            .AddUnitOfWork<AnalyticUnitOfWork>();
        builder.Services.AddScoped<IAnalyticUnitOfWork>(provider => provider.GetRequiredService<AnalyticUnitOfWork>());
            
        builder.Services.AddTransient<AnalyticInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseAnalyticPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<AnalyticDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddAnalyticPostgres(this IServiceCollection services)
    {
        services.AddScoped<IAnalyticRepository, AnalyticPostgresRepository>();
        return services
            .AddPostgres<AnalyticDbContext>();
    }
}
