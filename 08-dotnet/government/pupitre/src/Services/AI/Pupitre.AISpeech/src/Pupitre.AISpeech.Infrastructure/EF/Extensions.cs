using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.AISpeech.Domain.Repositories;
using Pupitre.AISpeech.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AISpeech.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddSpeechRequestPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<SpeechRequestDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(SpeechRequestDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_SpeechRequest", "speechrequest");
            })
            .AddUnitOfWork<SpeechRequestUnitOfWork>();
        builder.Services.AddScoped<ISpeechRequestUnitOfWork>(provider => provider.GetRequiredService<SpeechRequestUnitOfWork>());
            
        builder.Services.AddTransient<SpeechRequestInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseSpeechRequestPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<SpeechRequestDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddSpeechRequestPostgres(this IServiceCollection services)
    {
        services.AddScoped<ISpeechRequestRepository, SpeechRequestPostgresRepository>();
        return services
            .AddPostgres<SpeechRequestDbContext>();
    }
}
