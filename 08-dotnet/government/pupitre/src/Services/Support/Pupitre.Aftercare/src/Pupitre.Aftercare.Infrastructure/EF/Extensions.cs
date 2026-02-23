using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.Aftercare.Domain.Repositories;
using Pupitre.Aftercare.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Aftercare.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddAftercarePlanPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<AftercarePlanDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(AftercarePlanDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_AftercarePlan", "aftercareplan");
            })
            .AddUnitOfWork<AftercarePlanUnitOfWork>();
        builder.Services.AddScoped<IAftercarePlanUnitOfWork>(provider => provider.GetRequiredService<AftercarePlanUnitOfWork>());
            
        builder.Services.AddTransient<AftercarePlanInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseAftercarePlanPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<AftercarePlanDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddAftercarePlanPostgres(this IServiceCollection services)
    {
        services.AddScoped<IAftercarePlanRepository, AftercarePlanPostgresRepository>();
        return services
            .AddPostgres<AftercarePlanDbContext>();
    }
}
