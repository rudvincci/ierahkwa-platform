using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.Accessibility.Domain.Repositories;
using Pupitre.Accessibility.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Accessibility.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddAccessProfilePostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<AccessProfileDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(AccessProfileDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_AccessProfile", "accessprofile");
            })
            .AddUnitOfWork<AccessProfileUnitOfWork>();
        builder.Services.AddScoped<IAccessProfileUnitOfWork>(provider => provider.GetRequiredService<AccessProfileUnitOfWork>());
            
        builder.Services.AddTransient<AccessProfileInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseAccessProfilePostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<AccessProfileDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddAccessProfilePostgres(this IServiceCollection services)
    {
        services.AddScoped<IAccessProfileRepository, AccessProfilePostgresRepository>();
        return services
            .AddPostgres<AccessProfileDbContext>();
    }
}
