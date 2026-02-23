using Mamey.Persistence.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Identity.Web.UI;
using Microsoft.Identity.Web;
using Mamey.Auth.Abstractions;
using Mamey.Auth.Azure.B2B;
using Mamey.Auth.Azure.B2C;

namespace Mamey.Auth.Azure;

public static class Extensions
{
    private const string RegistryName = "auth.azure";
    
    /// <summary>
    /// Adds Azure authentication coordination (B2B, B2C, Azure AD) to the Mamey builder
    /// </summary>
    /// <param name="builder">The Mamey builder</param>
    /// <param name="sectionName">Configuration section name (default: "azure")</param>
    /// <returns>The Mamey builder for chaining</returns>
    public static IMameyBuilder AddAzure(this IMameyBuilder builder, string sectionName = "azure")
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = "azure";
        }

        var options = builder.GetOptions<AzureMultiAuthOptions>(sectionName);
        return builder.AddAzure(options);
    }
    
    /// <summary>
    /// Adds Azure authentication coordination (B2B, B2C, Azure AD) to the Mamey builder
    /// </summary>
    /// <param name="builder">The Mamey builder</param>
    /// <param name="options">Azure multi-authentication options</param>
    /// <returns>The Mamey builder for chaining</returns>
    public static IMameyBuilder AddAzure(this IMameyBuilder builder, AzureMultiAuthOptions options)
    {
        if (!builder.TryRegister(RegistryName))
        {
            return builder;
        }
        
        if (options == null)
        {
            return builder;
        }
        
        builder.Services.AddSingleton(options);
        
        // Register Azure AD B2B if enabled
        if (options.EnableAzureB2B)
        {
            builder.AddAzureB2B(options.AzureB2BSectionName, options.AzureB2BScheme);
        }
        
        // Register Azure AD B2C if enabled
        if (options.EnableAzureB2C)
        {
            builder.AddAzureB2C(options.AzureB2CSectionName, options.AzureB2CScheme);
        }
        
        // Register regular Azure AD if enabled
        if (options.EnableAzure)
        {
            builder.AddAzureAD(options.AzureSectionName, options.AzureScheme);
        }
        
        // Add Graph client if any Azure authentication is enabled
        if (options.EnableAzure || options.EnableAzureB2B || options.EnableAzureB2C)
        {
            var azureOptions = builder.GetOptions<AzureOptions>(options.AzureSectionName);
            if (azureOptions != null)
            {
                builder.AddGraphClient(azureOptions);
            }
        }
        
        // Register Azure authentication service
        builder.Services.AddScoped<IAzureAuthService, B2BAuthenticationService>();
        
        // Register Azure authentication middleware
        builder.Services.AddScoped<Middlewares.AzureAuthMiddleware>();
        
        return builder;
    }
    
    /// <summary>
    /// Uses Azure authentication middleware in the application pipeline
    /// </summary>
    public static IApplicationBuilder UseAzure(this IApplicationBuilder app)
    {
        app.UseMiddleware<Middlewares.AzureAuthMiddleware>();
        return app;
    }
    
    /// <summary>
    /// Adds regular Azure AD authentication
    /// </summary>
    private static IMameyBuilder AddAzureAD(this IMameyBuilder builder, string sectionName, string schemeName)
    {
        var azureOptions = builder.GetOptions<AzureOptions>(sectionName);
        if (azureOptions == null || !azureOptions.Enabled)
        {
            return builder;
        }
        
        // Register Azure AD authentication services
        // This is a placeholder for regular Azure AD authentication
        // Implementation can be added later if needed
        
        return builder;
    }
    
    /// <summary>
    /// Legacy method for backward compatibility
    /// </summary>
    [Obsolete("Use AddAzure() instead")]
    public static IMameyBuilder AddAzureAuthentication(this IMameyBuilder builder)
    {
        return builder.AddAzure();
    }

    /// <summary>
    /// Adds Microsoft Graph client to the service collection
    /// </summary>
    public static IMameyBuilder AddGraphClient(this IMameyBuilder builder, AzureOptions azureOptions)
    {
        if (azureOptions?.GraphOptions == null)
        {
            return builder;
        }
        
        builder.Services.AddSingleton(sp =>
        {
            return new GraphServiceClient(
                new HttpClient(),
                sp.GetRequiredService<IAuthenticationProvider>(),
                azureOptions.GraphOptions.BaseUrl);
        });

        return builder;
    }
}

public class GraphOptions : AzureOptions
{
    public string? BaseUrl { get; set; }
    public string? Instance { get; set; }
    public List<DownstreamApp>? DownstreamApps { get; set; }

}
public class DownstreamApp
{
    public string? Name { get; set; }
    public string? Scopes { get; set; }
}