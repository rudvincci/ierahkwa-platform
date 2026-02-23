using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mamey.Identity.EntityFramework.DbContexts;
using Mamey.Identity.EntityFramework.Entities;
using Mamey.Identity.EntityFramework.Stores;

namespace Mamey.Identity.EntityFramework;

public static class Extensions
{
    public static IServiceCollection AddIdentityEntityFramework<TContext>(
        this IServiceCollection services,
        string connectionString,
        Action<DbContextOptionsBuilder>? configureDbContext = null)
        where TContext : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        services.AddDbContext<TContext>(options =>
        {
            options.UseNpgsql(connectionString);
            configureDbContext?.Invoke(options);
        });

        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<TContext>()
            .AddUserStore<MameyUserStore<TContext>>()
            .AddRoleStore<MameyRoleStore<TContext>>()
            .AddClaimsPrincipalFactory<MameyClaimsPrincipalFactory>();

        return services;
    }

    public static IServiceCollection AddIdentityEntityFramework(
        this IServiceCollection services,
        string connectionString,
        Action<DbContextOptionsBuilder>? configureDbContext = null)
    {
        return services.AddIdentityEntityFramework<IdentityDbContext>(connectionString, configureDbContext);
    }
}
