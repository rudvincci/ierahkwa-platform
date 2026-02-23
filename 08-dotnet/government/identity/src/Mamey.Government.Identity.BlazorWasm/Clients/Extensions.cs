using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Identity.BlazorWasm.Clients;

internal static class Extensions
{
    private static string localUrl = "http://localhost:50001/";
    
    public static IMameyBuilder AddIdentityClient(this IMameyBuilder builder, string? apiUrl = null)
    {
        builder.Services.AddTransient<AttachIdentityCookieHandler>();
        builder.Services.AddScoped<IApiCookieJarStore, ApiCookieJarStore>();

        // One CookieContainer per *Blazor scope* (per-circuit) keeps user's session isolated.
        builder.Services.AddScoped<CookieContainer>();
        
        if (string.IsNullOrEmpty(apiUrl))
        {
            apiUrl = localUrl;
        }
        
        // Register HttpClient with the factory (simple pattern like ApplicationNameInternalClient)
        builder.Services.AddHttpClient<IIdentityApiClient, IdentityApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiUrl);
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.Timeout = TimeSpan.FromMinutes(5);
        }).AddHttpMessageHandler<AttachIdentityCookieHandler>();
        
        return builder;
    }
}