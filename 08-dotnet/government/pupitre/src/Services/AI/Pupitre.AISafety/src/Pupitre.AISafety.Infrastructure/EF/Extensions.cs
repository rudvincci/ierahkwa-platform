using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.AISafety.Domain.Repositories;
using Pupitre.AISafety.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AISafety.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddSafetyCheckPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<SafetyCheckDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(SafetyCheckDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_SafetyCheck", "safetycheck");
            })
            .AddUnitOfWork<SafetyCheckUnitOfWork>();
        builder.Services.AddScoped<ISafetyCheckUnitOfWork>(provider => provider.GetRequiredService<SafetyCheckUnitOfWork>());
            
        builder.Services.AddTransient<SafetyCheckInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseSafetyCheckPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<SafetyCheckDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddSafetyCheckPostgres(this IServiceCollection services)
    {
        services.AddScoped<ISafetyCheckRepository, SafetyCheckPostgresRepository>();
        return services
            .AddPostgres<SafetyCheckDbContext>();
    }
}
