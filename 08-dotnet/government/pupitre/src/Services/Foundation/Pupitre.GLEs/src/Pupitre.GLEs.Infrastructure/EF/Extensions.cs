using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.GLEs.Domain.Repositories;
using Pupitre.GLEs.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.GLEs.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddGLEPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<GLEDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(GLEDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_GLE", "gle");
            })
            .AddUnitOfWork<GLEUnitOfWork>();
        builder.Services.AddScoped<IGLEUnitOfWork>(provider => provider.GetRequiredService<GLEUnitOfWork>());
            
        builder.Services.AddTransient<GLEInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseGLEPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<GLEDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddGLEPostgres(this IServiceCollection services)
    {
        services.AddScoped<IGLERepository, GLEPostgresRepository>();
        return services
            .AddPostgres<GLEDbContext>();
    }
}
