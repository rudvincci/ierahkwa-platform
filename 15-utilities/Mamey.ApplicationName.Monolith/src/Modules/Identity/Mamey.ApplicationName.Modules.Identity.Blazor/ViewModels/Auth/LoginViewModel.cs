using System.ComponentModel.DataAnnotations;
using System.Reactive;
using System.Reactive.Linq;
using Mamey.ApplicationName.Modules.Identity.Blazor.Services;
using Mamey.ApplicationName.Modules.Identity.Contracts.Commands;
using Mamey.Auth.Identity.Managers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MudBlazor;
using ReactiveUI;

namespace Mamey.ApplicationName.Modules.Identity.Blazor.ViewModels.Auth
{
    public class LoginViewModel : ReactiveObject
    {
        // private readonly IIdentityAuthService _authService;
        private readonly IIdentityRedirectManager _redirect;
        private readonly NavigationManager _nav;
        private readonly ISnackbar _snackbar;
        private readonly ILogger<LoginViewModel> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MameySignInManager _signInManager;

        private bool _isBusy;
        private string? _errorMessage;
        private bool _initialized;

        public LoginViewModel(
            // IIdentityAuthService authService,
            IIdentityRedirectManager redirect,
            NavigationManager nav,
            ISnackbar snackbar,
            ILogger<LoginViewModel> logger,
            IHttpContextAccessor httpContextAccessor, MameySignInManager signInManager)
        {
            // _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _redirect = redirect ?? throw new ArgumentNullException(nameof(redirect));
            _nav = nav ?? throw new ArgumentNullException(nameof(nav));
            _snackbar = snackbar ?? throw new ArgumentNullException(nameof(snackbar));
            _logger = logger;
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _signInManager = signInManager;

            Input = new LoginInputModel(_logger);
            if (_logger != null)
            {
                _logger.LogInformation("LoginViewModel created with instance ID: {InstanceId}", GetHashCode());
            }
            else
            {
                System.Console.WriteLine($"LoginViewModel created with instance ID: {GetHashCode()} (Logger is null)");
            }

            var canLogin = this.WhenAnyValue(
                vm => vm.Input.Username,
                vm => vm.Input.Password,
                vm => vm.IsBusy,
                (u, p, busy) =>
                    !string.IsNullOrWhiteSpace(u) &&
                    !string.IsNullOrWhiteSpace(p) &&
                    !busy)
                .DistinctUntilChanged();

            InitializeAsync = ReactiveCommand.CreateFromTask(InitializeImpl);
            LoginAsync = ReactiveCommand.CreateFromTask(LoginImpl, canLogin);

            canLogin.Subscribe(canLoginValue =>
            {
                if (_logger != null)
                {
                    _logger.LogInformation("CanLogin changed: {CanLogin}", canLoginValue);
                }
                else
                {
                    System.Console.WriteLine($"CanLogin changed: {canLoginValue}");
                }
            });

            InitializeAsync.ThrownExceptions.Subscribe(ex =>
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Error during initialization");
                }
            });
            LoginAsync.ThrownExceptions.Subscribe(ex =>
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Error during login");
                }
            });
        }

        public bool IsInitialized
        {
            get => _initialized;
            private set => this.RaiseAndSetIfChanged(ref _initialized, value);
        }

        public LoginInputModel Input { get; }

        public bool IsBusy
        {
            get => _isBusy;
            private set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }

        public string? ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        public string ReturnUrl { get; set; } = "/";

        public bool CanLogin => !string.IsNullOrWhiteSpace(Input.Username) &&
                               !string.IsNullOrWhiteSpace(Input.Password) &&
                               !IsBusy;

        public ReactiveCommand<Unit, Unit> InitializeAsync { get; }
        public ReactiveCommand<Unit, Unit> LoginAsync { get; }

        private async Task InitializeImpl()
        {
            if (_initialized)
                return;
            IsBusy = true;
            try
            {
                if (_logger != null)
                {
                    _logger.LogInformation("Initializing LoginViewModel");
                }
                if (_httpContextAccessor.HttpContext is not null && HttpMethods.IsGet(_httpContextAccessor.HttpContext.Request.Method))
                {
                    await _httpContextAccessor.HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                }
                _initialized = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoginImpl()
        {
            IsBusy = true;
            ErrorMessage = null;

            try
            {
                if (_logger != null)
                {
                    _logger.LogInformation("Login attempt with Username: {Username}, RememberMe: {RememberMe}", Input.Username, Input.RememberMe);
                }
                var result = await _signInManager.PasswordSignInAsync(Input.Username.Trim(), Input.Password.Trim(), Input.RememberMe, false);
                // var result = await _authService.LoginAsync(
                //     new Login(
                //         Input.Username.Trim(),
                //         Input.Password,
                //         Input.RememberMe,
                //         ReturnUrl),
                //     CancellationToken.None);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    _redirect.RedirectTo(ReturnUrl);
                }
                else if (result.RequiresTwoFactor)
                {
                    _redirect.RedirectTo(
                        "Account/LoginWith2fa",
                        new() { ["returnUrl"] = ReturnUrl, ["rememberMe"] = Input.RememberMe });
                }
                else if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    _redirect.RedirectTo("Account/Lockout");
                }
                else
                {
                    ErrorMessage = "Invalid login attempt.";
                }
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Error during login");
                }
                ErrorMessage = "Login failed. Please try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    public class LoginInputModel
    {
        private readonly ILogger<LoginViewModel>? _logger;

        public LoginInputModel(ILogger<LoginViewModel>? logger)
        {
            _logger = logger;
        }

        private string _username = string.Empty;
        [Required]
        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    if (_logger != null)
                    {
                        _logger.LogInformation("Username set to: {Value}", value);
                    }
                    else
                    {
                        System.Console.WriteLine($"Username set to: {value}");
                    }
                }
            }
        }

        private string _password = string.Empty;
        [Required]
        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    if (_logger != null)
                    {
                        _logger.LogInformation("Password set to: {Value}", value);
                    }
                    else
                    {
                        System.Console.WriteLine($"Password set to: {value}");
                    }
                }
            }
        }

        private bool _rememberMe;
        public bool RememberMe
        {
            get => _rememberMe;
            set
            {
                if (_rememberMe != value)
                {
                    _rememberMe = value;
                    if (_logger != null)
                    {
                        _logger.LogInformation("RememberMe set to: {Value}", value);
                    }
                    else
                    {
                        System.Console.WriteLine($"RememberMe set to: {value}");
                    }
                }
            }
        }
    }
}