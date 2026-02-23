// File: ViewModels/B2BAuthViewModel.cs
using ReactiveUI;
using System.Reactive;
// using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Mamey.Auth.Azure.B2B.BlazorWasm.ViewModels
{
    /// <summary>
    /// ViewModel for handling Azure B2B authentication with ReactiveUI, including token refresh logic.
    /// </summary>
    public class B2BAuthViewModel : ReactiveObject
    {
        // private readonly IAccessTokenProvider _tokenProvider;
        private IDisposable _tokenRefreshSubscription;
        private readonly int _refreshRetryLimit = 3; // Retry refresh up to 3 times.
        private int _refreshAttempts = 0;

        // Allows customization of refresh interval before token expiration.
        public TimeSpan TokenRefreshBeforeExpiration { get; set; } = TimeSpan.FromMinutes(5); // Default is 5 minutes before expiration.

        private string _userName;
        public string UserName
        {
            get => _userName;
            private set => this.RaiseAndSetIfChanged(ref _userName, value);
        }

        private bool _isAuthenticated;
        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            private set => this.RaiseAndSetIfChanged(ref _isAuthenticated, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            private set => this.RaiseAndSetIfChanged(ref _isLoading, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            private set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; }

        // public B2BAuthViewModel(IAccessTokenProvider tokenProvider)
        // {
        //     _tokenProvider = tokenProvider;
        //
        //     LoginCommand = ReactiveCommand.CreateFromTask(LoginAsync, canExecute: this.WhenAnyValue(vm => vm.IsLoading).Select(loading => !loading));
        //     LogoutCommand = ReactiveCommand.CreateFromTask(LogoutAsync, canExecute: this.WhenAnyValue(vm => vm.IsLoading).Select(loading => !loading));
        //
        //     this.WhenAnyValue(x => x.IsAuthenticated)
        //         .Subscribe(_ => UpdateUserState());
        // }

        private async Task LoginAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                await Task.Delay(1000); // Simulate login delay.
                IsAuthenticated = true;
                UserName = "AzureUser";

                StartTokenExpirationMonitoring();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Login failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LogoutAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                await Task.Delay(1000); // Simulate logout delay.
                IsAuthenticated = false;
                UserName = null;

                StopTokenExpirationMonitoring();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Logout failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        // private async Task UpdateUserState()
        // {
        //     var result = await _tokenProvider.RequestAccessToken();
        //
        //     if (result.TryGetToken(out var token))
        //     {
        //         var expiration = token.ExpiresOn.UtcDateTime;
        //         var currentTime = DateTime.UtcNow;
        //
        //         if (currentTime < expiration)
        //         {
        //             UserName = "AzureUser";
        //             IsAuthenticated = true;
        //
        //             StartTokenExpirationMonitoring(expiration);
        //         }
        //         else
        //         {
        //             IsAuthenticated = false;
        //             ErrorMessage = "Token has expired.";
        //         }
        //     }
        //     else
        //     {
        //         IsAuthenticated = false;
        //         ErrorMessage = "Token could not be retrieved.";
        //     }
        // }

        private void StartTokenExpirationMonitoring(DateTime? expiration = null)
        {
            StopTokenExpirationMonitoring();

            var tokenExpiration = expiration ?? DateTime.UtcNow.AddMinutes(30);
            var timeUntilExpiration = tokenExpiration - DateTime.UtcNow;

            // _tokenRefreshSubscription = Observable.Timer(timeUntilExpiration - TokenRefreshBeforeExpiration)
            //     .Subscribe(async _ => await RefreshTokenAsync());
        }

        private void StopTokenExpirationMonitoring()
        {
            _tokenRefreshSubscription?.Dispose();
        }

        // private async Task RefreshTokenAsync()
        // {
        //     IsLoading = true;
        //     ErrorMessage = string.Empty;
        //
        //     if (_refreshAttempts >= _refreshRetryLimit)
        //     {
        //         ErrorMessage = "Failed to refresh the token after multiple attempts.";
        //         IsAuthenticated = false;
        //         StopTokenExpirationMonitoring();
        //         return;
        //     }
        //
        //     try
        //     {
        //         var result = await _tokenProvider.RequestAccessToken(new AccessTokenRequestOptions { ForceRefresh = true });
        //
        //         if (result.TryGetToken(out var refreshedToken))
        //         {
        //             UserName = "AzureUser";
        //             IsAuthenticated = true;
        //             _refreshAttempts = 0;
        //
        //             StartTokenExpirationMonitoring(refreshedToken.ExpiresOn.UtcDateTime);
        //         }
        //         else
        //         {
        //             _refreshAttempts++;
        //             ErrorMessage = "Failed to refresh the token, retrying...";
        //             await Task.Delay(2000);
        //             await RefreshTokenAsync();
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         _refreshAttempts++;
        //         ErrorMessage = $"Token refresh failed: {ex.Message}. Retrying...";
        //         await Task.Delay(2000);
        //         await RefreshTokenAsync();
        //     }
        //     finally
        //     {
        //         IsLoading = false;
        //     }
        // }
    }
}
