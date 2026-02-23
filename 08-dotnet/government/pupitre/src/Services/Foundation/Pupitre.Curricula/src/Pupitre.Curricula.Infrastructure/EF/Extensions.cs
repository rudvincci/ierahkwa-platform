using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.Curricula.Domain.Repositories;
using Pupitre.Curricula.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Curricula.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddCurriculumPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<CurriculumDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(CurriculumDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_Curriculum", "curriculum");
            })
            .AddUnitOfWork<CurriculumUnitOfWork>();
        builder.Services.AddScoped<ICurriculumUnitOfWork>(provider => provider.GetRequiredService<CurriculumUnitOfWork>());
            
        builder.Services.AddTransient<CurriculumInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseCurriculumPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<CurriculumDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddCurriculumPostgres(this IServiceCollection services)
    {
        services.AddScoped<ICurriculumRepository, CurriculumPostgresRepository>();
        return services
            .AddPostgres<CurriculumDbContext>();
    }
}
