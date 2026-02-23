using UserManagement.Application.DTOs;

namespace UserManagement.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
    Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
    Task<bool> LogoutAsync(int userId);
    Task<bool> ResetPasswordRequestAsync(string email);
    Task<bool> ResetPasswordConfirmAsync(ResetPasswordConfirmDto resetPasswordDto);
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
    Task<bool> ValidateTokenAsync(string token);
}

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int TokenExpiryMinutes { get; set; } = 60;
    public int RefreshTokenExpiryDays { get; set; } = 7;
}
