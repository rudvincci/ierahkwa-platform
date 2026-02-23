using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.DTO;

public class UserDto
{
    public UserDto(Guid id, Guid subjectId, string username, string email, bool isActive, bool isLocked, DateTime? lockedUntil, DateTime? lastLoginAt, bool emailConfirmationRequired, bool twoFactorEnabled, bool multiFactorEnabled, DateTime? emailConfirmedAt, DateTime? twoFactorEnabledAt, DateTime? multiFactorEnabledAt, DateTime createdAt, DateTime? modifiedAt)
    {
        Id = id;
        SubjectId = subjectId;
        Username = username;
        Email = email;
        IsActive = isActive;
        IsLocked = isLocked;
        LockedUntil = lockedUntil;
        LastLoginAt = lastLoginAt;
        EmailConfirmationRequired = emailConfirmationRequired;
        TwoFactorEnabled = twoFactorEnabled;
        MultiFactorEnabled = multiFactorEnabled;
        EmailConfirmedAt = emailConfirmedAt;
        TwoFactorEnabledAt = twoFactorEnabledAt;
        MultiFactorEnabledAt = multiFactorEnabledAt;
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }

    public Guid Id { get; set; }
    public Guid SubjectId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? LockedUntil { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool EmailConfirmationRequired { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public bool MultiFactorEnabled { get; set; }
    public DateTime? EmailConfirmedAt { get; set; }
    public DateTime? TwoFactorEnabledAt { get; set; }
    public DateTime? MultiFactorEnabledAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}










