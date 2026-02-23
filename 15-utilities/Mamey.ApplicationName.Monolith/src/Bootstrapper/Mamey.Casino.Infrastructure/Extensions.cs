// Infrastructure/Extensions.cs

using Mamey.Casino.Infrastructure.EF;
using Mamey.Casino.Infrastructure.Stores;
using Mamey.Persistence.Redis;
using Mamey.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Security.Claims;
using Mamey.Auth.Identity.Abstractions;
using Mamey.BlazorWasm;
// using Mamey.BlazorWasm;
// using Mamey.BlazorWasm.Api;
// using Mamey.BlazorWasm.Http;
using Mamey.Casino.Infrastructure.Services;
using Microsoft.AspNetCore.SignalR;

namespace Mamey.Casino.Infrastructure;

public static class Extensions
{
    public static IMameyBuilder AddCasinoInfrastructure(this IMameyBuilder builder)
    {
        builder.Services.AddSingleton<RolePermissions>(new RolePermissions(RolePermissionMapper.GenerateRolePermissionMapping()));
        var services = builder.Services;
        var appOpts = services.GetOptions<AppOptions>("app");
        var authOpts = services.GetOptions<AuthOptions>("auth");

        services.AddSingleton(appOpts);
        services.AddSingleton(authOpts);

        services.AddScoped<IRefreshTokenStore, RefreshTokenStore>();
        services.AddSingleton(
            new RolePermissions(RolePermissionMapper.GenerateRolePermissionMapping())
        );
        
        // configure SignalR
        services.AddSignalR();
        services.AddSingleton<IUserIdProvider, UserIdProvider>();
        builder.Services.AddScoped<INotificationService, NotificationService>();


        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IUserSettingsService, UserSettingsService>();
        builder.Services.AddScoped<ICache, RedisCache>();

        return builder
            // .AddEf()
            .AddRedis()
            .AddBlazorWasm();
    }

    public static async Task<IApplicationBuilder> UseCasinoInfrastructureAsync(this IApplicationBuilder app)
    {
        // ensures Postgres + Identity are set up, plus auth middleware
        // await app.UsePostgresDbAsync();

        return app;
    }
}

public static class RolePermissionMapper
{
    public static Dictionary<Type, Dictionary<string, long>> GenerateRolePermissionMapping()
    {
        var mappings = new Dictionary<Type, Dictionary<string, long>>();

        // Dynamically map roles to permissions for Bank and Member roles
        mappings[typeof(CasinoRole)] = MapRoleToPermission(typeof(CasinoRole));
        mappings[typeof(CasinoPermission)] = MapRoleToPermission(typeof(CasinoPermission));

        return mappings;
    }

    private static Dictionary<string, long> MapRoleToPermission(Type roleType)
    {
        var rolePermissions = new Dictionary<string, long>();

        // Iterate over all public constant fields in the roleType (BankRole or MemberRole)
        foreach (var field in roleType.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var roleName = field.Name; // Role name (e.g., "Teller", "Trustee")
            var roleValue = field.GetValue(null); // Corresponding permission value

            if (roleValue is Enum enumValue)
            {
                rolePermissions[roleName] = Convert.ToInt64(enumValue);
            }
        }

        return rolePermissions;
    }
}

public static class CasinoRole
{
    // Admin Role
    public const CasinoPermission Admin =
        CasinoPermission.All;


    // Customer Service Representative Role
    public const CasinoPermission User =
        CasinoPermission.None;
}

[Flags]
public enum CasinoPermission : long
{
    None = 0L,

    // Account Management
    ViewAllAccounts = 1L << 0,
    ViewIndividualAccounts = 1L << 1,
    All = 1L << 999
}

public class UserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection) =>
        connection.User?
            .FindFirst(ClaimTypes.NameIdentifier)
            ?.Value;
}