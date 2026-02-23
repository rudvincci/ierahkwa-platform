using System.Net.Http.Headers;
using Blazored.LocalStorage;
namespace Mamey.ApplicationName.BlazorWasm.Handlers;

public class AuthenticatedHttpClientHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;
    private const string TokenKey = "authToken";

    public AuthenticatedHttpClientHandler(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _localStorage.GetItemAsync<string>(TokenKey);

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Accept", "application/json");
            // request.Headers.Add("X-Custom-Header", "YourCustomValue");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
