using System.IdentityModel.Tokens.Jwt;
using System.Reactive.Subjects;
using System.Security.Claims;
using Mamey.BlazorWasm.Http;
using Microsoft.AspNetCore.Components.Authorization;
using ILocalStorageService = Blazored.LocalStorage.ILocalStorageService;
using Mamey;
namespace Mamey.ApplicationName.BlazorWasm.Services;

public class MameyAuthStateProvider: AuthenticationStateProvider, IDisposable
    {
        private readonly ILocalStorageService _localStorage;
        private readonly BehaviorSubject<JsonWebToken> _tokenSubject;
        private Timer _refreshTimer;
        private readonly IDisposable _tokenSubscription;
        private const string TokenStorageKey = "authToken";

        public MameyAuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
            // Initialize without blocking. We'll load the token asynchronously later.
            _tokenSubject = new BehaviorSubject<JsonWebToken>(null);

            _tokenSubscription = _tokenSubject.Subscribe(_ =>
            {
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            });

            // Optionally, call an async init method.
            // Note: You cannot await here, so you'll need to call InitializeAsync() from a non-blocking context.
            _ = InitializeAsync();
        }
        public async Task InitializeAsync()
        {
            var token = await _localStorage.GetItemAsync<JsonWebToken>(TokenStorageKey);
            _tokenSubject.OnNext(token);
            if (token != null)
            {
                SetupRefreshTimer(token);
            }
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = _tokenSubject.Value;

            if (token != null && token.Expiry?.GetDate() > DateTime.UtcNow)
            {
                // Parse claims from the token.
                var claims = ParseClaimsFromJwt(token.AccessToken);
                var identity = new ClaimsIdentity(claims, "jwt");
                var user = new ClaimsPrincipal(identity);
                return new AuthenticationState(user);
            }
            else
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        private Claim[] ParseClaimsFromJwt(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            return token.Claims.ToArray();
        }

        public async Task MarkUserAsAuthenticatedAsync(JsonWebToken token)
        {
            // Persist token in local storage.
            await _localStorage.SetItemAsync(TokenStorageKey, token);
            _tokenSubject.OnNext(token);
            SetupRefreshTimer(token);
        }

        public async Task MarkUserAsLoggedOutAsync()
        {
            await _localStorage.RemoveItemAsync(TokenStorageKey);
            _tokenSubject.OnNext(null);
            _refreshTimer?.Dispose();
        }

        private void SetupRefreshTimer(JsonWebToken token)
        {
            _refreshTimer?.Dispose();
            var timeUntilExpiry = token.Expiry?.GetDate() - DateTime.UtcNow;
            var refreshTime = timeUntilExpiry - TimeSpan.FromMinutes(5);
            if (refreshTime < TimeSpan.Zero)
                refreshTime = TimeSpan.Zero;

            _refreshTimer = new Timer(async (_) =>
            {
                await RefreshTokenAsync();
            }, null, (TimeSpan)refreshTime, Timeout.InfiniteTimeSpan);
        }

        public async Task RefreshTokenAsync()
        {
            var currentToken = _tokenSubject.Value;
            if (currentToken != null)
            {
                // Replace the following simulation with a real API call to refresh the token.
                await Task.Delay(300);
                // For demonstration, we extend the expiration.
                currentToken.Expiry = DateTime.UtcNow.AddHours(1).ToUnixTimeMilliseconds();
                await _localStorage.SetItemAsync(TokenStorageKey, currentToken);
                _tokenSubject.OnNext(currentToken);
                SetupRefreshTimer(currentToken);
            }
        }

        public void Dispose()
        {
            _tokenSubscription?.Dispose();
            _refreshTimer?.Dispose();
            _tokenSubject?.Dispose();
        }
    }