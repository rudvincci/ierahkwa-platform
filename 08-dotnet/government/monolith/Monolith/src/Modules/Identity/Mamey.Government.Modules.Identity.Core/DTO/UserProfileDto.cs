using System;

namespace Mamey.Government.Modules.Identity.Core.DTO;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string AuthenticatorIssuer { get; set; } = string.Empty;
    public string AuthenticatorSubject { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public Guid? TenantId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
