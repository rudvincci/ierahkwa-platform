using Mamey.ApplicationName.BlazorWasm.Services;
using Mamey.BlazorWasm.Http;
using Microsoft.AspNetCore.Components.Authorization;
using ILocalStorageService = Blazored.LocalStorage.ILocalStorageService;

namespace Mamey.ApplicationName.BlazorWasm.Handlers;

public class UnauthorizedDelegatingHandler : DelegatingHandler
{
    private readonly ILogger<UnauthorizedDelegatingHandler> _logger;
    private readonly ILocalStorageService _localStorage;
    private const string TokenStorageKey = "authToken";

    private readonly AuthenticationStateProvider _customAuthenticationStateProvider;

    public UnauthorizedDelegatingHandler(ILogger<UnauthorizedDelegatingHandler> logger, AuthenticationStateProvider customAuthenticationStateProvider,
        ILocalStorageService localStorage)
    {
        _logger = logger;
        _customAuthenticationStateProvider = customAuthenticationStateProvider;
        _localStorage = localStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            var currentUser = await _localStorage.GetItemAsync<JsonWebToken>(TokenStorageKey);

            if(currentUser != null)
            {
                // await ((MameyAuthStateProvider) _customAuthenticationStateProvider).SetTokenAsync(null);
            }
        }

        return response;
    }
}