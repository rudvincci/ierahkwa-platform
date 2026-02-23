using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Mamey.ApplicationName.BlazorWasm.Clients;
using Mamey.ApplicationName.BlazorWasm.Configuration;
using Mamey.ApplicationName.BlazorWasm.Services;
using Mamey.ApplicationName.BlazorWasm.Services.Auth;
using Mamey.ApplicationName.BlazorWasm.ViewModels.Auth;
using Microsoft.JSInterop;
using MudBlazor;

namespace Mamey.ApplicationName.BlazorWasm.Components.Auth
{
    public partial class Login : ComponentBase
    {
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
        [Inject] private IHttpClientFactory HttpClientFactory { get; set; } = default!;
        [Inject] private ILocalStorageService LocalStorage { get; set; } = default!;
        [Inject] private IJSRuntime JsRuntime { get; set; } = default!;
        [Inject] private AuthenticationService AuthenticationService { get; set; } = default!;
        private string message = string.Empty;
        private bool _rememberMe = false;
        MudForm form = new MudForm();
        private bool isSubmitting = false;
        private bool hasTriedSubmitting = false;

        private string emailError = string.Empty;
        private string passwordError = string.Empty;
        private ElementReference hiddenInput;
        private AuthViewModel authViewModel;
        
        /// <summary>
        /// If a Return URL is given, we will navigate there after login.
        /// </summary>
        [SupplyParameterFromQuery(Name = "returnUrl")]
        private string? ReturnUrl { get; set; }
        
        
        /// <summary>
        /// The Model the Form is going to bind to.
        /// </summary>
        [SupplyParameterFromForm]
        private LoginModel Input { get; set; } = new()
        {
            Email = string.Empty,
            Password = string.Empty,
            RememberMe = false
        };

        protected override async Task OnInitializedAsync()
        {
            authViewModel = new AuthViewModel(AuthenticationService);
            // Get the authentication state
            var authState = await ((MameyAuthStateProvider)AuthStateProvider).GetAuthenticationStateAsync();
            var user = authState.User;

            // Redirect if the user is authenticated
            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                Navigation.NavigateTo("/");
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    // Reference22 the function as part of window.App
                    await JsRuntime.InvokeVoidAsync("App.focusElement", hiddenInput);
                }
                catch (JSException ex)
                {
                    Console.WriteLine($"Error setting focus: {ex.Message}");
                }
            }
        }
        private async Task PerformLogin()
        {
            isSubmitting = true;
            message = string.Empty;

            try
            {
                bool result = await authViewModel.HandleLoginAsync();
                if(result)
                {
                    Console.WriteLine("Login successful.");
                    var returnUrl = await LocalStorage.GetItemAsync<string>("returnUrl");
                     if (!string.IsNullOrEmpty(returnUrl)) 
                     {
                         await LocalStorage.RemoveItemAsync("returnUrl");
                         Navigation.NavigateTo(returnUrl, true);
                     }
                     else
                     {
                         Navigation.NavigateTo("/");
                     }
                }
                else
                {
                    Console.WriteLine("Login failed.");
                }
                
                //
                // var result = await authViewModel.HandleLoginAsync();
                //
                // if(result)
                // {
                //     Console.WriteLine("Login successful.");
                //     var navigationUrl = GetNavigationUrl();
                //
                //     Navigation.NavigateTo(navigationUrl);
                //     // Redirect to the dashboard or desired page.
                // }
                // else
                // {
                //     Console.WriteLine("Login failed.");
                //     message = "Login failed. Please try again.";
                // }
                // // Use IHttpClientFactory to create an HttpClient
                // // var client = HttpClientFactory.CreateClient("BankApi");
                // // var response = await client.PostAsJsonAsync($"/identity-module/sign-in", loginModel);
                //
                // // if (response.IsSuccessStatusCode)
                // // {
                // //     var result = await response.Content.ReadFromJsonAsync<AuthDto>(Mamey.JsonExtensions.SerializerOptions);
                // //     if (result != null)
                // //     {
                // //         await ((MameyAuthenticationStateProvider) AuthStateProvider).SetTokenAsync(result.Jwt.AccessToken, result.Jwt.RefreshToken);
                // //         // if (_rememberMe)
                // //         // {
                // //         //     await LocalStorage.SetItemAsync("authToken", result.Jwt.AccessToken);
                // //         //     await LocalStorage.SetItemAsync("refreshToken", result.Jwt.RefreshToken);
                // //         // }
                // //         // else
                // //         // {
                // //         //     await JsRuntime.InvokeVoidAsync("sessionStorage.setItem", "authToken", result.Jwt.AccessToken);
                // //         //     await JsRuntime.InvokeVoidAsync("sessionStorage.setItem", "refreshToken", result.Jwt.RefreshToken);
                // //         // }
                // //
                // //         await ((MameyAuthenticationStateProvider)AuthStateProvider).SetTokenAsync(result.Jwt.AccessToken, result.Jwt.RefreshToken);
                // //         var navigationUrl = GetNavigationUrl();
                // //
                // //         Navigation.NavigateTo(navigationUrl);
                // //         // await SetTokenAsync(result.AccessToken, result.RefreshToken);
                // //         //
                // //         // var returnUrl = await LocalStorage.GetItemAsync<string>("returnUrl");
                // //         // if (!string.IsNullOrEmpty(returnUrl))
                // //         // {
                // //         //     await LocalStorage.RemoveItemAsync("returnUrl");
                // //         //     Navigation.NavigateTo(returnUrl, true);
                // //         // }
                // //         // else
                // //         // {
                // //         //     Navigation.NavigateTo("/");
                // //         // }
                // //     }
                // //     else
                // //     {
                // //         message = "Login failed. Please try again.";
                // //     }
                // // }
                // // else
                // // {
                // //     message = "Invalid username or password.";
                // // }
            }
            catch (Exception ex)
            {
                message = $"An error occurred: {ex.Message}";
            }
            finally
            {
                isSubmitting = false;
            }
        }
        private string GetNavigationUrl()
        {
            if(string.IsNullOrWhiteSpace(ReturnUrl))
            {
                return "/";
            }

            return ReturnUrl;
        }


        public async Task SubmitForm()
        {
            Console.WriteLine("Submitting form...");
            hasTriedSubmitting = true; // Set flag to true when submitting
            await form.Validate();

            if (ValidateInput())
            {
                await PerformLogin();
            }
        }
        private bool ValidateInput()
        {
            emailError = string.IsNullOrWhiteSpace(authViewModel.LoginModel.Email) ? "Email is required" : string.Empty;
            passwordError = string.IsNullOrWhiteSpace(authViewModel.LoginModel.Password) ? "Password is required" : string.Empty;

            return string.IsNullOrEmpty(emailError) && string.IsNullOrEmpty(passwordError);
        }

        
    }
}
