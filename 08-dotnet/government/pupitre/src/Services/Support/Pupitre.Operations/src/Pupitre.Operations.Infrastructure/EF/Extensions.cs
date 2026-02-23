using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.Operations.Domain.Repositories;
using Pupitre.Operations.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Operations.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddOperationMetricPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<OperationMetricDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(OperationMetricDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_OperationMetric", "operationmetric");
            })
            .AddUnitOfWork<OperationMetricUnitOfWork>();
        builder.Services.AddScoped<IOperationMetricUnitOfWork>(provider => provider.GetRequiredService<OperationMetricUnitOfWork>());
            
        builder.Services.AddTransient<OperationMetricInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseOperationMetricPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<OperationMetricDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddOperationMetricPostgres(this IServiceCollection services)
    {
        services.AddScoped<IOperationMetricRepository, OperationMetricPostgresRepository>();
        return services
            .AddPostgres<OperationMetricDbContext>();
    }
}
