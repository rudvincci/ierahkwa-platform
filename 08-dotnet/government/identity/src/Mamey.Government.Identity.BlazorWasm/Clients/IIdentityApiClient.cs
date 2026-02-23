using System.Net;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Http;
using Mamey.Types;

namespace Mamey.Government.Identity.BlazorWasm.Clients;

public interface IIdentityApiClient
{
    Task<ApiResponse<AuthDto>> PasswordSignInAsync(SignInRequest signInRequest);
    Task<(bool IsConfirmed, string Message)> ConfirmEmailAsync(UserId userId, string code);

    Task<(bool ok, CookieContainer jar, AuthDto? authDto)> LoginAndCaptureAsync(
        SignInRequest signInRequest);

    Task LogoutAsync(Guid userId);
}