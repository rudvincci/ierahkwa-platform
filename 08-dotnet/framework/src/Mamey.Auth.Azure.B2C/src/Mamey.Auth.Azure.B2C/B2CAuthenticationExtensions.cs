using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;

namespace Mamey.Auth.Azure.B2C;

/// <summary>
/// Extension methods for setting up Azure B2C authentication in ASP.NET Core.
/// </summary>
public static class B2CAuthenticationExtensions
{
    public const string Section = "azureAdB2C";

    /// <summary>
    /// Adds Azure B2C authentication to the specified builder with optional anonymous access.
    /// </summary>
    /// <param name="builder">The builder to configure Azure B2C authentication.</param>
    /// <param name="allowAnonymousAccess">A value indicating whether to allow anonymous access by default.</param>
    /// <returns>The configured builder.</returns>
    public static IMameyBuilder AddB2BAuth(this IMameyBuilder builder, bool allowAnonymousAccess = true)
    {
        var b2cOptions = builder.GetOptions<AzureB2COptions>(Section);
        builder.Services.AddSingleton(b2cOptions);

        builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(config =>
            {
                config.CallbackPath = b2cOptions.CallbackPath;
                config.ClientId = b2cOptions.ClientId;
                config.ClientSecret = b2cOptions.ClientSecret;
                config.Domain = b2cOptions.Domain;
                config.Instance = b2cOptions.Instance;
                config.SignUpSignInPolicyId = b2cOptions.SignUpSignInPolicyId;
                config.EditProfilePolicyId = b2cOptions.EditProfilePolicyId;
                config.ResetPasswordPolicyId = b2cOptions.ResetPasswordPolicyId;
                config.SignedOutCallbackPath = b2cOptions.SignedOutCallbackPath;
                config.TenantId = b2cOptions.TenantId;
                config.Scope.Add("openid");
                config.Scope.Add("profile");
                config.ResponseType = "id_token";
            });

        builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();

        builder.Services.AddAuthorization(options =>
        {
            options.FallbackPolicy = allowAnonymousAccess
                ? new AuthorizationPolicyBuilder().RequireAssertion(context => true).Build()
                : options.DefaultPolicy;
        });

        return builder;
    }

    /// <summary>
    /// Adds Azure B2C authentication middleware to the application.
    /// </summary>
    /// <param name="app">The application to configure.</param>
    /// <returns>The configured application.</returns>
    public static WebApplication UseB2BAuth(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }
}