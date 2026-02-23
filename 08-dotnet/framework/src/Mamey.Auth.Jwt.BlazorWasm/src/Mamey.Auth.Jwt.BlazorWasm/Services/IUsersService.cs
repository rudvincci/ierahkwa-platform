// MyApp.Client/Services/ReactiveAuthenticationService.cs
using Mamey.Http;
// using Mamey.Pagination;
using Mamey.Auth.Jwt.BlazorWasm.Models;
using Mamey.Auth.Jwt.BlazorWasm.Requests;

namespace Mamey.Auth.Jwt.BlazorWasm.Services;

public interface IUsersService
{
    Task<ApiResponse<UserDetailsDto>> GetAsync(Guid userId);
    Task<ApiResponse> RegisterAsync(RegisterRequest request);
    Task<ApiResponse<AuthDto>> LoginAsync(SignIn request);
    // Task<ApiResponse<PagedDto<UserDto>>> BrowseUsersAsync(Guid? userId = null);
    Task<ApiResponse> RecoverPasswordAsync(RecoverPasswordRequest request);
    Task<ApiResponse> ResetPasswordAsync(ResetPasswordRequest request, string token);
    Task<ApiResponse<AuthDto>> UseRefreshTokenAsync(UseRefreshTokenRequest request);

}
