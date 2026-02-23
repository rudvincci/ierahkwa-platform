using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.AITranslation.Domain.Repositories;
using Pupitre.AITranslation.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AITranslation.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddTranslationRequestPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<TranslationRequestDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(TranslationRequestDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_TranslationRequest", "translationrequest");
            })
            .AddUnitOfWork<TranslationRequestUnitOfWork>();
        builder.Services.AddScoped<ITranslationRequestUnitOfWork>(provider => provider.GetRequiredService<TranslationRequestUnitOfWork>());
            
        builder.Services.AddTransient<TranslationRequestInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseTranslationRequestPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<TranslationRequestDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddTranslationRequestPostgres(this IServiceCollection services)
    {
        services.AddScoped<ITranslationRequestRepository, TranslationRequestPostgresRepository>();
        return services
            .AddPostgres<TranslationRequestDbContext>();
    }
}
