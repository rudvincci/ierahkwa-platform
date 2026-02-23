using Mamey;
﻿using System;
using Mamey.Postgres;
using Pupitre.Bookstore.Domain.Repositories;
using Pupitre.Bookstore.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Bookstore.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddBookPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<BookDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(BookDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_Book", "book");
            })
            .AddUnitOfWork<BookUnitOfWork>();
        builder.Services.AddScoped<IBookUnitOfWork>(provider => provider.GetRequiredService<BookUnitOfWork>());
            
        builder.Services.AddTransient<BookInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseBookPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<BookDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddBookPostgres(this IServiceCollection services)
    {
        services.AddScoped<IBookRepository, BookPostgresRepository>();
        return services
            .AddPostgres<BookDbContext>();
    }
}
