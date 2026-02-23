using System.Text.Json;
using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using Mamey.ApplicationName.BlazorWasm.Configuration;
using Mamey.ApplicationName.BlazorWasm.Services.Auth;
using Mamey.ApplicationName.BlazorWasm.Services.Profile;
using Mamey.ApplicationName.BlazorWasm.Services.Roles;
using Mamey.ApplicationName.BlazorWasm.Services.Statements;
using Mamey.ApplicationName.BlazorWasm.Services.Support;
using Microsoft.JSInterop;
using MudBlazor.Services;

namespace Mamey.ApplicationName.BlazorWasm.Services;

internal static class Extensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddMudServices();
        // Register Blazored LocalStorage
        services.AddBlazoredLocalStorageAsSingleton(config =>
        {
            config.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            config.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            config.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
            config.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            config.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            config.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
            config.JsonSerializerOptions.WriteIndented = false;
        });
        
        services.AddSingleton<GlobalSettings>();
        services.AddSingleton<UserPreferenceService>();
        services.AddSingleton<NotificationService>();
        

        services.AddSingleton<UserService>();
        services.AddSingleton<SupportService>();
        services.AddSingleton<StatementService>();
        services.AddSingleton<RoleService>();
        
        services.AddScoped<CookieService>();

        return services;
    }
}
public class CookieService
{
    private readonly IJSRuntime _jsRuntime;

    public CookieService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Retrieves the specified cookie value from the browser.
    /// </summary>
    /// <param name="cookieName">The name of the cookie to retrieve.</param>
    /// <returns>The value of the cookie or null if not found.</returns>
    public async Task<string?> GetCookieAsync(string cookieName)
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>("App.getCookie", cookieName);
        }
        catch (JSException ex)
        {
            Console.WriteLine($"Error retrieving cookie: {ex.Message}");
            return null;
        }
    }
}