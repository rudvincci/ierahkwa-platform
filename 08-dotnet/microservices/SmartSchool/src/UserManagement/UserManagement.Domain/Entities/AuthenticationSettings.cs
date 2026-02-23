using Common.Domain.Entities;

namespace UserManagement.Domain.Entities;

public class AuthenticationSettings : BaseEntity
{
    public bool EnableTwoFactor { get; set; } = false;
    public bool EnableEmailConfirmation { get; set; } = true;
    public int PasswordMinLength { get; set; } = 8;
    public bool RequireUppercase { get; set; } = true;
    public bool RequireLowercase { get; set; } = true;
    public bool RequireDigit { get; set; } = true;
    public bool RequireSpecialCharacter { get; set; } = true;
    public int LockoutDurationMinutes { get; set; } = 15;
    public int MaxFailedAttempts { get; set; } = 5;
    public int SessionTimeoutMinutes { get; set; } = 60;
    public int RefreshTokenExpiryDays { get; set; } = 7;
    public int? TenantId { get; set; }
}
