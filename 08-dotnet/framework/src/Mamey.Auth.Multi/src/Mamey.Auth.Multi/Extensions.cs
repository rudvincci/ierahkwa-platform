using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Mamey.Auth.Jwt;
using Mamey.Auth.DecentralizedIdentifiers;
using Mamey.Auth.Azure;
using Mamey.Auth.Identity;
using Mamey.Auth.Distributed;

namespace Mamey.Auth.Multi;

public static class Extensions
{
    private const string RegistryName = "auth.multi";
    private const string DefaultSectionName = "multiAuth";
    
    /// <summary>
    /// Adds multi-authentication coordination (JWT, DID, Azure, Identity, Distributed, Certificate) to the Mamey builder
    /// </summary>
    /// <param name="builder">The Mamey builder</param>
    /// <param name="sectionName">Configuration section name (default: "multiAuth")</param>
    /// <returns>The Mamey builder for chaining</returns>
    public static IMameyBuilder AddMultiAuth(this IMameyBuilder builder, string sectionName = DefaultSectionName)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = DefaultSectionName;
        }

        var options = builder.GetOptions<MultiAuthOptions>(sectionName);
        return builder.AddMultiAuth(options);
    }
    
    /// <summary>
    /// Adds multi-authentication coordination (JWT, DID, Azure, Identity, Distributed, Certificate) to the Mamey builder
    /// </summary>
    /// <param name="builder">The Mamey builder</param>
    /// <param name="options">Multi-authentication options</param>
    /// <returns>The Mamey builder for chaining</returns>
    public static IMameyBuilder AddMultiAuth(this IMameyBuilder builder, MultiAuthOptions options)
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
        
        // Register JWT authentication if enabled
        if (options.EnableJwt)
        {
            builder.AddJwt(options.JwtSectionName);
        }
        
        // Register DID authentication if enabled
        if (options.EnableDid)
        {
            builder.AddDecentralizedIdentifiers(options.DidSectionName);
        }
        
        // Register Azure authentication if enabled (delegates to Azure coordinator)
        if (options.EnableAzure)
        {
            builder.AddAzure(options.AzureSectionName);
        }
        
        // Register Identity authentication if enabled
        // TODO: Implement Identity authentication registration when available
        // if (options.EnableIdentity)
        // {
        //     builder.AddIdentity(options.IdentitySectionName);
        // }
        
        // Register Distributed authentication if enabled
        // TODO: Implement Distributed authentication registration when available
        // if (options.EnableDistributed)
        // {
        //     builder.AddDistributed(options.DistributedSectionName);
        // }
        
        // Register Certificate authentication if enabled
        // Note: Certificate authentication is typically handled through JWT or other methods
        // This is a placeholder for future implementation
        
        // Register multi-authentication middleware
        builder.Services.AddScoped<Middlewares.MultiAuthMiddleware>();
        
        return builder;
    }
    
    /// <summary>
    /// Uses multi-authentication middleware in the application pipeline
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder for chaining</returns>
    public static IApplicationBuilder UseMultiAuth(this IApplicationBuilder app)
    {
        app.UseMiddleware<Middlewares.MultiAuthMiddleware>();
        return app;
    }
}

