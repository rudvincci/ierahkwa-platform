using System.Net.Http.Headers;
using System.Net.Mime;
using Mamey.Persistence.OpenStack.OCS.Auth;
using Mamey.Persistence.OpenStack.OCS.Client;
using Mamey.Persistence.OpenStack.OCS.RequestHandler;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Persistence.OpenStack.OCS;

public static class Extensions
{
    private const string SectionName = "OcsClient";

    public static IMameyBuilder AddOcsClient(this IMameyBuilder builder, string sectionName = SectionName)
    {
        if (string.IsNullOrEmpty(sectionName))
        {
            sectionName = SectionName;
        }

        var ocsOptions = builder.GetOptions<OcsOptions>(sectionName);

        if (string.IsNullOrEmpty(ocsOptions.InternalHttpClientName))
        {
            ocsOptions.InternalHttpClientName = "OcsClient";
        }

        builder.Services.AddSingleton(ocsOptions);
        builder.Services.AddHttpClient(ocsOptions.InternalHttpClientName, c =>
        {
            c.BaseAddress = new Uri(ocsOptions.StorageUrl);
            c.DefaultRequestHeaders.UserAgent.Add(ProductInfoHeaderValue.Parse("OcsClient"));
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
        });
        builder.Services.AddTransient<IRequestHandler, RequestHandler.RequestHandler>();
        builder.Services.AddTransient<IOcsClient, OcsClient>();
        builder.Services.AddTransient<IAuthManager, AuthManager>();

        return builder;
    }
}