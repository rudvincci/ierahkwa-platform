using Mamey.Persistence.OpenStack.OCS.Auth;
using Mamey.Persistence.OpenStack.OCS.Http;

namespace Mamey.Persistence.OpenStack.OCS.RequestHandler;

internal class RequestHandler : IRequestHandler
{
    private readonly HttpClient _httpClient;
    private readonly IAuthManager _authManager;

    public RequestHandler(OcsOptions ocsOptions, IHttpClientFactory httpClientFactory, IAuthManager authManager)
    {
        _authManager = authManager;
        _httpClient = httpClientFactory.CreateClient(ocsOptions.InternalHttpClientName);
    }

    public async Task<HttpRequestResult> Send(Func<IHttpRequestBuilder, IHttpRequestBuilder> httpRequestBuilder)
    {
        try
        {
            var authData = await _authManager.Authenticate();

            var httpRequest = httpRequestBuilder(new HttpRequestBuilder()
                    .WithHeader("X-Auth-Token", authData.SubjectToken))
                .Build();

            var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseContentRead);

#if DEBUG
            Console.WriteLine($"\n\n\n{await response.Content.ReadAsStringAsync()}\n\n\n");
#endif

            return new HttpRequestResult(response.IsSuccessStatusCode, response.StatusCode, response.Content);
        }
        catch (Exception ex)
        {
            return new HttpRequestResult(ex);
        }
    }
}