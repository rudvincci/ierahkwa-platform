using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Mamey.Identity.Azure.Configuration;
using Mamey.Identity.Azure.Services;
using Mamey.Identity.Azure.B2B;
using Mamey.Identity.Azure.B2C;
using Mamey.Identity.Azure.Graph;
using Mamey.Identity.Azure.Abstractions;
using Mamey.Identity.Core;
using Microsoft.Graph;

namespace Mamey.Identity.Azure;

public static class Extensions
{
    public static IMameyBuilder AddAzureAuthentication(this IMameyBuilder builder)
    {
        var azureOptions = builder.GetOptions<Configuration.AzureOptions>(Configuration.AzureOptions.APPSETTINGS_SECTION);
        
        if (azureOptions.Enabled)
        {
            switch (azureOptions.Type?.ToLowerInvariant())
            {
                case "b2b":
                    builder.AddB2BAuthentication(azureOptions);
                    break;
                case "b2c":
                    builder.AddB2CAuthentication(azureOptions);
                    break;
                default:
                    builder.AddAzureADAuthentication(azureOptions);
                    break;
            }
        }

        // Add Microsoft Graph client
        builder.AddGraphClient(azureOptions);
        
        // Register Azure authentication service
        builder.Services.AddScoped<IAzureAuthService, Services.AzureAuthService>();

        return builder;
    }

    public static IMameyBuilder AddB2BAuthentication(this IMameyBuilder builder, Configuration.AzureOptions azureOptions)
    {
        if (!azureOptions.Enabled)
            return builder;

        builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(options =>
            {
                options.Instance = azureOptions.Instance;
                options.TenantId = azureOptions.TenantId;
                options.ClientId = azureOptions.ClientId;
                options.ClientSecret = azureOptions.ClientSecret;
                options.CallbackPath = azureOptions.CallbackPath;
                options.SignedOutCallbackPath = azureOptions.SignedOutCallbackPath;
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAzureB2B", policy =>
                policy.RequireClaim("iss", $"{azureOptions.Instance}/{azureOptions.TenantId}/v2.0"));
        });

        return builder;
    }

    public static IMameyBuilder AddB2CAuthentication(this IMameyBuilder builder, Configuration.AzureOptions azureOptions)
    {
        if (!azureOptions.Enabled)
            return builder;

        builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(options =>
            {
                options.Instance = azureOptions.Instance;
                options.TenantId = azureOptions.TenantId;
                options.ClientId = azureOptions.ClientId;
                options.ClientSecret = azureOptions.ClientSecret;
                options.CallbackPath = azureOptions.CallbackPath;
                options.SignedOutCallbackPath = azureOptions.SignedOutCallbackPath;
                options.SignUpSignInPolicyId = azureOptions.SignUpSignInPolicyId;
                options.ResetPasswordPolicyId = azureOptions.ResetPasswordPolicyId;
                options.EditProfilePolicyId = azureOptions.EditProfilePolicyId;
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAzureB2C", policy =>
                policy.RequireClaim("iss", $"{azureOptions.Instance}/{azureOptions.TenantId}/v2.0"));
        });

        return builder;
    }

    public static IMameyBuilder AddAzureADAuthentication(this IMameyBuilder builder, Configuration.AzureOptions azureOptions)
    {
        if (!azureOptions.Enabled)
            return builder;

        builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(options =>
            {
                options.Instance = azureOptions.Instance;
                options.TenantId = azureOptions.TenantId;
                options.ClientId = azureOptions.ClientId;
                options.ClientSecret = azureOptions.ClientSecret;
                options.CallbackPath = azureOptions.CallbackPath;
                options.SignedOutCallbackPath = azureOptions.SignedOutCallbackPath;
            });

        return builder;
    }

    public static IMameyBuilder AddGraphClient(this IMameyBuilder builder, Configuration.AzureOptions azureOptions)
    {
        if (!azureOptions.Enabled)
            return builder;

        // Register Microsoft Graph client
        builder.Services.AddScoped<GraphServiceClient>();
        builder.Services.AddScoped<IGraphService, Graph.GraphService>();

        return builder;
    }
}
