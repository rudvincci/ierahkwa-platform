using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Text;
using Mamey.ApplicationName.BlazorWasm.Clients;
using Mamey.ApplicationName.BlazorWasm.ViewModels.Auth;
using Mamey.BlazorWasm.Http;
using Mamey.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using ILocalStorageService = Blazored.LocalStorage.ILocalStorageService;

namespace Mamey.ApplicationName.BlazorWasm.Services.Auth;

public class AuthenticationService
{
    private readonly MameyAuthStateProvider _authStateProvider;
        private readonly ILocalStorageService _localStorage;
        private readonly IHttpClient _httpClient;
        private readonly IApiResponseHandler _responseHandler;

        public AuthenticationService(MameyAuthStateProvider authStateProvider, ILocalStorageService localStorage, IHttpClient httpClient, IApiResponseHandler responseHandler)
        {
            _authStateProvider = authStateProvider;
            _localStorage = localStorage;
            _httpClient = httpClient;
            _responseHandler = responseHandler;
        }

        // Performs login and stores the token.
        public async Task LoginAsync(LoginModel loginModel)
        {
            // In production, post the credentials to your API endpoint and get a JWT token in response.
  

            // For demonstration, we create a dummy token.
            try
            {
                
                var result = await _responseHandler.HandleAsync( _httpClient.PostApiResponseAsync<AuthDto?>($"/identity-module/sign-in", loginModel));
                
                if (result is not null)
                {
                    
                    // var result =
                    //     await result.Content.ReadFromJsonAsync<AuthDto>(Mamey.JsonExtensions.SerializerOptions);
                    // if (result != null)
                    // {
                    // }
                    // await _localStorage.SetItemAsync<JsonWebToken>(TokenKey, result.Jwt);
                    // var claims = this.ParseClaimsFromJwt(result.Jwt.AccessToken).ToList();
                    //
                    // List<Claim> otherClaims = [
                    //     new Claim(ClaimTypes.NameIdentifier, result.User?.UserId!.ToString()!),
                    //     new Claim(ClaimTypes.Name, result.User?.Name.FullName!),
                    //     new Claim(ClaimTypes.Email, result.User?.Email!.ToString()!),
                    //     new Claim(ClaimTypes.Role, result.Jwt.Role)
                    // ];
                    await _authStateProvider.MarkUserAsAuthenticatedAsync(result.Jwt);
                   
                    // NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType: nameof(MameyAuthenticationStateProvider))))));
                }

                
            }
            catch (JSException ex)
            {
                Console.WriteLine($"Error deleting cookie: {ex.Message}");
            }
        }

        // Logs out the user.
        public async Task LogoutAsync()
        {
            await _authStateProvider.MarkUserAsLoggedOutAsync();
        }

        // Refreshes the token.
        public async Task RefreshTokenAsync()
        {
            await _authStateProvider.RefreshTokenAsync();
        }

    }
