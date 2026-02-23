using System.Net.Http.Headers;
using System.Text.Json;
using Mamey.Http;
namespace Mamey.Graph;

public static class AuthenticatedHttpClientExtensions
{
    public static async Task<T> GetJsonAsync<T>(this IHttpClient httpClient, string url, AuthenticationHeaderValue authorization)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = authorization;

        var response = await httpClient.SendAsync(request);
        var responseBytes = await response.Content.ReadAsByteArrayAsync();
        return JsonSerializer.Deserialize<T>(responseBytes, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }
    internal static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
