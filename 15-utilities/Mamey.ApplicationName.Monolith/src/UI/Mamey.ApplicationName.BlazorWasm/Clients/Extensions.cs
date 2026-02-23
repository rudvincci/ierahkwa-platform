using Mamey.ApplicationName.BlazorWasm.Handlers;
using Mamey;
using Mamey.Exceptions;
using Mamey.Http;
using Mamey.Types;

namespace Mamey.ApplicationName.BlazorWasm.Clients;

public static class Extensions
{
    public static IMameyBuilder AddApiClients(this IMameyBuilder builder)
    {
        builder.AddHttpClient(httpClientBuilder: clientBuilder =>
        {
            var appOptions = builder.Services.GetOptions<AppOptions>("app");
            
            clientBuilder.ConfigureHttpClient(config =>
                {
                    
                    if(appOptions.OrganizationId is null)
                    {
                        throw new MameyException("Organization Id is null, check appsettings.json configuration");
                    }
                    config.BaseAddress = new Uri("http://localhost:51816/");
                    config.DefaultRequestHeaders.Add("X-ORG", appOptions.OrganizationId?.ToString());
                })
                .AddHttpMessageHandler<CookieDelegatingHandler>()
                .AddHttpMessageHandler<UnauthorizedDelegatingHandler>();
        }, clientName:"fhg-api");

        return builder;
    }
   
}