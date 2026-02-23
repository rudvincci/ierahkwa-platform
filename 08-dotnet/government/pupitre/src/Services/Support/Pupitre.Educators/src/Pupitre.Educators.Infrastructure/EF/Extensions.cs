using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.Educators.Domain.Repositories;
using Pupitre.Educators.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Educators.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddEducatorPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<EducatorDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(EducatorDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_Educator", "educator");
            })
            .AddUnitOfWork<EducatorUnitOfWork>();
        builder.Services.AddScoped<IEducatorUnitOfWork>(provider => provider.GetRequiredService<EducatorUnitOfWork>());
            
        builder.Services.AddTransient<EducatorInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseEducatorPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<EducatorDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddEducatorPostgres(this IServiceCollection services)
    {
        services.AddScoped<IEducatorRepository, EducatorPostgresRepository>();
        return services
            .AddPostgres<EducatorDbContext>();
    }
}
