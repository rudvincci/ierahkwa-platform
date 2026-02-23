using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.Notifications.Domain.Repositories;
using Pupitre.Notifications.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Notifications.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddNotificationPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<NotificationDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(NotificationDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_Notification", "notification");
            })
            .AddUnitOfWork<NotificationUnitOfWork>();
        builder.Services.AddScoped<INotificationUnitOfWork>(provider => provider.GetRequiredService<NotificationUnitOfWork>());
            
        builder.Services.AddTransient<NotificationInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseNotificationPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddNotificationPostgres(this IServiceCollection services)
    {
        services.AddScoped<INotificationRepository, NotificationPostgresRepository>();
        return services
            .AddPostgres<NotificationDbContext>();
    }
}
