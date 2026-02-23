using System.Net;
using System.Net.Http.Headers;
using Mamey.Government.Identity.Contracts.Commands;

namespace Mamey.Government.Identity.BlazorWasm.Clients;

internal static class ApiClientFactory
{
    public static HttpClient CreateAuthenticatedClient(string apiBaseUrl, string cookieName, string cookieValue)
    {
        var cookieContainer = new CookieContainer();

        // Make sure the baseUri matches the domain used to issue the cookie
        var baseUri = new Uri(apiBaseUrl);
        cookieContainer.Add(baseUri, new Cookie(cookieName, cookieValue)
        {
            HttpOnly = true,
            Secure = true,
            Domain = baseUri.Host,
            Path = "/"
        });

        var handler = new HttpClientHandler
        {
            CookieContainer = cookieContainer,
            UseCookies = true
        };

        var client = new HttpClient(handler)
        {
            BaseAddress = baseUri
        };

        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        return client;
    }
}