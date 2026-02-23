using Mamey.Azure.Abstractions;
using Mamey.Graph.Providers;
using Mamey.Graph.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions.Authentication;

namespace Mamey.Graph;

public static class Extensions
{
    public static IMameyBuilder AddWebAssembyGraph(this IMameyBuilder builder,
        List<DownstreamApp> scopes, string sectionName = AzureOptions.APPSETTINGS_SECTION)
    {
        var graphOptions = builder.Services.GetOptions<GraphOptions>(sectionName);
        builder.Services.AddSingleton(graphOptions);


        if (string.IsNullOrEmpty(graphOptions.Type))
        {
            throw new ArgumentNullException(nameof(graphOptions.Type));
        }

        switch (graphOptions.Type)
        {
            case "client":
                builder.ConfigureClient(graphOptions);
                break;
            case "server":
                builder.ConfigureServer(graphOptions);
                break;
            default:
                break;
        }


        builder
            .AddAzure(sectionName)
            .AddGraphClient(scopes, graphOptions, sectionName);
        return builder;
    }

    public static IMameyBuilder AddGraphClient(this IMameyBuilder builder, List<DownstreamApp>? scopes = null, GraphOptions? graphOptions = null, string sectionName = AzureOptions.APPSETTINGS_SECTION)
    {
        //var sp = builder.Services.BuildServiceProvider();
        graphOptions = graphOptions is null ? builder.GetOptions<GraphOptions>("azureAd") : graphOptions;
        builder.Services.AddSingleton(graphOptions);

        //if (string.IsNullOrEmpty(graphOptions.BaseUrl) || string.IsNullOrEmpty(graphOptions.Scopes))
        //{
        //    return builder;
        //}

        builder.Services.AddMsalAuthentication(options =>
        {
            var configuration = builder.Services
                .BuildServiceProvider()
                .GetRequiredService<IConfiguration>();

            configuration.Bind(sectionName, options.ProviderOptions.Authentication);
            options.ProviderOptions.DefaultAccessTokenScopes.Add(graphOptions.Scopes);


            // ADD Scope for Bank Api Gateway app
            if (graphOptions.DownstreamApps is not null && graphOptions.DownstreamApps.Any())
            {
                if (!scopes.Any())
                {
                    scopes = graphOptions.DownstreamApps;
                }
                scopes.ForEach(downstramAppScopeUri => options
                    .ProviderOptions.AdditionalScopesToConsent.Add(downstramAppScopeUri.Scopes));
            }

        });

        builder.Services.AddTransient<IGraphService, GraphService>();
        //services.AddScoped<IAuthenticationProvider, GraphAuthenticationProvider>();

        builder.Services.AddScoped(sp =>
        {
            return new GraphServiceClient(
                new HttpClient(),
                sp.GetRequiredService<IAuthenticationProvider>(),
                graphOptions.BaseUrl);
        });


        // Make the same instance accessible as both AuthenticationStateProvider and TokenAuthenticationStateProvider

        builder.Services.AddTokenAuthenticationStateProvider();
        builder.Services.AddScoped<IBlazorAuthenticationService, BlazorAuthenticationService>();
        builder.Services.AddScoped<IJwtService, JwtService>();
        //services.AddSingleton<ITokenService, TokenService>();

        return builder;
    }

    public static void AddTokenAuthenticationStateProvider(this IServiceCollection services)
    {
        // Make the same instance accessible as both AuthenticationStateProvider and TokenAuthenticationStateProvider
        services.AddScoped<TokenAuthenticationStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<TokenAuthenticationStateProvider>());
    }

    private static IMameyBuilder ConfigureClient(this IMameyBuilder builder, GraphOptions graphOptions)
    {
        if (graphOptions.Enabled)
        {
            //if (graphOptions.Auth == "b2b")
            //{
            //    builder.AddAzureB2B();

            //}
            //else if (graphOptions.Auth == "b2c")
            //{
            //    builder.AddAzureB2C(graphOptions);
            //}
            //else
            //{

            //}
        }

        return builder;
    }
    private static IMameyBuilder ConfigurePublicClient(this IMameyBuilder builder)
    {



        return builder;
    }
    private static IMameyBuilder ConfigurePublicServer(this IMameyBuilder builder)
    {



        return builder;
    }
    private static IMameyBuilder ConfigureServer(this IMameyBuilder builder, GraphOptions graphOptions)
    {
        if (graphOptions.Enabled)
        {
            if (graphOptions.Auth == "b2b")
            {

            }
            else if (graphOptions.Auth == "b2c")
            {

            }
            else
            {

            }
        }
        return builder;
    }
    private static IMameyBuilder ConfigureConfidentialClient(this IMameyBuilder builder)
    {



        return builder;
    }
    private static IMameyBuilder ConfigureConfidentialServer(this IMameyBuilder builder)
    {



        return builder;
    }
    //private class GraphAuthenticationProvider : IAuthenticationProvider
    //{
    //    private readonly GraphOptions _graphOptions;
    //    public GraphAuthenticationProvider(IAccessTokenProvider tokenProvider,
    //    GraphOptions graphOptions)
    //    {
    //        TokenProvider = tokenProvider;
    //        _graphOptions = graphOptions;
    //    }


    //    public IAccessTokenProvider TokenProvider { get; }

    //    public async Task AuthenticateRequestAsync(RequestInformation request,
    //        Dictionary<string, object>? additionalAuthenticationContext = null,
    //        CancellationToken cancellationToken = default)
    //    {
    //        var result = await TokenProvider.RequestAccessToken(
    //            new AccessTokenRequestOptions()
    //            {
    //                Scopes = _graphOptions.Scopes
    //            });

    //        if (result.TryGetToken(out var token))
    //        {
    //            request.Headers.Add("Authorization",
    //                $"{CoreConstants.Headers.Bearer} {token.Value}");
    //        }
    //    }
    //}
}









// //public class TokenService : ITokenService
// //{
// //    private readonly ILocalStorageService _localStorageService;
// //    //private readonly TokenAuthenticationStateProvider _authenticationStateProvider;
// //    private readonly AppState _appState;

// //    private const string AccessTokenKey = "accessToken";
// //    private const string RefreshTokenKey = "refreshToken";

// //    public TokenService(ILocalStorageService localStorageService, AppState appState)
// //    {
// //        _localStorageService = localStorageService;
// //        // _authenticationStateProvider = (TokenAuthenticationStateProvider)authenticationStateProvider;
// //        _appState = appState;
// //        _appState.OnAccessTokenChange += RefreshTokenIfNeeded;
// //    }

// //    public async Task<string> GetAccessTokenAsync()
// //    {
// //        return await _localStorageService.GetItemAsync<string>(AccessTokenKey);
// //    }

// //    public async Task<string> GetRefreshTokenAsync()
// //    {
// //        return await _localStorageService.GetItemAsync<string>(RefreshTokenKey);
// //    }

// //    public async Task StoreAccessTokenAsync(string accessToken)
// //    {
// //        if (string.IsNullOrEmpty(accessToken))
// //            throw new ArgumentException("Invalid token. Token cannot be null or empty.");

// //        await _localStorageService.SetItemAsync(AccessTokenKey, accessToken);
// //        _appState.AccessToken = accessToken;
// //    }

// //    public async Task StoreRefreshTokenAsync(string refreshToken)
// //    {
// //        if (string.IsNullOrEmpty(refreshToken))
// //            throw new ArgumentException("Invalid token. Token cannot be null or empty.");

// //        await _localStorageService.SetItemAsync(RefreshTokenKey, refreshToken);
// //    }

// //    public async Task<string> RefreshAccessTokenAsync(string refreshToken)
// //    {
// //        // Implement the actual token refresh logic here, calling the identity provider's token refresh endpoint
// //        // Assuming that new tokens are returned and then stored
// //        var newAccessToken = "new_access_token"; // Placeholder for new access token
// //        await StoreAccessTokenAsync(newAccessToken);
// //        return newAccessToken;
// //    }

// //    public async Task ClearTokenAsync()
// //    {
// //        await _localStorageService.RemoveItemAsync(AccessTokenKey);
// //        await _localStorageService.RemoveItemAsync(RefreshTokenKey);
// //        _appState.AccessToken = null;
// //    }

// //    public bool NeedsRefresh(string refreshToken)
// //    {
// //        // Implement logic to determine if the refresh token needs refreshing, possibly based on expiry
// //        return true; // Placeholder implementation
// //    }

// //    private async void RefreshTokenIfNeeded()
// //    {
// //        var refreshToken = await GetRefreshTokenAsync();
// //        if (refreshToken != null && NeedsRefresh(refreshToken))
// //        {
// //            await RefreshAccessTokenAsync(refreshToken);
// //            //await _authenticationStateProvider.RefreshToken();
// //            //_authenticationStateProvider.NotifyAuthenticationStateChange(GetAuthenticationStateAsync());
// //        }
// //    }

// //    //private Task<AuthenticationState> GetAuthenticationStateAsync()
// //    //{
// //    //    return _authenticationStateProvider.GetAuthenticationStateAsync();
// //    //}
// //}
