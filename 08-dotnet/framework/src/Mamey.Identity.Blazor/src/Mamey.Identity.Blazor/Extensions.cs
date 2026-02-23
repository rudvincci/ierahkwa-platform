using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Mamey.Identity.Blazor.Configuration;
using Mamey.Identity.Blazor.Services;
using Mamey.Identity.Blazor.Components;
using Mamey.Identity.Core;

namespace Mamey.Identity.Blazor;

/// <summary>
/// Extension methods for configuring Blazor WebAssembly authentication.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds Blazor WebAssembly authentication services to the service collection.
    /// </summary>
    /// <param name="builder">The WebAssembly host builder.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The WebAssembly host builder.</returns>
    public static WebAssemblyHostBuilder AddMameyIdentityBlazor(this WebAssemblyHostBuilder builder, IConfiguration configuration)
    {
        builder.Services.Configure<BlazorIdentityOptions>(configuration.GetSection(BlazorIdentityOptions.SectionName));
        
        var options = configuration.GetSection(BlazorIdentityOptions.SectionName).Get<BlazorIdentityOptions>() ?? new BlazorIdentityOptions();
        
        // Register Blazor authentication services
        builder.Services.AddScoped<IBlazorAuthenticationService, BlazorAuthenticationService>();
        builder.Services.AddScoped<IBlazorTokenService, BlazorTokenService>();
        builder.Services.AddScoped<IBlazorUserService, BlazorUserService>();
        
        // Configure HttpClient for API calls
        builder.Services.AddHttpClient("MameyIdentity", client =>
        {
            client.BaseAddress = new Uri(options.ApiBaseUrl);
            client.DefaultRequestHeaders.Add("User-Agent", "Mamey.Identity.Blazor/1.0.0");
        });
        
        // Configure authentication state provider
        builder.Services.AddScoped<AuthenticationStateProvider, MameyAuthenticationStateProvider>();
        
        return builder;
    }
    
    /// <summary>
    /// Adds Blazor WebAssembly authentication services with custom configuration.
    /// </summary>
    /// <param name="builder">The WebAssembly host builder.</param>
    /// <param name="configureOptions">Action to configure options.</param>
    /// <returns>The WebAssembly host builder.</returns>
    public static WebAssemblyHostBuilder AddMameyIdentityBlazor(this WebAssemblyHostBuilder builder, Action<BlazorIdentityOptions> configureOptions)
    {
        builder.Services.Configure(configureOptions);
        
        var options = new BlazorIdentityOptions();
        configureOptions(options);
        
        // Register Blazor authentication services
        builder.Services.AddScoped<IBlazorAuthenticationService, BlazorAuthenticationService>();
        builder.Services.AddScoped<IBlazorTokenService, BlazorTokenService>();
        builder.Services.AddScoped<IBlazorUserService, BlazorUserService>();
        
        // Configure HttpClient for API calls
        builder.Services.AddHttpClient("MameyIdentity", client =>
        {
            client.BaseAddress = new Uri(options.ApiBaseUrl);
            client.DefaultRequestHeaders.Add("User-Agent", "Mamey.Identity.Blazor/1.0.0");
        });
        
        // Configure authentication state provider
        builder.Services.AddScoped<AuthenticationStateProvider, MameyAuthenticationStateProvider>();
        
        return builder;
    }
}
