using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.Fundraising.Domain.Repositories;
using Pupitre.Fundraising.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Fundraising.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddCampaignPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<CampaignDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(CampaignDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_Campaign", "campaign");
            })
            .AddUnitOfWork<CampaignUnitOfWork>();
        builder.Services.AddScoped<ICampaignUnitOfWork>(provider => provider.GetRequiredService<CampaignUnitOfWork>());
            
        builder.Services.AddTransient<CampaignInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseCampaignPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<CampaignDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddCampaignPostgres(this IServiceCollection services)
    {
        services.AddScoped<ICampaignRepository, CampaignPostgresRepository>();
        return services
            .AddPostgres<CampaignDbContext>();
    }
}
