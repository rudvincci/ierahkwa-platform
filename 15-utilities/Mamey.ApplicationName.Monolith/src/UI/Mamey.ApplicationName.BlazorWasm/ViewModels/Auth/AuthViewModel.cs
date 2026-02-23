using System.ComponentModel.DataAnnotations;
using Mamey.ApplicationName.BlazorWasm.Services;
using Mamey.ApplicationName.BlazorWasm.Services.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ReactiveUI;

namespace Mamey.ApplicationName.BlazorWasm.ViewModels.Auth;

// Aggregated ViewModel for managing authentication state
public class AuthViewModel : ReactiveObject
{
    private LoginModel loginModel = new LoginModel();
    private AuthenticationService _authenticationService;

    public AuthViewModel(AuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public LoginModel LoginModel
    {
        get => loginModel;
        set => this.RaiseAndSetIfChanged(ref loginModel, value);
    }

    private RegisterModel registerModel = new RegisterModel();

    public RegisterModel RegisterModel
    {
        get => registerModel;
        set => this.RaiseAndSetIfChanged(ref registerModel, value);
    }

    private SmsValidationModel smsModel = new SmsValidationModel();

    public SmsValidationModel SmsModel
    {
        get => smsModel;
        set => this.RaiseAndSetIfChanged(ref smsModel, value);
    }

    // MFA properties for displaying the QR code and setup key
    private string mfaQrCodeUrl;

    public string MfaQrCodeUrl
    {
        get => mfaQrCodeUrl;
        set => this.RaiseAndSetIfChanged(ref mfaQrCodeUrl, value);
    }

    private string mfaSetupKey;

    public string MfaSetupKey
    {
        get => mfaSetupKey;
        set => this.RaiseAndSetIfChanged(ref mfaSetupKey, value);
    }

    public AuthViewModel()
    {
        // Placeholder default MFA values; replace with API-generated values
        MfaQrCodeUrl = "https://via.placeholder.com/300x300?text=MFA+QR+Code";
        MfaSetupKey = "ABC123XYZ";
    }

    // Simulated asynchronous methods, to be replaced with actual API calls
    public async Task<bool> HandleLoginAsync()
    {
        try
        {
            // Call your Microsoft Identity Core API for login here.
            await _authenticationService.LoginAsync(LoginModel);
            
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }

    public async Task<bool> HandleRegisterAsync()
    {
        await Task.Delay(500);
    
        // Call the registration API endpoint.
        return true;
    }

    public async Task<bool> HandleSmsValidationAsync()
    {
        await Task.Delay(500);
        // Call API to validate SMS code.
        return true;
    }

    public async Task<bool> HandleMfaSetupCompleteAsync()
    {
        await Task.Delay(500);
        // Call API to confirm that MFA setup has been completed.
        return true;
    }

    public async Task ResendEmailConfirmationAsync()
    {
        await Task.Delay(500);
        // Simulate resending a confirmation email.
    }
}

// Model for login
public class LoginModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}

// Model for registration
public class RegisterModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Confirm Password is required.")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }
}

// Model for SMS validation
public class SmsValidationModel
{
    [Required(ErrorMessage = "Phone number is required.")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "Validation code is required.")]
    public string Code { get; set; }
}