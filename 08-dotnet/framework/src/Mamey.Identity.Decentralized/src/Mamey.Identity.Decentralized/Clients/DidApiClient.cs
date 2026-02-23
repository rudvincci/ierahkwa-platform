using System.Net.Http.Json;
using Mamey.Identity.Decentralized.Core;
using Mamey.Identity.Decentralized.VC;

namespace Mamey.Identity.Decentralized.Clients;

public class DidApiClient
{
    private readonly HttpClient _http;

    public DidApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<DidDocument> CreateAsync(CreateDidRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/did", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DidDocument>();
    }

    public async Task<DidDocument> ResolveAsync(string did)
    {
        var response = await _http.GetAsync($"api/did/{did}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DidDocument>();
    }

    public async Task<DidDocument> UpdateAsync(string did, UpdateDidRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/did/{did}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DidDocument>();
    }

    public async Task DeactivateAsync(string did)
    {
        var response = await _http.DeleteAsync($"api/did/{did}");
        response.EnsureSuccessStatusCode();
    }
}