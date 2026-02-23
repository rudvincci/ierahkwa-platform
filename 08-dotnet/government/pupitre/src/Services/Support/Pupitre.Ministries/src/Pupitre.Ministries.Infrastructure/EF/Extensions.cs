using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.Ministries.Domain.Repositories;
using Pupitre.Ministries.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Ministries.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddMinistryDataPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<MinistryDataDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(MinistryDataDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_MinistryData", "ministrydata");
            })
            .AddUnitOfWork<MinistryDataUnitOfWork>();
        builder.Services.AddScoped<IMinistryDataUnitOfWork>(provider => provider.GetRequiredService<MinistryDataUnitOfWork>());
            
        builder.Services.AddTransient<MinistryDataInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseMinistryDataPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<MinistryDataDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddMinistryDataPostgres(this IServiceCollection services)
    {
        services.AddScoped<IMinistryDataRepository, MinistryDataPostgresRepository>();
        return services
            .AddPostgres<MinistryDataDbContext>();
    }
}
