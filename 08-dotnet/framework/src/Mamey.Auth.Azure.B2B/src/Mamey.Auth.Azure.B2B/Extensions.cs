using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;


namespace Mamey.Auth.Azure.B2B;

public static class Extensions
{
    private const string RegistryName = "auth.azure.b2b";
    private const string DefaultSectionName = "azure:b2b";
    private const string DefaultSchemeName = "AzureB2B";
    
    /// <summary>
    /// Configures Azure AD B2B Authentication with OpenID Connect and JWT Bearer tokens.
    /// </summary>
    /// <param name="builder">The builder for configuring services.</param>
    /// <param name="sectionName">Configuration section name (default: "azure:b2b")</param>
    /// <param name="schemeName">Authentication scheme name (default: "AzureB2B")</param>
    /// <param name="allowAnonymousAccess">Allow anonymous access to certain pages by default.</param>
    public static IMameyBuilder AddAzureB2B(this IMameyBuilder builder, string sectionName = DefaultSectionName, string schemeName = DefaultSchemeName, bool allowAnonymousAccess = true)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = DefaultSectionName;
        }
        
        if (string.IsNullOrWhiteSpace(schemeName))
        {
            schemeName = DefaultSchemeName;
        }
        
        if (!builder.TryRegister(RegistryName))
        {
            return builder;
        }
        
        var b2bOptions = builder.GetOptions<AzureB2BOptions>(sectionName);
        if (b2bOptions == null || !b2bOptions.Enabled)
        {
            return builder;
        }
        
        builder.Services.AddSingleton(b2bOptions);

        // Add authentication services (OpenID Connect and JWT Bearer).
        builder.Services.AddAuthentication(schemeName)
            .AddMicrosoftIdentityWebApp(config =>
            {
                config.ClientId = b2bOptions.ClientId;
                config.ClientSecret = b2bOptions.ClientSecret;
                config.Domain = b2bOptions.Domain;
                config.Instance = b2bOptions.Instance;
                config.TenantId = b2bOptions.TenantId;
                config.CallbackPath = b2bOptions.CallbackPath;
                config.SignedOutCallbackPath = b2bOptions.SignedOutCallbackPath;
                config.Scope.Add("openid");
                config.Scope.Add("profile");
                config.ResponseType = "code"; // Authorization Code Flow
            });

        builder.Services.AddAuthorization(options =>
        {
            if (allowAnonymousAccess)
            {
                // Allow anonymous access by default
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAssertion(context => true) // Allow anonymous access
                    .Build();
            }
            else
            {
                // Enforce authorization by default
                options.FallbackPolicy = options.DefaultPolicy;
            }
        });

        // Add UI and controller support for authentication (optional)
        builder.Services.AddControllersWithViews()
            .AddMicrosoftIdentityUI();

        return builder;
    }

    /// <summary>
    /// Configures Azure AD B2B Authentication with OpenID Connect and JWT Bearer tokens.
    /// </summary>
    /// <param name="builder">The builder for configuring services.</param>
    /// <param name="options">Azure B2B options</param>
    /// <param name="schemeName">Authentication scheme name (default: "AzureB2B")</param>
    /// <param name="allowAnonymousAccess">Allow anonymous access to certain pages by default.</param>
    public static IMameyBuilder AddAzureB2B(this IMameyBuilder builder, AzureB2BOptions options, string schemeName = DefaultSchemeName, bool allowAnonymousAccess = true)
    {
        if (string.IsNullOrWhiteSpace(schemeName))
        {
            schemeName = DefaultSchemeName;
        }
        
        if (!builder.TryRegister(RegistryName))
        {
            return builder;
        }
        
        if (options == null || !options.Enabled)
        {
            return builder;
        }
        
        builder.Services.AddSingleton(options);

        // Add authentication services (OpenID Connect and JWT Bearer).
        builder.Services.AddAuthentication(schemeName)
            .AddMicrosoftIdentityWebApp(config =>
            {
                config.ClientId = options.ClientId;
                config.ClientSecret = options.ClientSecret;
                config.Domain = options.Domain;
                config.Instance = options.Instance;
                config.TenantId = options.TenantId;
                config.CallbackPath = options.CallbackPath;
                config.SignedOutCallbackPath = options.SignedOutCallbackPath;
                config.Scope.Add("openid");
                config.Scope.Add("profile");
                config.ResponseType = "code"; // Authorization Code Flow
            });

        builder.Services.AddAuthorization(authOptions =>
        {
            if (allowAnonymousAccess)
            {
                // Allow anonymous access by default
                authOptions.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAssertion(context => true) // Allow anonymous access
                    .Build();
            }
            else
            {
                // Enforce authorization by default
                authOptions.FallbackPolicy = authOptions.DefaultPolicy;
            }
        });

        // Add UI and controller support for authentication (optional)
        builder.Services.AddControllersWithViews()
            .AddMicrosoftIdentityUI();

        return builder;
    }
    
    /// <summary>
    /// Legacy method for backward compatibility
    /// </summary>
    [Obsolete("Use AddAzureB2B() instead")]
    public static IMameyBuilder AddB2BAuth(this IMameyBuilder builder, bool allowAnonymousAccess = true)
    {
        return builder.AddAzureB2B(DefaultSectionName, DefaultSchemeName, allowAnonymousAccess);
    }
    
    /// <summary>
    /// Use Azure AD B2B Authentication in the application pipeline.
    /// </summary>
    /// <param name="app">The WebApplication.</param>
    public static WebApplication UseB2BAuth(this WebApplication app)
    {
        // Ensure the app uses authentication and authorization.
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }
}
