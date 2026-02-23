using Mamey.ApplicationName.BlazorWasm.Clients;
using Mamey.ApplicationName.BlazorWasm.Configuration;
using Mamey.ApplicationName.BlazorWasm.Handlers;
using Mamey.ApplicationName.BlazorWasm.Services;
using Mamey.ApplicationName.BlazorWasm.Services.Auth;
using Mamey;
using Mamey.Exceptions;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudExtensions.Services;
using Mamey.Http;
using Mamey.Types;

namespace Mamey.ApplicationName.BlazorWasm;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        // Register services
        builder.ConfigureServices();
        builder.Services.AddMudExtensions();

        // Build and run the application
        var app = builder.Build();
        await app.Services.GetRequiredService<GlobalSettings>().InitializeAsync();
        await app.RunAsync();
    }

    /// <summary>
    /// Extension method to register services.
    /// </summary>
    /// <param name="builder">The WebAssemblyHostBuilder instance.</param>
    /// <returns>The updated WebAssemblyHostBuilder.</returns>
    public static WebAssemblyHostBuilder ConfigureServices(this WebAssemblyHostBuilder builder)
    {
        var config = new ConfigurationBuilder()
            // appsettings.json is required
            .AddJsonFile("appsettings.json", optional: false)
            // appsettings.Development.json" is optional, values override appsettings.json
            .AddJsonFile($"appsettings.Development.json", optional: true)
            // User secrets are optional, values override both JSON files
            .Build();
        builder.Services.AddSingleton<IConfiguration>(config);
        builder.Services
            .AddMamey(configuration: config)
            .AddApiClients()
            ;
        builder.Services.AddScoped<AuthenticationService>();
        builder.Services.AddCascadingAuthenticationState();

        // Register the custom AuthenticationStateProvider
        // builder.Services.AddScoped<AuthenticationStateProvider, MameyAuthenticationStateProvider>();
        builder.Services.AddScoped<MameyAuthStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<MameyAuthStateProvider>());
        // Register the authentication service.

        builder.Services.AddServices();


        // Register the AuthenticatedHttpClientHandler
        builder.Services.AddScoped<AuthenticatedHttpClientHandler>();
        builder.Services.AddScoped<CookieDelegatingHandler>();
        builder.Services.AddScoped<UnauthorizedDelegatingHandler>();
        builder.Services.AddSingleton<GlobalSettings>();
        
        
        builder.Services.AddAuthorizationCore();
        
        
        // Register HttpClient with AuthenticatedHttpClientHandler
        
        // Fallback HttpClient for other usages
        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        

        return builder;
    }
}