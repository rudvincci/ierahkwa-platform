using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Mamey.Auth.Azure.B2C;

public static class Extensions
{
    private const string RegistryName = "auth.azure.b2c";
    private const string DefaultSectionName = "azure:b2c";
    private const string DefaultSchemeName = "AzureB2C";
    
    /// <summary>
    /// Configures Azure AD B2C Authentication with OpenID Connect and JWT Bearer tokens.
    /// </summary>
    /// <param name="builder">The builder for configuring services.</param>
    /// <param name="sectionName">Configuration section name (default: "azure:b2c")</param>
    /// <param name="schemeName">Authentication scheme name (default: "AzureB2C")</param>
    /// <param name="allowAnonymousAccess">Allow anonymous access to certain pages by default.</param>
    public static IMameyBuilder AddAzureB2C(this IMameyBuilder builder, string sectionName = DefaultSectionName, string schemeName = DefaultSchemeName, bool allowAnonymousAccess = true)
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
        
        var b2cOptions = builder.GetOptions<AzureB2COptions>(sectionName);
        if (b2cOptions == null || !b2cOptions.Enabled)
        {
            return builder;
        }
        
        builder.Services.AddSingleton(b2cOptions);
        
        // Add authentication services
        builder.Services.AddAuthentication(schemeName)
            .AddMicrosoftIdentityWebApp(config =>
            {
                config.CallbackPath = b2cOptions.CallbackPath;
                config.ClientId = b2cOptions.ClientId;
                config.ClientSecret = b2cOptions.ClientSecret;
                config.Domain = b2cOptions.Domain;
                config.EditProfilePolicyId = b2cOptions.EditProfilePolicyId;
                config.Instance = b2cOptions.Instance;
                config.ResetPasswordPolicyId = b2cOptions.ResetPasswordPolicyId;
                config.SignedOutCallbackPath = b2cOptions.SignedOutCallbackPath;
                config.SignUpSignInPolicyId = b2cOptions.SignUpSignInPolicyId;
                config.TenantId = b2cOptions.TenantId;
                config.Scope.Add("openid");
                config.Scope.Add("profile");
                config.ResponseType = "id_token";
                config.SaveTokens = true;
                config.Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProvider = context => Task.CompletedTask,
                    OnMessageReceived = context => Task.CompletedTask,
                    OnTicketReceived = context => Task.CompletedTask,
                    OnTokenResponseReceived = context => Task.CompletedTask,
                    OnTokenValidated = context =>
                    {
                        var claimsIdentity = context.Principal?.Identity as System.Security.Claims.ClaimsIdentity;
                        if (claimsIdentity != null)
                        {
                            claimsIdentity.AddClaim(new System.Security.Claims.Claim("customClaim", "customValue"));
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.Redirect("/error?message=authentication_failed");
                        context.HandleResponse();
                        return Task.CompletedTask;
                    },
                    OnAuthorizationCodeReceived = context => Task.CompletedTask,
                    OnRedirectToIdentityProviderForSignOut = context => Task.CompletedTask,
                    OnUserInformationReceived = context => Task.CompletedTask,
                    OnRemoteSignOut = context => Task.CompletedTask,
                    OnAccessDenied = context => Task.CompletedTask,
                    OnRemoteFailure = context => Task.CompletedTask,
                    OnSignedOutCallbackRedirect = context => Task.CompletedTask
                };
            })
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddInMemoryTokenCaches();

        builder.Services.AddControllersWithViews()
            .AddMicrosoftIdentityUI();
        
        builder.Services.AddAuthorization(authOptions =>
        {
            if (allowAnonymousAccess)
            {
                authOptions.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAssertion(context => true)
                    .Build();
            }
            else
            {
                authOptions.FallbackPolicy = authOptions.DefaultPolicy;
            }
        });
        
        builder.Services.AddServerSideBlazor()
            .AddMicrosoftIdentityConsentHandler();
            
        return builder;
    }
    
    /// <summary>
    /// Configures Azure AD B2C Authentication with OpenID Connect and JWT Bearer tokens.
    /// </summary>
    /// <param name="builder">The builder for configuring services.</param>
    /// <param name="options">Azure B2C options</param>
    /// <param name="schemeName">Authentication scheme name (default: "AzureB2C")</param>
    /// <param name="allowAnonymousAccess">Allow anonymous access to certain pages by default.</param>
    public static IMameyBuilder AddAzureB2C(this IMameyBuilder builder, AzureB2COptions options, string schemeName = DefaultSchemeName, bool allowAnonymousAccess = true)
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
        
        // Add authentication services
        builder.Services.AddAuthentication(schemeName)
            .AddMicrosoftIdentityWebApp(config =>
            {
                config.CallbackPath = options.CallbackPath;
                config.ClientId = options.ClientId;
                config.ClientSecret = options.ClientSecret;
                config.Domain = options.Domain;
                config.EditProfilePolicyId = options.EditProfilePolicyId;
                config.Instance = options.Instance;
                config.ResetPasswordPolicyId = options.ResetPasswordPolicyId;
                config.SignedOutCallbackPath = options.SignedOutCallbackPath;
                config.SignUpSignInPolicyId = options.SignUpSignInPolicyId;
                config.TenantId = options.TenantId;
                config.Scope.Add("openid");
                config.Scope.Add("profile");
                config.ResponseType = "id_token";
                config.SaveTokens = true;
                config.Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProvider = context => Task.CompletedTask,
                    OnMessageReceived = context => Task.CompletedTask,
                    OnTicketReceived = context => Task.CompletedTask,
                    OnTokenResponseReceived = context => Task.CompletedTask,
                    OnTokenValidated = context =>
                    {
                        var claimsIdentity = context.Principal?.Identity as System.Security.Claims.ClaimsIdentity;
                        if (claimsIdentity != null)
                        {
                            claimsIdentity.AddClaim(new System.Security.Claims.Claim("customClaim", "customValue"));
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.Redirect("/error?message=authentication_failed");
                        context.HandleResponse();
                        return Task.CompletedTask;
                    },
                    OnAuthorizationCodeReceived = context => Task.CompletedTask,
                    OnRedirectToIdentityProviderForSignOut = context => Task.CompletedTask,
                    OnUserInformationReceived = context => Task.CompletedTask,
                    OnRemoteSignOut = context => Task.CompletedTask,
                    OnAccessDenied = context => Task.CompletedTask,
                    OnRemoteFailure = context => Task.CompletedTask,
                    OnSignedOutCallbackRedirect = context => Task.CompletedTask
                };
            })
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddInMemoryTokenCaches();

        builder.Services.AddControllersWithViews()
            .AddMicrosoftIdentityUI();
        
        builder.Services.AddAuthorization(authOptions =>
        {
            if (allowAnonymousAccess)
            {
                authOptions.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAssertion(context => true)
                    .Build();
            }
            else
            {
                authOptions.FallbackPolicy = authOptions.DefaultPolicy;
            }
        });
        
        builder.Services.AddServerSideBlazor()
            .AddMicrosoftIdentityConsentHandler();
            
        return builder;
    }
    
    /// <summary>
    /// Legacy method for backward compatibility
    /// </summary>
    [Obsolete("Use AddAzureB2C() instead")]
    public static IMameyBuilder AddB2CAuth(this IMameyBuilder builder, bool allowAnonymousAccess = true)
    {
        return builder.AddAzureB2C(DefaultSectionName, DefaultSchemeName, allowAnonymousAccess);
    }

    /// <summary>
    /// Use Azure AD B2C Authentication in the application pipeline.
    /// </summary>
    /// <param name="app">The WebApplication.</param>
    public static WebApplication UseB2CAuth(this WebApplication app)
    {
        app.UseAuthorization();
        return app;
    }
}
