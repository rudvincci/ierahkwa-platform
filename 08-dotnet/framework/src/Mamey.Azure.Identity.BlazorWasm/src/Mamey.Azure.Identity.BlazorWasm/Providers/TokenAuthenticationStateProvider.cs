using System.Security.Claims;
using System.Text.Json;
using Mamey.Azure.Abstractions;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Identity.Client;
using Microsoft.JSInterop;

namespace Mamey.Azure.Identity.BlazorWasm.Providers;

public class TokenAuthenticationStateProvider : AuthenticationStateProvider
{
    private const string AuthToken = "authToken";
    private const string AuthTokenExpiry = "authTokenExpiry";

    private readonly IJSRuntime _jsRuntime;
    private readonly IPublicClientApplication _publicClientApplication;
    private readonly IConfidentialClientApplication _confidentialClientApplication;
    private readonly AzureBlazorWasmOptions _azureAdOptions;

    public TokenAuthenticationStateProvider(IJSRuntime jsRuntime,
        AzureBlazorWasmOptions azureAdOptions)
    {
        _jsRuntime = jsRuntime;
      
        _azureAdOptions = azureAdOptions;
        _publicClientApplication = PublicClientApplicationBuilder.Create(_azureAdOptions.ClientId)
            //.WithRedirectUri(_azureAdOptions.RedirectUri)
            .WithRedirectUri(_azureAdOptions.RedirectUri)
            .WithAuthority($"{_azureAdOptions.Authority}")
            .Build();

        if (!string.IsNullOrEmpty(_azureAdOptions.ClientSecret))
        {
            _confidentialClientApplication = ConfidentialClientApplicationBuilder.Create(_azureAdOptions.ClientId)
                .WithClientSecret(_azureAdOptions.ClientSecret)
                .WithAuthority($"{_azureAdOptions.Authority}")
                .Build();
        }
    }

    public async Task UpdateAuthenticationStateAsync(AuthenticationResult? authenticationResult)
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            var accounts = await _publicClientApplication.GetAccountsAsync();
            var result = await _publicClientApplication.AcquireTokenSilent(_azureAdOptions.Scopes?.Split(" "), accounts.FirstOrDefault()).ExecuteAsync();
            await UpdateAuthenticationStateAsync(result);
            return result?.AccessToken;
        }
        catch (MsalUiRequiredException)
        {
            //var result = await TriggerInteractiveAuthentication();
            //await UpdateAuthenticationStateAsync(result);
            //// Trigger a user interaction for login
            //return result?.AccessToken;
            return null;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error acquiring access token: {ex}");
            return null;
        }
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await this.GetTokenAsync();
        var identity = string.IsNullOrEmpty(token)
            ? new ClaimsIdentity()
            : new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        try
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            return keyValuePairs?.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())) ?? Enumerable.Empty<Claim>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing JWT: {ex}");
            return Enumerable.Empty<Claim>();
        }
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        return Convert.FromBase64String(base64);
    }
    //public async Task<AuthenticationResult?> TriggerInteractiveAuthentication()
    //{
    //    try
    //    {
    //        var result = await _publicClientApplication.AcquireTokenInteractive(_azureAdOptions.Scopes?.Split(" "))
    //            .WithPrompt(Prompt.SelectAccount)
    //            .ExecuteAsync();

    //        await UpdateAuthenticationStateAsync(result);

    //        // Provide feedback on successful login
    //        await _jsRuntime.InvokeVoidAsync("alert", "Login successful!");

    //        return result;
    //    }
    //    catch (MsalUiRequiredException uiEx)
    //    {
    //        // Provide specific feedback for user interaction requirements
    //        await _jsRuntime.InvokeVoidAsync("alert", "Additional user interaction is required.");
    //        Console.Error.WriteLine($"User interaction required: {uiEx}");
    //        return null;
    //    }
    //    catch (Exception ex)
    //    {
    //        // Provide feedback on failure
    //        //await _jsRuntime.InvokeVoidAsync("alert", "Login failed. Please try again.");
    //        Console.Error.WriteLine($"Interactive authentication failed: {ex}");
    //        return null;
    //    }
    //}
    //public async Task<string> CheckAndRefreshToken()
    //{
    //    try
    //    {
    //        var accounts = await _publicClientApplication.GetAccountsAsync();
    //        var silentResult = await _publicClientApplication.AcquireTokenSilent(new[] { "api-scope-here" }, accounts.FirstOrDefault()).ExecuteAsync();

    //        if (DateTimeOffset.UtcNow >= silentResult.ExpiresOn)
    //        {
    //            // Token is expiring soon or has expired - trigger interactive authentication
    //            return (await TriggerInteractiveAuthentication())?.AccessToken;
    //        }

    //        return silentResult.AccessToken;
    //    }
    //    catch (MsalUiRequiredException)
    //    {
    //        // Silent request has failed, user interaction required
    //        return (await TriggerInteractiveAuthentication())?.AccessToken;
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.Error.WriteLine("Failed to refresh token: " + ex);
    //        return null;
    //    }
    //}

}
