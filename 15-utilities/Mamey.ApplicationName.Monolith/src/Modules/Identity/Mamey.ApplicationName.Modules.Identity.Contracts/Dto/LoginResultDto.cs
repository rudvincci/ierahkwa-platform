namespace Mamey.ApplicationName.Modules.Identity.Contracts.Dto;

public class LoginResultDto
{
    public bool Succeeded { get; set; }
    public bool RequiresTwoFactor { get; set; }
    public bool IsLockedOut { get; set; }
    public string RedirectUrl { get; set; } = string.Empty;
    public string TwoFactorUrl { get; set; } = string.Empty;
}