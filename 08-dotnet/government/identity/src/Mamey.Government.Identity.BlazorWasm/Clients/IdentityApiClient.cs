using System.Net;
using System.Text;
using System.Text.Json;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Http;
using Mamey.Types;

namespace Mamey.Government.Identity.BlazorWasm.Clients;

internal class IdentityApiClient : IIdentityApiClient
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public IdentityApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<ApiResponse<AuthDto>> PasswordSignInAsync(SignInRequest signInRequest)
    {
        var json = JsonSerializer.Serialize(signInRequest, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/auth/sign-in", content);
        
        if (response.IsSuccessStatusCode)
        {
            var responseJson = await response.Content.ReadAsStringAsync();
            var authDto = JsonSerializer.Deserialize<AuthDto>(responseJson, JsonOptions);
            
            return new ApiResponse<AuthDto>(authDto, response, true, response.StatusCode);
        }
        
        var errorJson = await response.Content.ReadAsStringAsync();
        var error = JsonSerializer.Deserialize<ApiResponse.ErrorResponse>(errorJson, JsonOptions);
        
        return new ApiResponse<AuthDto>(default, response, false, response.StatusCode, error);
    }

    public Task<(bool IsConfirmed, string Message)> ConfirmEmailAsync(UserId userId, string code)
    {
        throw new NotImplementedException();
    }
    
    public async Task<(bool ok, CookieContainer jar, AuthDto? authDto)> LoginAndCaptureAsync(
        SignInRequest signInRequest)
    {
        var result = await PasswordSignInAsync(signInRequest);
        if (!result.Succeeded)
        {
            return (false, null!, null!);
        }

        // Note: CookieContainer handling would need to be implemented separately if needed
        return (result.Succeeded, null!, result.Value);
    }

    public async Task LogoutAsync(Guid userId)
    {
        var json = JsonSerializer.Serialize(userId, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        await _httpClient.PostAsync("/api/sign-out", content);
    }
}