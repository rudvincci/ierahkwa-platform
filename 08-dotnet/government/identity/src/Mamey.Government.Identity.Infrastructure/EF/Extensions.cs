using System;
using Mamey.Postgres;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Government.Identity.Infrastructure.EF.Repositories;
using Mamey.Government.Identity.Infrastructure.Redis.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Identity.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<UserDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(UserDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_User", "public");
            })
            .AddUnitOfWork<UserUnitOfWork>();
        builder.Services.AddScoped<IUserUnitOfWork>(provider => provider.GetRequiredService<UserUnitOfWork>());
            
        builder.Services.AddUserPostgres();
        builder.Services.AddTransient<UserInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseUserPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var dbContext =  scope.ServiceProvider.GetRequiredService<UserInitializer>();
            dbContext.InitAsync().GetAwaiter().GetResult();

        }
		return builder;
	}
    private static IServiceCollection AddUserPostgres(this IServiceCollection services)
    {
        services.AddHostedService<SessionSyncService>();
        
        // Register concrete Postgres repositories (not interfaces - Composite repos handle interfaces)
        // Composite repos need these concrete implementations
        services.AddScoped<SubjectPostgresRepository>();
        services.AddScoped<UserPostgresRepository>();
        services.AddScoped<RolePostgresRepository>();
        services.AddScoped<PermissionPostgresRepository>();
        services.AddScoped<CredentialPostgresRepository>();
        services.AddScoped<SessionPostgresRepository>();
        services.AddScoped<EmailConfirmationPostgresRepository>();
        services.AddScoped<TwoFactorAuthPostgresRepository>();
        services.AddScoped<MultiFactorAuthPostgresRepository>();
        services.AddScoped<MfaChallengePostgresRepository>();
        
        return services
            .AddPostgres<UserDbContext>();
    }
}
