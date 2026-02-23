using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Types;

namespace Mamey.Government.Identity.BlazorWasm.Services;

public interface IAuthService
{
    // Task RegisterAsync(Register command);
    Task<AuthDto?> PasswordSignInAsync(SignInRequest signInRequest, string returnUrl = "/");
    Task<bool> ConfirmEmailAsync(UserId userId, string code);
    Task RefreshSignInAsync(UserId userId);
    Task<AuthDto?> TwoFactorRecoveryCodeSignInAsync(UserId userId, string code);
    Task LogoutAsync();
}