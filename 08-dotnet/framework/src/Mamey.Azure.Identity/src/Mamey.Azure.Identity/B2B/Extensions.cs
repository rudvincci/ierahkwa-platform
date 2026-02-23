using Mamey.Azure.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;

namespace Mamey.Azure.Identity.B2B;

public static class Extensions
{
    public static IMameyBuilder AddAzureB2B(this IMameyBuilder builder)
    {
        var sp = builder.Services
                .BuildServiceProvider();
        var configuration = sp.GetRequiredService<IConfiguration>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(configuration.GetSection(AzureOptions.APPSETTINGS_SECTION))
                .EnableTokenAcquisitionToCallDownstreamApi()
                    .AddMicrosoftGraph(configuration.GetSection("downstreamApi"))
                    .AddInMemoryTokenCaches();

        return builder;
    }
}

