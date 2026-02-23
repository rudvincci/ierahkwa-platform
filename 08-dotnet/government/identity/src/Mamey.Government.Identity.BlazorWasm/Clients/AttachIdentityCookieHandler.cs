using System.Net.Http.Headers;
using Mamey.Government.Identity.BlazorWasm.Providers;

namespace Mamey.Government.Identity.BlazorWasm.Clients;

public sealed class AttachIdentityCookieHandler : DelegatingHandler
{
  
    private readonly ApiAuthenticationStateProvider _auth;

    public AttachIdentityCookieHandler(ApiAuthenticationStateProvider auth) => _auth = auth;


    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var token = _auth.CurrentToken;
        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return base.SendAsync(request, ct);
    }
}