using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mamey.Government.Identity.Domain.Events;
using Mamey.Government.Identity.Domain.Exceptions;
using Mamey.Types;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Unit.Core.Entities")]
namespace Mamey.Government.Identity.Domain.Entities;


internal class User : AggregateRoot<UserId>
{
    private readonly DateTime? _multiFactorEnabledAt;

    public User(UserId id, SubjectId subjectId, string username, Email email, 
        string passwordHash, DateTime createdAt, DateTime? modifiedAt = null, 
        UserStatus status = UserStatus.Active, bool multiFactorEnabled = false, DateTime? multiFactorEnabledAt = null, 
        int version = 0)
        : base(id, version)
    {
        _multiFactorEnabledAt = multiFactorEnabledAt;
        SubjectId = subjectId;
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
        Status = status;
        MultiFactorEnabled = multiFactorEnabled;
    }

    #region Properties

    /// <summary>
    /// Reference to the subject this user represents.
    /// </summary>
    [Description("Reference to the subject this user represents")]
    public SubjectId SubjectId { get; private set; }

    /// <summary>
    /// Username for authentication.
    /// </summary>
    [Description("Username for authentication")]
    public string Username { get; private set; }

    /// <summary>
    /// Email address for the user.
    /// </summary>
    [Description("Email address for the user")]
    public Email Email { get; private set; }

    /// <summary>
    /// Hashed password for authentication.
    /// </summary>
    [Description("Hashed password for authentication")]
    public string PasswordHash { get; private set; }

    /// <summary>
    /// Current status of the user.
    /// </summary>
    [Description("Current status of the user")]
    public UserStatus Status { get; private set; }

    /// <summary>
    /// Date and time the user was created.
    /// </summary>
    [Description("Date and time the user was created.")]
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Date and time the user was modified.
    /// </summary>
    [Description("Date and time the user was modified.")]
    public DateTime? ModifiedAt { get; private set; }

    /// <summary>
    /// Date and time the user last logged in.
    /// </summary>
    [Description("Date and time the user last logged in.")]
    public DateTime? LastLoginAt { get; private set; }

    /// <summary>
    /// Number of failed login attempts.
    /// </summary>
    [Description("Number of failed login attempts")]
    public int FailedLoginAttempts { get; private set; }

    /// <summary>
    /// Date and time the user was locked due to failed attempts.
    /// </summary>
    [Description("Date and time the user was locked due to failed attempts.")]
    public DateTime? LockedUntil { get; private set; }

    /// <summary>
    /// Date and time the password was last changed.
    /// </summary>
    [Description("Date and time the password was last changed.")]
    public DateTime? PasswordChangedAt { get; private set; }

    /// <summary>
    /// Whether email confirmation is required.
    /// </summary>
    [Description("Whether email confirmation is required")]
    public bool EmailConfirmationRequired { get; private set; } = true;

    /// <summary>
    /// Whether two-factor authentication is enabled.
    /// </summary>
    [Description("Whether two-factor authentication is enabled")]
    public bool TwoFactorEnabled { get; private set; } = false;

    /// <summary>
    /// Whether multi-factor authentication is enabled.
    /// </summary>
    [Description("Whether multi-factor authentication is enabled")]
    public bool MultiFactorEnabled { get; private set; } = false;

    /// <summary>
    /// Date and time the email was confirmed.
    /// </summary>
    [Description("Date and time the email was confirmed")]
    public DateTime? EmailConfirmedAt { get; private set; }

    /// <summary>
    /// Date and time 2FA was enabled.
    /// </summary>
    [Description("Date and time 2FA was enabled")]
    public DateTime? TwoFactorEnabledAt { get; private set; }

    /// <summary>
    /// Date and time MFA was enabled.
    /// </summary>
    [Description("Date and time MFA was enabled")]
    public DateTime? MultiFactorEnabledAt { get; private set; }

    public bool IsActive => Status == UserStatus.Active;
    public bool IsLocked => Status == UserStatus.Locked && LockedUntil.HasValue && LockedUntil.Value > DateTime.UtcNow;
    #endregion

    public static User Create(Guid id, Guid subjectId, string username, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new MissingUsernameException();
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new MissingUserEmailException();
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new MissingPasswordHashException();
        }

        var emailObj = new Email(email);
        var user = new User(id, subjectId, username, emailObj, passwordHash, DateTime.UtcNow);
        user.AddEvent(new UserCreated(user));
        return user;
    }

    public void Update(string username, string email)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new MissingUsernameException();
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new MissingUserEmailException();
        }

        Username = username;
        Email = new Email(email);
        ModifiedAt = DateTime.UtcNow;
        
        AddEvent(new UserModified(this));
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
        {
            throw new MissingPasswordHashException();
        }

        PasswordHash = newPasswordHash;
        PasswordChangedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        
        AddEvent(new UserPasswordChanged(this));
    }

    public void Activate()
    {
        if (Status == UserStatus.Active)
        {
            throw new UserAlreadyActiveException();
        }

        Status = UserStatus.Active;
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new UserActivated(this));
    }

    public void Deactivate()
    {
        if (Status == UserStatus.Inactive)
        {
            throw new UserAlreadyInactiveException();
        }

        Status = UserStatus.Inactive;
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new UserDeactivated(this));
    }

    public void Lock(DateTime lockedUntil)
    {
        if (Status == UserStatus.Locked)
        {
            throw new UserAlreadyLockedException();
        }

        Status = UserStatus.Locked;
        LockedUntil = lockedUntil;
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new UserLocked(this, lockedUntil));
    }

    public void Unlock()
    {
        if (Status != UserStatus.Locked)
        {
            throw new UserNotLockedException();
        }

        Status = UserStatus.Active;
        LockedUntil = null;
        FailedLoginAttempts = 0;
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new UserUnlocked(this));
    }

    public void RecordSuccessfulLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        FailedLoginAttempts = 0;
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new UserLoggedIn(this));
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        FailedLoginAttempts = 0;
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new UserLoggedIn(this));
    }

    public void RecordFailedLogin()
    {
        FailedLoginAttempts++;
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new UserLoginFailed(this, FailedLoginAttempts));

        // Lock user after 5 failed attempts for 30 minutes
        if (FailedLoginAttempts >= 5)
        {
            Lock(DateTime.UtcNow.AddMinutes(30));
        }
    }

   

    public bool CanAuthenticate()
    {
        return Status == UserStatus.Active && !IsLocked;
    }

    public void ConfirmEmail()
    {
        if (EmailConfirmedAt.HasValue)
        {
            throw new EmailAlreadyConfirmedException();
        }

        EmailConfirmedAt = DateTime.UtcNow;
        EmailConfirmationRequired = false;
        AddEvent(new UserEmailConfirmed(this));
    }

    public void EnableTwoFactor()
    {
        if (TwoFactorEnabled)
        {
            throw new TwoFactorAlreadyEnabledException();
        }

        TwoFactorEnabled = true;
        TwoFactorEnabledAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new UserTwoFactorEnabled(this));
    }

    public void DisableTwoFactor()
    {
        if (!TwoFactorEnabled)
        {
            throw new TwoFactorNotEnabledException();
        }

        TwoFactorEnabled = false;
        TwoFactorEnabledAt = null;
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new UserTwoFactorDisabled(this));
    }

    public void EnableMultiFactor()
    {
        if (MultiFactorEnabled)
        {
            throw new MultiFactorAlreadyEnabledException();
        }

        MultiFactorEnabled = true;
        MultiFactorEnabledAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new UserMultiFactorEnabled(this));
    }

    public void DisableMultiFactor()
    {
        if (!MultiFactorEnabled)
        {
            throw new MultiFactorNotEnabledException();
        }

        MultiFactorEnabled = false;
        MultiFactorEnabledAt = null;
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new UserMultiFactorDisabled(this));
    }

    public bool RequiresEmailConfirmation()
    {
        return EmailConfirmationRequired && !EmailConfirmedAt.HasValue;
    }

    public bool RequiresTwoFactor()
    {
        return TwoFactorEnabled;
    }

    public bool RequiresMultiFactor()
    {
        return MultiFactorEnabled;
    }

    public bool IsEmailConfirmed()
    {
        return EmailConfirmedAt.HasValue;
    }

}