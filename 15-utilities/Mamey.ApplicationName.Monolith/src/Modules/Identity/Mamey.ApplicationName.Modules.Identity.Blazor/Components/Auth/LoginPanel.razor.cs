// using System.ComponentModel.DataAnnotations;
// using Microsoft.AspNetCore.Components;
// using Microsoft.AspNetCore.Components.Authorization;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Extensions.Logging;
// using MudBlazor;
// using Mamey.ApplicationName.Modules.Identity.Blazor.Services;
// using Mamey.ApplicationName.Modules.Identity.Contracts.Commands;
// using Mamey.Auth.Identity.Managers;
// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Identity;
//
// namespace Mamey.ApplicationName.Modules.Identity.Blazor.Components.Auth;
//
// [AllowAnonymous]
// public partial class LoginPanel : ComponentBase
// {
//     [Inject] private IIdentityAuthService? AuthService { get; set; }
//     [Inject] private IIdentityRedirectManager? RedirectManager { get; set; }
//     [Inject] private NavigationManager? _NavigationManager { get; set; }
//     [Inject] private ISnackbar? Snackbar { get; set; }
//
//     [Inject] private IHttpContextAccessor? HttpContextAccessor { get; set; }
//
//     [SupplyParameterFromQuery]
//     private string? ReturnUrl { get; set; }
//
//     private MudForm _form = new();
//     private bool _isBusy;
//     private string? _errorMessage;
//     private bool _initialized;
//     private string _username = string.Empty;
//     private string _password = string.Empty;
//     private bool _rememberMe;
//
//     [Required]
//     public string Username
//     {
//         get => _username;
//         set
//         {
//             if (_username != value)
//             {
//                 _username = value;
//                 LogInputChange("Username", value);
//                 StateHasChanged();
//             }
//         }
//     }
//
//     [Required]
//     public string Password
//     {
//         get => _password;
//         set
//         {
//             if (_password != value)
//             {
//                 _password = value;
//                 LogInputChange("Password", value);
//                 StateHasChanged();
//             }
//         }
//     }
//
//     public bool RememberMe
//     {
//         get => _rememberMe;
//         set
//         {
//             if (_rememberMe != value)
//             {
//                 _rememberMe = value;
//                 LogInputChange("RememberMe", value);
//                 StateHasChanged();
//             }
//         }
//     }
//
//     public bool IsBusy
//     {
//         get => _isBusy;
//         private set
//         {
//             _isBusy = value;
//             StateHasChanged();
//         }
//     }
//
//     public string? ErrorMessage
//     {
//         get => _errorMessage;
//         private set
//         {
//             _errorMessage = value;
//             StateHasChanged();
//         }
//     }
//
//     public bool CanLogin => !string.IsNullOrWhiteSpace(Username) &&
//                            !string.IsNullOrWhiteSpace(Password) &&
//                            !IsBusy;
//
//     public LoginPanel()
//     {
//             // Console.WriteLine("LoginPanel constructor called");
//         
//        
//     }
//
//     protected override async Task OnInitializedAsync()
//     {
//        
//             // Console.WriteLine("LoginPanel OnInitializedAsync called");
//         
//
//         if (!_initialized)
//         {
//             await InitializeAsync();
//         }
//     }
//
//     private async Task InitializeAsync()
//     {
//         if (_initialized)
//             return;
//         IsBusy = true;
//         try
//         {
//   
//                 // Console.WriteLine("Initializing LoginPanel");
//             if (HttpContextAccessor?.HttpContext != null && HttpMethods.IsGet(HttpContextAccessor.HttpContext.Request.Method))
//             {
//                 await HttpContextAccessor.HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
//             }
//             _initialized = true;
//         }
//         finally
//         {
//             IsBusy = false;
//             // Console.WriteLine("CanLogin after initialization: {CanLogin}", CanLogin.ToString());
//  
//         }
//     }
//
//     private async Task LoginUserAsync()
//     {
//         if (AuthService == null || RedirectManager == null )
//         {
//             System.Console.WriteLine("LoginUserAsync: AuthService, RedirectManager, or Logger is null");
//             return;
//         }
//
//         // Console.WriteLine("LoginUserAsync called with Username: {Username}, Password: {Password}, RememberMe: {RememberMe}, CanLogin: {CanLogin}",
//         //     Username, Password, RememberMe, CanLogin);
//
//         await _form.Validate();
//         if (_form.IsValid)
//         {
//             IsBusy = true;
//             ErrorMessage = null;
//             try
//             {
//                 // Console.WriteLine("Login attempt with Username: {Username}, RememberMe: {RememberMe}", Username, RememberMe);
//                 var result = await AuthService.LoginAsync(
//                     new Login(
//                         Username.Trim(),
//                         Password,
//                         RememberMe,
//                         ReturnUrl ?? "/"),
//                     CancellationToken.None);
//
//                 if (result.Succeeded)
//                 {
//                     // Console.WriteLine("User logged in.");
//                     RedirectManager.RedirectTo(result.RedirectUrl);
//                 }
//                 else
//                 {
//                     ErrorMessage = "Invalid login attempt.";
//                     Snackbar?.Add("Invalid login attempt.", Severity.Error);
//                 }
//             }
//             catch (Exception ex)
//             {
//                 // Console.WriteLine("Error during login");
//                 ErrorMessage = "Login failed. Please try again.";
//                 Snackbar?.Add("Login failed. Please try again.", Severity.Error);
//             }
//             finally
//             {
//                 IsBusy = false;
//             }
//         }
//         else
//         {
//             ErrorMessage = "Please correct the form errors.";
//             // Console.WriteLine("Form validation failed");
//             Snackbar?.Add("Please correct the form errors.", Severity.Warning);
//         }
//     }
//
//     private void LogInputChange<T>(string property, T value)
//     {
//        
//             // Console.WriteLine("Input changed: {Property} = {Value}", property, value);
//     }
//
//     private void DebugInputValues()
//     {
//        
//
//         // Console.WriteLine("Debug Input Values: Username = {Username}, Password = {Password}, RememberMe = {RememberMe}, CanLogin = {CanLogin}",
//         //     Username, Password, RememberMe, CanLogin);
//     }
// }