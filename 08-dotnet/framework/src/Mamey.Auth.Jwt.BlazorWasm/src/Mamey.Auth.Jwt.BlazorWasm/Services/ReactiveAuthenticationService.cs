namespace Mamey.Auth.Jwt.BlazorWasm.Services;

using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Mamey.Auth.Jwt.BlazorWasm.Models;
using Mamey.Auth.Jwt.BlazorWasm.Requests;
using Mamey.BlazorWasm.Http;
using Microsoft.AspNetCore.Components;
using ReactiveUI;

public class ReactiveAuthenticationService : ReactiveObject, IJwtAuthenticationService
{
    private readonly IUsersService _usersService;
    private readonly ILocalStorageService _localStorageService;
    private readonly NavigationManager _navigationManager;
    private readonly HttpClient _httpClient;

    public event Action<AuthenticatedUser?> AuthenticatedUserChanged;

    private string _authToken;

    public string AuthToken
    {
        get => _authToken;
        set => this.RaiseAndSetIfChanged(ref _authToken, value);
    }

    private string _refreshToken;

    public string RefreshToken
    {
        get => _refreshToken;
        set => this.RaiseAndSetIfChanged(ref _refreshToken, value);
    }

    public AuthDto UserAuthenticationResponse { get; private set; }

    public AuthenticatedUser? AuthenticatedUser { get; private set; }

    public bool IsAuthenticated { get; private set; }

    public ReactiveAuthenticationService(ILocalStorageService localStorageService, HttpClient httpClient)
    {
        _localStorageService = localStorageService;
        _httpClient = httpClient;
    }

    public async Task<string> RefreshTokenAsync()
    {
        RefreshToken = await _localStorageService.GetItemAsync<string>("refreshToken");

        if (!string.IsNullOrWhiteSpace(RefreshToken))
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://your-oauth-server.com/token/refresh")
            {
                Content = new StringContent(JsonSerializer.Serialize(new
                {
                    refresh_token = RefreshToken,
                    client_id = "your_client_id",
                    client_secret = "your_client_secret",
                    grant_type = "refresh_token"
                }), Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<JsonWebToken>(jsonResponse);

                AuthToken = tokenResponse.AccessToken;
                RefreshToken = tokenResponse.RefreshToken;

                await _localStorageService.SetItemAsync("authToken", AuthToken);
                await _localStorageService.SetItemAsync("refreshToken", RefreshToken);

                return AuthToken;
            }
        }
        return null;
    }

    public async Task RevokeTokensAsync()
    {
        var refreshToken = await _localStorageService.GetItemAsync<string>("refreshToken");
        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            var revokeRequest = new HttpRequestMessage(HttpMethod.Post, "https://your-oauth-server.com/revoke")
            {
                Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("token", refreshToken),
                    new KeyValuePair<string, string>("token_type_hint", "refresh_token"),
                    new KeyValuePair<string, string>("client_id", "your_client_id")
                })
            };
            var response = await _httpClient.SendAsync(revokeRequest);
            if (response.IsSuccessStatusCode)
            {
                await _localStorageService.RemoveItemAsync("refreshToken");
                await _localStorageService.RemoveItemAsync("authToken");
            }
        }
    }

    public async Task<bool?> LoginAsync(string username, string password)
    {
        var response = await _usersService.LoginAsync(new SignIn
        {
            Email = username,
            Password = password
        });

        if (response is null || response.HttpResponse.StatusCode == HttpStatusCode.BadGateway)
        {
            return null;
        }

        if (!response.Succeeded)
        {
            return false;
        }
        if (!response.Succeeded && response.Value.Type == "Customer" || response.Value.Type == "User")
        {
            return false;
        }

        UserAuthenticationResponse = new AuthDto
        {
            UserId = response.Value.UserId,
            OrganizationId = response.Value.OrganizationId,
            Name = response.Value.Name,
            Role = response.Value.Role,
            AccessToken = response.Value.AccessToken,
            RefreshToken = response.Value.RefreshToken,
            Type = response.Value.Type,
            Status = response.Value.Status,
            Expires = response.Value.Expires
        };
        await _localStorageService.SetItemAsync("user", UserAuthenticationResponse);
        IsAuthenticated = true;
        return true;
    }

    public async Task LogoutAsync()
    {
        AuthToken = null;
        RefreshToken = null;
        await _localStorageService.RemoveItemAsync("authToken");
        await _localStorageService.RemoveItemAsync("refreshToken");
        AuthenticatedUser = null;


        _navigationManager.NavigateTo("login");
    }

    public async Task InitializeAsync()
    {
        UserAuthenticationResponse = await _localStorageService.GetItemAsync<AuthDto>("user");
    }

    public Task SignIn(string usernanme, string password)
    {
        throw new NotImplementedException();
    }

    public void Logout()
    {
        throw new NotImplementedException();
    }

    public ClaimsPrincipal Authenticate(string credentials)
    {
        throw new NotImplementedException();
    }

    public bool Authorize(ClaimsPrincipal user, string resource)
    {
        throw new NotImplementedException();
    }
}
