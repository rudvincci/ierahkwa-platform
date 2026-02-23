using System.ComponentModel;
using Mamey.Government.Identity.Domain.Events;
using Mamey.Government.Identity.Domain.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Entities;

internal class MultiFactorAuth : AggregateRoot<MultiFactorAuthId>
{
    #region Fields
    private ISet<MfaMethod> _enabledMethods = new HashSet<MfaMethod>();
    private ISet<MfaChallenge> _activeChallenges = new HashSet<MfaChallenge>();
    #endregion

    public MultiFactorAuth(MultiFactorAuthId id, UserId userId, DateTime createdAt,
        IEnumerable<MfaMethod>? enabledMethods = null, MultiFactorAuthStatus status = MultiFactorAuthStatus.Inactive,
        int version = 0)
        : base(id, version)
    {
        UserId = userId;
        CreatedAt = createdAt;
        Status = status;
        EnabledMethods = enabledMethods ?? Enumerable.Empty<MfaMethod>();
    }

    #region Properties

    /// <summary>
    /// Reference to the user this MFA belongs to.
    /// </summary>
    [Description("Reference to the user this MFA belongs to")]
    public UserId UserId { get; private set; }

    /// <summary>
    /// Date and time the MFA was created.
    /// </summary>
    [Description("Date and time the MFA was created")]
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Date and time the MFA was last modified.
    /// </summary>
    [Description("Date and time the MFA was last modified")]
    public DateTime? ModifiedAt { get; private set; }

    /// <summary>
    /// Date and time the MFA was activated.
    /// </summary>
    [Description("Date and time the MFA was activated")]
    public DateTime? ActivatedAt { get; private set; }

    /// <summary>
    /// Current status of the MFA.
    /// </summary>
    [Description("Current status of the MFA")]
    public MultiFactorAuthStatus Status { get; private set; }

    /// <summary>
    /// Collection of enabled MFA methods.
    /// </summary>
    [Description("Collection of enabled MFA methods")]
    public IEnumerable<MfaMethod> EnabledMethods
    {
        get => _enabledMethods;
        private set => _enabledMethods = new HashSet<MfaMethod>(value);
    }

    /// <summary>
    /// Collection of active MFA challenges.
    /// </summary>
    [Description("Collection of active MFA challenges")]
    public IEnumerable<MfaChallenge> ActiveChallenges
    {
        get => _activeChallenges;
        private set => _activeChallenges = new HashSet<MfaChallenge>(value);
    }

    /// <summary>
    /// Date and time the MFA was last used.
    /// </summary>
    [Description("Date and time the MFA was last used")]
    public DateTime? LastUsedAt { get; private set; }

    /// <summary>
    /// Number of times the MFA has been used.
    /// </summary>
    [Description("Number of times the MFA has been used")]
    public int UsageCount { get; private set; }

    /// <summary>
    /// Number of failed verification attempts.
    /// </summary>
    [Description("Number of failed verification attempts")]
    public int FailedAttempts { get; private set; }

    /// <summary>
    /// Date and time the MFA was last failed.
    /// </summary>
    [Description("Date and time the MFA was last failed")]
    public DateTime? LastFailedAt { get; private set; }

    /// <summary>
    /// Required number of methods for successful authentication.
    /// </summary>
    [Description("Required number of methods for successful authentication")]
    public int RequiredMethods { get; private set; } = 2;
    #endregion

    public static MultiFactorAuth Create(Guid id, Guid userId, IEnumerable<MfaMethod>? enabledMethods = null)
    {
        var mfa = new MultiFactorAuth(id, userId, DateTime.UtcNow, enabledMethods);
        mfa.AddEvent(new MultiFactorAuthCreated(mfa));
        return mfa;
    }

    public void EnableMethod(MfaMethod method)
    {
        if (Status == MultiFactorAuthStatus.Disabled)
        {
            throw new MultiFactorAuthDisabledException();
        }

        if (_enabledMethods.Contains(method))
        {
            throw new MfaMethodAlreadyEnabledException();
        }

        _enabledMethods.Add(method);
        
        if (Status == MultiFactorAuthStatus.Inactive && _enabledMethods.Count >= RequiredMethods)
        {
            Status = MultiFactorAuthStatus.Active;
            AddEvent(new MultiFactorAuthActivated(this));
        }

        AddEvent(new MfaMethodEnabled(this, method));
    }

    public void DisableMethod(MfaMethod method)
    {
        if (!_enabledMethods.Contains(method))
        {
            throw new MfaMethodNotEnabledException();
        }

        _enabledMethods.Remove(method);
        
        if (_enabledMethods.Count < RequiredMethods && Status == MultiFactorAuthStatus.Active)
        {
            Status = MultiFactorAuthStatus.Inactive;
            AddEvent(new MultiFactorAuthDeactivated(this));
        }

        AddEvent(new MfaMethodDisabled(this, method));
    }

    public void SetRequiredMethods(int count)
    {
        if (count < 1 || count > 5)
        {
            throw new InvalidRequiredMethodsCountException();
        }

        RequiredMethods = count;
        
        if (_enabledMethods.Count >= RequiredMethods && Status == MultiFactorAuthStatus.Inactive)
        {
            Status = MultiFactorAuthStatus.Active;
            AddEvent(new MultiFactorAuthActivated(this));
        }
        else if (_enabledMethods.Count < RequiredMethods && Status == MultiFactorAuthStatus.Active)
        {
            Status = MultiFactorAuthStatus.Inactive;
            AddEvent(new MultiFactorAuthDeactivated(this));
        }

        AddEvent(new MfaRequiredMethodsChanged(this, count));
    }

    public MfaChallenge CreateChallenge(MfaMethod method, string challengeData)
    {
        if (!_enabledMethods.Contains(method))
        {
            throw new MfaMethodNotEnabledException();
        }

        if (Status != MultiFactorAuthStatus.Active)
        {
            throw new MultiFactorAuthNotActiveException();
        }

        var challenge = new MfaChallenge(
            Guid.NewGuid(),
            Id,
            method,
            challengeData,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(5) // 5-minute expiry
        );

        _activeChallenges.Add(challenge);
        AddEvent(new MfaChallengeCreated(this, challenge));
        
        return challenge;
    }

    public void VerifyChallenge(MfaChallengeId challengeId, string response)
    {
        var challenge = _activeChallenges.FirstOrDefault(c => c.Id == challengeId);
        if (challenge == null)
        {
            throw new MfaChallengeNotFoundException();
        }

        if (challenge.IsExpired())
        {
            _activeChallenges.Remove(challenge);
            AddEvent(new MfaChallengeExpired(this, challenge));
            throw new MfaChallengeExpiredException();
        }

        if (!challenge.Verify(response))
        {
            FailedAttempts++;
            LastFailedAt = DateTime.UtcNow;
            AddEvent(new MfaChallengeVerificationFailed(this, challenge, FailedAttempts));
            throw new MfaChallengeVerificationFailedException();
        }

        _activeChallenges.Remove(challenge);
        LastUsedAt = DateTime.UtcNow;
        UsageCount++;
        FailedAttempts = 0;
        AddEvent(new MfaChallengeVerified(this, challenge));
    }

    public void Activate()
    {
        if (Status == MultiFactorAuthStatus.Active)
        {
            throw new MultiFactorAuthAlreadyActiveException();
        }

        if (_enabledMethods.Count < RequiredMethods)
        {
            throw new InsufficientMfaMethodsException();
        }

        Status = MultiFactorAuthStatus.Active;
        AddEvent(new MultiFactorAuthActivated(this));
    }

    public void Deactivate()
    {
        if (Status == MultiFactorAuthStatus.Inactive)
        {
            throw new MultiFactorAuthAlreadyInactiveException();
        }

        Status = MultiFactorAuthStatus.Inactive;
        _activeChallenges.Clear();
        AddEvent(new MultiFactorAuthDeactivated(this));
    }

    public void Disable()
    {
        if (Status == MultiFactorAuthStatus.Disabled)
        {
            throw new MultiFactorAuthAlreadyDisabledException();
        }

        Status = MultiFactorAuthStatus.Disabled;
        _activeChallenges.Clear();
        AddEvent(new MultiFactorAuthDisabled(this));
    }

    public bool IsLocked()
    {
        return FailedAttempts >= 5 && LastFailedAt.HasValue && 
               DateTime.UtcNow < LastFailedAt.Value.AddMinutes(15);
    }

    public bool CanBeUsed()
    {
        return Status == MultiFactorAuthStatus.Active && !IsLocked();
    }

    public bool HasRequiredMethods()
    {
        return _enabledMethods.Count >= RequiredMethods;
    }

    public void CleanupExpiredChallenges()
    {
        var expiredChallenges = _activeChallenges.Where(c => c.IsExpired()).ToList();
        foreach (var challenge in expiredChallenges)
        {
            _activeChallenges.Remove(challenge);
            AddEvent(new MfaChallengeExpired(this, challenge));
        }
    }

    public void UpdateRequiredMethods(int requiredMethods)
    {
        if (requiredMethods < 1)
        {
            throw new ArgumentException("Required methods must be at least 1", nameof(requiredMethods));
        }

        RequiredMethods = requiredMethods;
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new MultiFactorAuthRequiredMethodsUpdated(this, requiredMethods));
    }
}