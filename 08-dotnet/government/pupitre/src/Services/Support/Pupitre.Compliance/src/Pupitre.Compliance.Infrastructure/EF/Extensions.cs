using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.Compliance.Domain.Repositories;
using Pupitre.Compliance.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Compliance.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddComplianceRecordPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<ComplianceRecordDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(ComplianceRecordDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_ComplianceRecord", "compliancerecord");
            })
            .AddUnitOfWork<ComplianceRecordUnitOfWork>();
        builder.Services.AddScoped<IComplianceRecordUnitOfWork>(provider => provider.GetRequiredService<ComplianceRecordUnitOfWork>());
            
        builder.Services.AddTransient<ComplianceRecordInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseComplianceRecordPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<ComplianceRecordDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddComplianceRecordPostgres(this IServiceCollection services)
    {
        services.AddScoped<IComplianceRecordRepository, ComplianceRecordPostgresRepository>();
        return services
            .AddPostgres<ComplianceRecordDbContext>();
    }
}
