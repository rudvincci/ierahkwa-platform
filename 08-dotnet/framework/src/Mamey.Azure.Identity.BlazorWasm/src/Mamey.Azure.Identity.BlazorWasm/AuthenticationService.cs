using Mamey.Azure.Abstractions;
using Microsoft.Identity.Client;

namespace Mamey.Azure.Identity.BlazorWasm;
public class AuthenticationService
{
    private readonly AzureBlazorWasmOptions _config;
    private readonly IConfidentialClientApplication _confidentialClientApplication;
    private readonly IPublicClientApplication _publicClientApplication;

    public AuthenticationService(AzureBlazorWasmOptions config)
    {
        _config = config;
        _publicClientApplication = PublicClientApplicationBuilder.Create(_config.ClientId)
            .WithRedirectUri(_config.RedirectUri)
            .WithAuthority($"{_config.Instance}{_config.Domain}")
            .Build();

        if (!string.IsNullOrEmpty(_config.ClientSecret))
        {
            _confidentialClientApplication = ConfidentialClientApplicationBuilder.Create(_config.ClientId)
                .WithClientSecret(_config.ClientSecret)
                .WithAuthority($"{_config.Instance}{_config.Domain}")
                .Build();
        }
    }

    public async Task<AuthenticationResult> AcquireTokenAsync()
    {
        AuthenticationResult result = null;
        switch (_config.Auth)
        {
            case "b2c":
                result = await AcquireTokenInteractiveAsync();
                break;
            case "b2b":
                if (_config.Type.Equals("client"))
                {
                    result = await AcquireTokenDeviceCodeAsync();
                }
                else if (_config.Type.Equals("server"))
                {
                    result = await AcquireTokenClientCredentialsAsync();
                }
                break;
        }
        return result;
    }

    private async Task<AuthenticationResult> AcquireTokenInteractiveAsync()
    {
        var scopes = _config.Scopes.Split(' ');
        return await _publicClientApplication.AcquireTokenInteractive(scopes).ExecuteAsync();
    }

    private async Task<AuthenticationResult> AcquireTokenDeviceCodeAsync()
    {
        var scopes = _config.Scopes.Split(' ');
        return await _publicClientApplication.AcquireTokenWithDeviceCode(scopes, callback => {
            Console.WriteLine(callback.Message);
            return Task.FromResult(0);
        }).ExecuteAsync();
    }

    private async Task<AuthenticationResult> AcquireTokenClientCredentialsAsync()
    {
        if (_confidentialClientApplication == null)
            throw new InvalidOperationException("Confidential Client is not configured properly.");

        var scopes = _config.Scopes.Split(' ');
        return await _confidentialClientApplication.AcquireTokenForClient(scopes).ExecuteAsync();
    }
}




public interface IAuthenticationService
{
    Task<string> AcquireAccessTokenAsync();
}

public class ClientCredentialsAuthService : IAuthenticationService
{
    private readonly AzureBlazorWasmOptions _config;
    private readonly IConfidentialClientApplication _app;

    public ClientCredentialsAuthService(AzureBlazorWasmOptions config)
    {
        _config = config;
        _app = ConfidentialClientApplicationBuilder.Create(_config.ClientId)
            .WithClientSecret(_config.ClientSecret)
            .WithAuthority(_config.Authority)
            .Build();
    }

    public async Task<string> AcquireAccessTokenAsync()
    {
        var result = await _app.AcquireTokenForClient(_config.DownstreamApi.Scopes.Split(" ")).ExecuteAsync();
        return result.AccessToken;
    }
}