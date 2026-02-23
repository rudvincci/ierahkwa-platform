// using System.Security.Claims;
// using System.IdentityModel.Tokens.Jwt;
// using Mamey.ApplicationName.BlazorWasm.Clients;
// using Microsoft.AspNetCore.Components.Authorization;
// using Microsoft.AspNetCore.Components;
// using Mamey.ApplicationName.BlazorWasm.Configuration;
// using Mamey.BlazorWasm.Http;
// using Microsoft.JSInterop;
// using System.Net.Http.Json;
// using Mamey.ApplicationName.BlazorWasm.ViewModels.Auth;
// using ZXing.Aztec.Internal;
// using ILocalStorageService = Blazored.LocalStorage.ILocalStorageService;
//
// namespace Mamey.ApplicationName.BlazorWasm.Services
// {
//     public class MameyAuthenticationStateProvider : AuthenticationStateProvider
//     {
//         private readonly HttpClient _httpClient;
//         private readonly ILocalStorageService _localStorage;
//         private readonly NavigationManager _navigationManager;
//         private AuthenticatedUser _authenticatedUser = new();
//         private readonly IJSRuntime _jsRuntime;
//
//         private const string TokenKey = "authToken";
//         private const string RefreshTokenKey = "refreshToken";
//         private const string ReturnUrlKey = "returnUrl";
//         private const string CurrentUserKey = "currentUser";
//         private const string AccessTokenCookieKey = "__access-token";
//
//         public MameyAuthenticationStateProvider(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage, NavigationManager navigationManager, IJSRuntime jsRuntime)
//         {
//             _httpClient = httpClientFactory.CreateClient("fhg-api");
//             _localStorage = localStorage;
//             _navigationManager = navigationManager;
//             _jsRuntime = jsRuntime;
//         }
//
//         public AuthenticatedUser AuthenticatedUser => _authenticatedUser;
//
//         public override async Task<AuthenticationState> GetAuthenticationStateAsync()
//         {
//             Console.WriteLine("Entering GetAuthenticationStateAsync");
//             
//             var accessTokenFromLocalStorage = await GetAccessTokenFromLocalStorage();
//
//             if (accessTokenFromLocalStorage == null || IsTokenExpired(accessTokenFromLocalStorage.AccessToken))
//             {
//                 return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
//             }
//
//             var claims = ParseClaimsFromJwt(accessTokenFromLocalStorage.AccessToken).ToList();
//             
//             
//             // List<Claim> claims = [
//             //     new Claim(ClaimTypes.NameIdentifier, accessTokenFromLocalStorage.Item2?.UserId!.ToString()!),
//             //     new Claim(ClaimTypes.Name, accessTokenFromLocalStorage.Item2?.Name.FullName!),
//             //     new Claim(ClaimTypes.Email, accessTokenFromLocalStorage.Item2?.Email!.ToString()!),
//             // ];
//             // Add role claims
//              //claims.Add(new Claim(ClaimTypes.Role, accessTokenFromLocalStorage.Item2?.Role));
//             // try
//             // {
//             //     var token = await _localStorage.GetItemAsync<string>(TokenKey);
//             //
//             //     if (string.IsNullOrEmpty(token) || IsTokenExpired(token))
//             //     {
//             //         Console.WriteLine("User is not authenticated or token is expired.");
//             //         _authenticatedUser = new AuthenticatedUser(); // Reset user
//             //         return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
//             //     }
//             //
//             //     UpdateAuthenticatedUserFromToken(token);
//             //     var claimsIdentity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwtAuthType");
//             //     var user = new ClaimsPrincipal(claimsIdentity);
//             //     return new AuthenticationState(user);
//             // }
//             // catch (Exception ex)
//             // {
//             //     Console.WriteLine($"Error in GetAuthenticationStateAsync: {ex.Message}");
//             //     return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
//             // }
//             var authenticationState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType: nameof(MameyAuthenticationStateProvider))));
//
//             return authenticationState;
//         }
//         
//         public async Task LoginAsync(LoginModel loginModel)
//         {
//             try
//             {
//                 
//                 var response = await _httpClient.PostAsJsonAsync($"/identity-module/sign-in", loginModel);
//                 if (response.IsSuccessStatusCode)
//                 {
//                     var result =
//                         await response.Content.ReadFromJsonAsync<AuthDto>(Mamey.JsonExtensions.SerializerOptions);
//                     if (result != null)
//                     {
//                     }
//                     await _localStorage.SetItemAsync<JsonWebToken>(TokenKey, result.Jwt);
//                     var claims = this.ParseClaimsFromJwt(result.Jwt.AccessToken).ToList();
//                     
//                     List<Claim> otherClaims = [
//                         new Claim(ClaimTypes.NameIdentifier, result.User?.UserId!.ToString()!),
//                         new Claim(ClaimTypes.Name, result.User?.Name.FullName!),
//                         new Claim(ClaimTypes.Email, result.User?.Email!.ToString()!),
//                         new Claim(ClaimTypes.Role, result.Jwt.Role)
//                     ];
//                    
//                     
//                     NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType: nameof(MameyAuthenticationStateProvider))))));
//                 }
//
//                 
//             }
//             catch (JSException ex)
//             {
//                 Console.WriteLine($"Error deleting cookie: {ex.Message}");
//             }
//         }
//         public async Task LogoutAsync()
//         {
//             try
//             {
//                 await _jsRuntime.InvokeVoidAsync("App.deleteCookie", AccessTokenCookieKey);
//                 await _localStorage.RemoveItemAsync(TokenKey);
//                 await _localStorage.RemoveItemAsync(RefreshTokenKey);
//                 NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
//             }
//             catch (JSException ex)
//             {
//                 Console.WriteLine($"Error deleting cookie: {ex.Message}");
//             }
//         }
//
//         private async Task InitializeAnonymousUser()
//         {
//             Console.WriteLine("Initializing anonymous user.");
//
//             // Ensure we have a default user even when not authenticated
//             _authenticatedUser = new AuthenticatedUser
//             {
//                 UserPreferences = await LoadUserPreferencesAsync()
//             };
//             NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
//         }
//
//         public async Task SetTokenAsync(string? token, string? refreshToken = null)
//         {
//             if (string.IsNullOrEmpty(token))
//             {
//                 await RemoveTokenAsync();
//                 return;
//             }
//
//             await _localStorage.SetItemAsync(TokenKey, token);
//
//             if (!string.IsNullOrEmpty(refreshToken))
//             {
//                 await _localStorage.SetItemAsync(RefreshTokenKey, refreshToken);
//             }
//
//             UpdateAuthenticatedUserFromToken(token);
//             NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
//
//             var returnUrl = await _localStorage.GetItemAsync<string>(ReturnUrlKey);
//             if (!string.IsNullOrEmpty(returnUrl))
//             {
//                 var authState = await GetAuthenticationStateAsync();
//                 if (authState.User.Identity?.IsAuthenticated ?? false)
//                 {
//                     await _localStorage.RemoveItemAsync(ReturnUrlKey);
//                     _navigationManager.NavigateTo(returnUrl, true);
//                 }
//             }
//         }
//
//         public async Task RemoveTokenAsync()
//         {
//             await _localStorage.RemoveItemAsync(TokenKey);
//             await _localStorage.RemoveItemAsync(RefreshTokenKey);
//             await _localStorage.RemoveItemAsync(ReturnUrlKey);
//
//             _authenticatedUser = new AuthenticatedUser
//             {
//                 UserPreferences = await LoadUserPreferencesAsync()
//             };
//             NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
//         }
//
//         private void UpdateAuthenticatedUserFromToken(string? token)
//         {
//             if (string.IsNullOrEmpty(token)) return;
//
//             var handler = new JwtSecurityTokenHandler();
//             var jwtToken = handler.ReadJwtToken(token);
//
//             _authenticatedUser.UserId = Guid.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value ?? Guid.Empty.ToString());
//             _authenticatedUser.Role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? string.Empty;
//             _authenticatedUser.UserPreferences = new UserPreferences(); // Load default preferences or from storage
//         }
//
//         private async Task<UserPreferences> LoadUserPreferencesAsync()
//         {
//             var storedPreferences = await _localStorage.GetItemAsync<UserPreferences>("userPreferences");
//             return storedPreferences ?? new UserPreferences();
//         }
//
//         private bool IsTokenExpired(string token)
//         {
//             var handler = new JwtSecurityTokenHandler();
//             var jwtToken = handler.ReadJwtToken(token);
//             return jwtToken.ValidTo < DateTime.UtcNow;
//         }
//
//         private IEnumerable<Claim> ParseClaimsFromJwt(string token)
//         {
//             var handler = new JwtSecurityTokenHandler();
//             var jwtToken = handler.ReadJwtToken(token);
//             return jwtToken.Claims;
//         }
//
//         
//
//         public async Task<JsonWebToken> GetAccessTokenFromLocalStorage()
//         {
//             // var cookie = await _jsRuntime.InvokeAsync<string>("App.getCookie", AccessTokenCookieKey);
//             
//             var accessToken = await _localStorage.GetItemAsync<JsonWebToken>(TokenKey);
//             return accessToken;
//         }
//         // public async Task SetCurrentUserAsync(AuthDto? authDto, bool useLocalStorage = false)
//         // { 
//         //     if (useLocalStorage)
//         //     {
//         //         await _localStorage.SetItemAsync(TokenKey, authDto.Jwt.AccessToken);
//         //         await _localStorage.SetItemAsync(RefreshTokenKey, authDto.Jwt.RefreshToken);
//         //     }
//         //     else
//         //     {
//         //         await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", TokenKey, authDto.Jwt.AccessToken);
//         //         await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", RefreshTokenKey, authDto.Jwt.RefreshToken);
//         //     }
//         //
//         //
//         //     // await _jsRuntime.InvokeVoidAsync("App.setCookie", AccessTokenCookieKey, authDto);
//         //     // await _localStorage.SetItemAsync<string>(AccessTokenCookieKey, authDto.Jwt.AccessToken);
//         //     // await _localStorage.SetItemAsync<string>(AccessTokenCookieKey, authDto.Jwt.AccessToken);
//         //     // await _localStorage.SetItemAsync(CurrentUserKey, authDto?.User);
//         //
//         //     NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
//         // }
//     }
//     public class TokenResponse
//     {
//         public string? AccessToken { get; set; }
//         public string? RefreshToken { get; set; }
//     }
// }
//
