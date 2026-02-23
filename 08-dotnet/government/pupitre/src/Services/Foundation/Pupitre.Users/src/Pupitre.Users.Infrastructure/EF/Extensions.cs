using System;
using Mamey;
using Mamey.Postgres;
using Pupitre.Users.Domain.Repositories;
using Pupitre.Users.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Users.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddUserPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<UserDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(UserDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_User", "user");
            })
            .AddUnitOfWork<UserUnitOfWork>();
        builder.Services.AddScoped<IUserUnitOfWork>(provider => provider.GetRequiredService<UserUnitOfWork>());
            
        builder.Services.AddTransient<UserInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseUserPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<UserDbContext>();
            dbContext.Database.Migrate();
        }
		return builder;
	}
    private static IServiceCollection AddUserPostgres(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserPostgresRepository>();
        return services
            .AddPostgres<UserDbContext>();
    }
}
