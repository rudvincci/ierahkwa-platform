using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.Lessons.Domain.Repositories;
using Pupitre.Lessons.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Lessons.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddLessonPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<LessonDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(LessonDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_Lesson", "lesson");
            })
            .AddUnitOfWork<LessonUnitOfWork>();
        builder.Services.AddScoped<ILessonUnitOfWork>(provider => provider.GetRequiredService<LessonUnitOfWork>());
            
        builder.Services.AddTransient<LessonInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseLessonPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<LessonDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddLessonPostgres(this IServiceCollection services)
    {
        services.AddScoped<ILessonRepository, LessonPostgresRepository>();
        return services
            .AddPostgres<LessonDbContext>();
    }
}
