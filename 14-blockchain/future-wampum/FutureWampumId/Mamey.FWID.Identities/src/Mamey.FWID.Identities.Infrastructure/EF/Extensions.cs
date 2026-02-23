using System;
using System.Linq;
using Mamey;
using Mamey.Postgres;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Infrastructure.EF.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Mamey.FWID.Identities.Infrastructure.EF;

public static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        builder.Services.AddIdentityPostgres();
        
        var scope = builder.Services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services
            .AddPostgres<IdentityDbContext>(builder =>
            {
                builder.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName);
                builder.MigrationsHistoryTable("__EFMigrationsHistory_Identity", "identity");
            })
            .AddUnitOfWork<IdentityUnitOfWork>();
        builder.Services.AddScoped<IIdentityUnitOfWork>(provider => provider.GetRequiredService<IdentityUnitOfWork>());
            
        builder.Services.AddTransient<IdentityInitializer>();
        return builder;
    }
    
    public static IApplicationBuilder UseIdentityPostgres(this IApplicationBuilder builder)
	{
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            // Check if seeding is enabled
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var environment = configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT");
            var postgresOptions = configuration.GetOptions<PostgresOptions>("postgres");
            var shouldSeed = environment == "Development" || postgresOptions?.Seed == true;
            
            if (shouldSeed)
            {
                var initializer = scope.ServiceProvider.GetRequiredService<IdentityInitializer>();
                // InitAsync will handle database creation/migration based on DEBUG/RELEASE mode
                initializer.InitAsync().GetAwaiter().GetResult();
            }
        }
		return builder;
	}
    private static IServiceCollection AddIdentityPostgres(this IServiceCollection services)
    {
        // Register concrete Postgres repositories (not interfaces - Composite repos handle interfaces)
        // Composite repos need these concrete implementations
        services.AddScoped<IdentityPostgresRepository>();
        services.AddScoped<PermissionMappingPostgresRepository>();
        services.AddScoped<IPermissionMappingRepository>(provider => 
            provider.GetRequiredService<PermissionMappingPostgresRepository>());
        services.AddScoped<SessionPostgresRepository>();
        services.AddScoped<MfaConfigurationPostgresRepository>();
        services.AddScoped<PermissionPostgresRepository>();
        services.AddScoped<RolePostgresRepository>();
        services.AddScoped<IdentityPermissionPostgresRepository>();
        services.AddScoped<IdentityRolePostgresRepository>();
        services.AddScoped<EmailConfirmationPostgresRepository>();
        services.AddScoped<SmsConfirmationPostgresRepository>();
        
        return services
            .AddPostgres<IdentityDbContext>();
    }
}
