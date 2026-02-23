// MyApp.Client/Services/ReactiveAuthenticationService.cs
using Mamey.Http;
using Mamey.Auth.Jwt.BlazorWasm.Models;
using Mamey.Auth.Jwt.BlazorWasm.Requests;

namespace Mamey.Auth.Jwt.BlazorWasm.Services;
internal class UsersService : IUsersService
{
    private readonly IHttpClient _client;

    public UsersService(IHttpClient client)
    {
        _client = client;
    }

    public Task<ApiResponse<UserDetailsDto>> GetAsync(Guid userId)
        => _client.GetApiResponseAsync<UserDetailsDto>($"users-service/users/{userId}");

    public Task<ApiResponse> RegisterAsync(RegisterRequest request)
        => _client.PostApiResponseAsync("users-service/register", request);

    public Task<ApiResponse<AuthDto>> LoginAsync(SignIn request)
        => _client.PostApiResponseAsync<AuthDto>("users-service/sign-in", request);

    // public Task<ApiResponse<PagedDto<UserDto>>> BrowseUsersAsync(Guid? userId = null)
    //     => _client.GetApiResponseAsync<PagedDto<UserDto>>($"users-service/users?userId={userId}");

    public Task<ApiResponse> RecoverPasswordAsync(RecoverPasswordRequest request)
        => _client.PostApiResponseAsync("users-service/recover-password", request);

    public Task<ApiResponse> ResetPasswordAsync(ResetPasswordRequest request, string token)
        => _client.PostApiResponseAsync("users-service/reset-password", request, new[] { new KeyValuePair<string, string>("validation", token) });

    public Task<ApiResponse<AuthDto>> UseRefreshTokenAsync(UseRefreshTokenRequest request)
        => _client.PostApiResponseAsync<AuthDto>("users-service/refresh-tokens/use", request);
}