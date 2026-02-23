// using Mamey.ApplicationName.Modules.Identity.Contracts.Commands;
// using Mamey.Auth.Identity.Managers;
// using Mamey.Blazor.Identity;
// using Mamey.Net.Http;
// using Microsoft.AspNetCore.Identity.Data;
//
// namespace Mamey.ApplicationName.Modules.Identity.Blazor.Clients;
//
// using System.Net.Http.Json;
// using Mamey.ApplicationName.Modules.Identity.Contracts.Dto;
//
// internal class AuthenticationApiClient : IAuthenticationApiClient<Login>
// {
//     private readonly HttpClient _http;
//
//     public AuthenticationApiClient(HttpClient http) => _http = http;
//
//     public async Task<MameySignInResult> LoginAsync(Login command, CancellationToken ct = default)
//     {
//         var content = new GenericHttpContent<Login>(command);
//         // var resp =  await _http.PostAsync("/api/identity/Auth/login", content, ct);
//         var response = await _http.PostAsJsonAsync("/api/identity/Auth/login", command, ct);
//         response.EnsureSuccessStatusCode();
//         return await response.Content.ReadFromJsonAsync<MameySignInResult>(cancellationToken: ct)
//                ?? throw new InvalidOperationException("Invalid login response");
//     }
//
//     public async Task ClearAuthenticationAsync(CancellationToken ct = default)
//     {
//         var response = await _http.PostAsync("api/identity/auth/logout", null, ct);
//         response.EnsureSuccessStatusCode();
//     }
// }