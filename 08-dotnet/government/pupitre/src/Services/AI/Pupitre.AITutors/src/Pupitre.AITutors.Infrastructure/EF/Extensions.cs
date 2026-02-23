using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.AITutors.Domain.Repositories;
using Pupitre.AITutors.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AITutors.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddTutorPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<TutorDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(TutorDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_Tutor", "tutor");
            })
            .AddUnitOfWork<TutorUnitOfWork>();
        builder.Services.AddScoped<ITutorUnitOfWork>(provider => provider.GetRequiredService<TutorUnitOfWork>());
            
        builder.Services.AddTransient<TutorInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseTutorPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<TutorDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddTutorPostgres(this IServiceCollection services)
    {
        services.AddScoped<ITutorRepository, TutorPostgresRepository>();
        return services
            .AddPostgres<TutorDbContext>();
    }
}
