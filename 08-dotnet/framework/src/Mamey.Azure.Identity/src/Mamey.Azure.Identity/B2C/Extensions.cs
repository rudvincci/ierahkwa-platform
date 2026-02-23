using Mamey.Azure.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;

namespace Mamey.Azure.Identity.B2C;

public static class Extensions
{
    public static IMameyBuilder AddAzureB2C(this IMameyBuilder builder)
    {
        var sp = builder.Services
                .BuildServiceProvider();
        var config = sp.GetRequiredService<IConfiguration>();

        builder.Services.Configure<JwtBearerOptions>(
            JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters.NameClaimType = "name";
                options.TokenValidationParameters.RoleClaimType = "role";

            });
        var graphOptions = builder.Services.GetOptions<AzureOptions>(AzureOptions.APPSETTINGS_SECTION);
        builder.Services.AddSingleton(graphOptions);

        builder.Services.AddAuthorizationCore();
        //if (string.IsNullOrEmpty(graphOptions.Type))
        //{
        //    throw new ArgumentNullException(nameof(graphOptions.Type));
        //}        

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(config.GetSection("azureAd"))
                .EnableTokenAcquisitionToCallDownstreamApi()
                    .AddMicrosoftGraph(graphBaseUrl: graphOptions?.DownstreamApi?.BaseUrl, defaultScopes: graphOptions.Scopes)
                    .AddInMemoryTokenCaches();

        return builder;

    }
    
}