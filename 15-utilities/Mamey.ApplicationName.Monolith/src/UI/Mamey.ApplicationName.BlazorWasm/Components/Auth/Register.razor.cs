// using System.Net.Http.Json;
// using System.Text.RegularExpressions;
// using Blazored.LocalStorage;
// using Mamey.ApplicationName.BlazorWasm.Services;
// using Microsoft.AspNetCore.Components;
// using Microsoft.AspNetCore.Components.Authorization;
// using Microsoft.JSInterop;
// using MudBlazor;
//
// namespace Mamey.ApplicationName.BlazorWasm.Components.Auth;
//
// public partial class Register : ComponentBase
// {
//     [Inject] private NavigationManager Navigation { get; set; } = default!;
//         [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
//         [Inject] private HttpClient HttpClient { get; set; } = default!;
//         [Inject] private ILocalStorageService LocalStorage { get; set; } = default!;
//         [Inject] private IJSRuntime JsRuntime { get; set; } = default!;
//
//         private RegisterModel _registerModel = new RegisterModel();
//         private string message = string.Empty;
//         private bool rememberMe = false;
//         bool success;
//         string[] errors = { };
//         MudTextField<string> pwField1;
//         MudForm form;
//         private bool isSubmitting = false;
//
//         private string usernameError = string.Empty;
//         private string passwordError = string.Empty;
//
//         public async Task PerformLogin()
//         {
//             if (!ValidateInput())
//             {
//                 message = "Please fill in all required fields.";
//                 return;
//             }
//
//             isSubmitting = true;
//             try
//             {
//                 var response = await HttpClient.PostAsJsonAsync("api/auth/login", _registerModel);
//                 if (response.IsSuccessStatusCode)
//                 {
//                     var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
//                     if (result != null)
//                     {
//                         await SetTokenAsync(result.AccessToken, result.RefreshToken);
//
//                         var returnUrl = await LocalStorage.GetItemAsync<string>("returnUrl");
//                         if (!string.IsNullOrEmpty(returnUrl))
//                         {
//                             await LocalStorage.RemoveItemAsync("returnUrl");
//                             Navigation.NavigateTo(returnUrl, true);
//                         }
//                         else
//                         {
//                             Navigation.NavigateTo("/");
//                         }
//                     }
//                     else
//                     {
//                         message = "Login failed. Please try again.";
//                     }
//                 }
//                 else
//                 {
//                     message = "Invalid username or password.";
//                 }
//             }
//             catch (Exception ex)
//             {
//                 message = $"An error occurred: {ex.Message}";
//             }
//             finally
//             {
//                 isSubmitting = false;
//             }
//         }
//
//         public async Task SetTokenAsync(string accessToken, string refreshToken)
//         {
//             if (rememberMe)
//             {
//                 await LocalStorage.SetItemAsync("authToken", accessToken);
//                 await LocalStorage.SetItemAsync("refreshToken", refreshToken);
//             }
//             else
//             {
//                 await JsRuntime.InvokeVoidAsync("sessionStorage.setItem", "authToken", accessToken);
//                 await JsRuntime.InvokeVoidAsync("sessionStorage.setItem", "refreshToken", refreshToken);
//             }
//
//             await ((MameyAuthStateProvider)AuthStateProvider).SetTokenAsync(accessToken, refreshToken);
//         }
//
//         public async Task SubmitForm()
//         {
//             await form.Validate();
//             if (form.IsValid)
//             {
//                 await PerformLogin();
//             }
//         }
//
//         private void OnInvalidSubmit()
//         {
//             message = "Please correct the errors and try again.";
//         }
//         
//         private bool ValidateInput()
//         {
//             usernameError = string.IsNullOrWhiteSpace(_registerModel.Username) ? "Email is required" : string.Empty;
//             passwordError = string.IsNullOrWhiteSpace(_registerModel.Password) ? "Password is required" : string.Empty;
//
//             return string.IsNullOrEmpty(usernameError) && string.IsNullOrEmpty(passwordError);
//         }
//         
//         private IEnumerable<string> PasswordStrength(string pw)
//         {
//             if (string.IsNullOrWhiteSpace(pw))
//             {
//                 yield return "Password is required!";
//                 yield break;
//             }
//             if (pw.Length < 8)
//                 yield return "Password must be at least of length 8";
//             if (!Regex.IsMatch(pw, @"[A-Z]"))
//                 yield return "Password must contain at least one capital letter";
//             if (!Regex.IsMatch(pw, @"[a-z]"))
//                 yield return "Password must contain at least one lowercase letter";
//             if (!Regex.IsMatch(pw, @"[0-9]"))
//                 yield return "Password must contain at least one digit";
//         }
//         private string PasswordMatch(string arg)
//         {
//             if (pwField1.Value != arg)
//                 return "Passwords don't match";
//             return null;
//         }
//         
//         protected class RegisterModel
//         {
//             public string Username { get; set; } = string.Empty;
//             public string Password { get; set; } = string.Empty;
//         }
// }