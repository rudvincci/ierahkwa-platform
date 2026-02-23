using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.AIContent.Domain.Repositories;
using Pupitre.AIContent.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AIContent.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddContentGenerationPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<ContentGenerationDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(ContentGenerationDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_ContentGeneration", "contentgeneration");
            })
            .AddUnitOfWork<ContentGenerationUnitOfWork>();
        builder.Services.AddScoped<IContentGenerationUnitOfWork>(provider => provider.GetRequiredService<ContentGenerationUnitOfWork>());
            
        builder.Services.AddTransient<ContentGenerationInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseContentGenerationPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<ContentGenerationDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddContentGenerationPostgres(this IServiceCollection services)
    {
        services.AddScoped<IContentGenerationRepository, ContentGenerationPostgresRepository>();
        return services
            .AddPostgres<ContentGenerationDbContext>();
    }
}
