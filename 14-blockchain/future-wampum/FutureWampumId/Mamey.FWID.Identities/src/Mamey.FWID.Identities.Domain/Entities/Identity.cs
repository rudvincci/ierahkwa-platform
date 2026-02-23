using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.FWID.Identities.Domain.Exceptions;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;

[assembly: InternalsVisibleTo("Mamey.FWID.Identities.Tests.Unit.Core.Entities")]
namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Represents an identity aggregate root.
/// </summary>
public class Identity : AggregateRoot<IdentityId>
{
    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private Identity()
    {
        Metadata = new Dictionary<string, object>();
    }

    /// <summary>
    /// Initializes a new instance of the Identity aggregate root.
    /// </summary>
    /// <param name="id">The identity identifier.</param>
    /// <param name="name">The name of the identity.</param>
    /// <param name="personalDetails">The personal details.</param>
    /// <param name="contactInformation">The contact information.</param>
    /// <param name="biometricData">The biometric data.</param>
    /// <param name="zone">The zone.</param>
    /// <param name="clanRegistrarId">The clan registrar identifier.</param>
    [JsonConstructor]
    public Identity(
        IdentityId id,
        Name name,
        PersonalDetails personalDetails,
        ContactInformation contactInformation,
        BiometricData biometricData,
        string? zone = null,
        Guid? clanRegistrarId = null)
        : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        PersonalDetails = personalDetails ?? throw new ArgumentNullException(nameof(personalDetails));
        ContactInformation = contactInformation ?? throw new ArgumentNullException(nameof(contactInformation));
        BiometricData = biometricData ?? throw new ArgumentNullException(nameof(biometricData));
        Zone = zone;
        ClanRegistrarId = clanRegistrarId;
        Status = IdentityStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        Metadata = new Dictionary<string, object>();
        
        // Raise domain event (blockchain account will be added to event after creation)
        // Note: Blockchain account is added to metadata and event is updated in IdentityService
        AddEvent(new IdentityCreated(Id, Name, CreatedAt, Zone, null));
    }

    #region Properties

    /// <summary>
    /// The name of the identity.
    /// </summary>
    public Name Name { get; private set; } = null!;

    /// <summary>
    /// The personal details of the identity.
    /// </summary>
    public PersonalDetails PersonalDetails { get; private set; } = null!;

    /// <summary>
    /// The contact information of the identity.
    /// </summary>
    public ContactInformation ContactInformation { get; private set; } = null!;

    /// <summary>
    /// The encrypted biometric data.
    /// </summary>
    public BiometricData BiometricData { get; private set; } = null!;

    /// <summary>
    /// The status of the identity.
    /// </summary>
    public IdentityStatus Status { get; private set; }

    /// <summary>
    /// The zone of the identity.
    /// </summary>
    public string? Zone { get; private set; }

    /// <summary>
    /// The clan registrar identifier.
    /// </summary>
    public Guid? ClanRegistrarId { get; private set; }

    /// <summary>
    /// Date and time the record was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Date and time the identity was verified.
    /// </summary>
    public DateTime? VerifiedAt { get; private set; }

    /// <summary>
    /// Date and time the identity was revoked.
    /// </summary>
    public DateTime? RevokedAt { get; private set; }

    /// <summary>
    /// Date and time the identity was last verified.
    /// </summary>
    public DateTime? LastVerifiedAt { get; private set; }

    /// <summary>
    /// Additional metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; private set; }

    #region Authentication Properties

    /// <summary>
    /// The username for authentication.
    /// </summary>
    public string? Username { get; private set; }

    /// <summary>
    /// The hashed password.
    /// </summary>
    public string? PasswordHash { get; private set; }

    /// <summary>
    /// Indicates whether the email has been confirmed.
    /// </summary>
    public bool EmailConfirmed { get; private set; }

    /// <summary>
    /// Indicates whether the phone number has been confirmed.
    /// </summary>
    public bool PhoneConfirmed { get; private set; }

    /// <summary>
    /// Date and time the email was confirmed.
    /// </summary>
    public DateTime? EmailConfirmedAt { get; private set; }

    /// <summary>
    /// Date and time the phone number was confirmed.
    /// </summary>
    public DateTime? PhoneConfirmedAt { get; private set; }

    /// <summary>
    /// Date and time of the last login.
    /// </summary>
    public DateTime? LastLoginAt { get; private set; }

    /// <summary>
    /// Number of consecutive failed login attempts.
    /// </summary>
    public int FailedLoginAttempts { get; private set; }

    /// <summary>
    /// Date and time the account is locked until (null if not locked).
    /// </summary>
    public DateTime? LockedUntil { get; private set; }

    /// <summary>
    /// Date and time the password was last changed.
    /// </summary>
    public DateTime? PasswordChangedAt { get; private set; }

    /// <summary>
    /// Indicates whether multi-factor authentication is enabled.
    /// </summary>
    public bool MultiFactorEnabled { get; private set; }

    /// <summary>
    /// Date and time multi-factor authentication was enabled.
    /// </summary>
    public DateTime? MultiFactorEnabledAt { get; private set; }

    /// <summary>
    /// The preferred MFA method.
    /// </summary>
    public MfaMethod? PreferredMfaMethod { get; private set; }

    /// <summary>
    /// The Azure user identifier for Azure AD authentication.
    /// </summary>
    public string? AzureUserId { get; private set; }

    /// <summary>
    /// The service identifier for service-to-service authentication.
    /// </summary>
    public string? ServiceId { get; private set; }

    #endregion

    #endregion

    #region Domain Methods

    /// <summary>
    /// Verifies the biometric data against the provided biometric.
    /// Compliant with Biometric Verification Microservice spec (§5.2).
    /// </summary>
    /// <param name="providedBiometric">The biometric data to verify against.</param>
    /// <param name="threshold">The match threshold (default: 0.85 per spec §4).</param>
    /// <param name="did">The DID associated with this identity (optional).</param>
    /// <exception cref="InvalidOperationException">Thrown when the identity is revoked.</exception>
    /// <exception cref="BiometricVerificationFailedException">Thrown when verification fails.</exception>
    public void VerifyBiometric(BiometricData providedBiometric, double threshold = 0.85, string? did = null)
    {
        if (Status == IdentityStatus.Revoked)
            throw new InvalidOperationException("Cannot verify revoked identity");
        
        var matchScore = BiometricData.Match(providedBiometric);
        
        if (matchScore < threshold)
            throw new BiometricVerificationFailedException(Id, matchScore, threshold);
        
        Status = IdentityStatus.Verified;
        VerifiedAt ??= DateTime.UtcNow;
        LastVerifiedAt = DateTime.UtcNow;
        
        // Emit both IdentityVerified (for internal use) and BiometricVerified (for external integration)
        AddEvent(new IdentityVerified(Id, LastVerifiedAt.Value, matchScore));
        
        // Emit BiometricVerified event per spec (§6) with evidence JWS
        var decision = matchScore >= threshold ? "PASS" : "FAIL";
        AddEvent(new BiometricVerified(
            Id,
            did,
            matchScore,
            providedBiometric.LivenessScore,
            decision,
            providedBiometric.EvidenceJws ?? string.Empty,
            LastVerifiedAt.Value));
    }

    /// <summary>
    /// Updates the biometric data (enrollment or re-enrollment).
    /// Compliant with Biometric Verification Microservice spec (§5.1).
    /// </summary>
    /// <param name="newBiometric">The new biometric data.</param>
    /// <param name="verificationBiometric">The biometric data to verify identity before update (optional for initial enrollment).</param>
    /// <param name="did">The DID associated with this identity (optional).</param>
    /// <param name="isEnrollment">Whether this is an initial enrollment (true) or re-enrollment (false).</param>
    /// <exception cref="InvalidOperationException">Thrown when the identity is revoked.</exception>
    public void UpdateBiometric(
        BiometricData newBiometric,
        BiometricData? verificationBiometric = null,
        string? did = null,
        bool isEnrollment = false)
    {
        if (Status == IdentityStatus.Revoked)
            throw new InvalidOperationException("Cannot update biometric for revoked identity");
        
        // Verify with old biometric first (if not initial enrollment)
        if (!isEnrollment && verificationBiometric != null)
        {
            VerifyBiometric(verificationBiometric, did: did);
        }
        
        var wasEnrolled = !string.IsNullOrEmpty(BiometricData?.TemplateId);
        BiometricData = newBiometric ?? throw new ArgumentNullException(nameof(newBiometric));
        
        // Emit BiometricUpdated for internal use
        AddEvent(new BiometricUpdated(Id, DateTime.UtcNow));
        
        // Emit BiometricEnrolled event per spec (§6) if this is enrollment or re-enrollment with template ID
        if (isEnrollment || (!wasEnrolled && !string.IsNullOrEmpty(newBiometric.TemplateId)))
        {
            AddEvent(new BiometricEnrolled(
                Id,
                did,
                newBiometric.TemplateId,
                newBiometric.EvidenceJws ?? string.Empty,
                newBiometric.AlgoVersion,
                newBiometric.Quality,
                newBiometric.LivenessScore,
                newBiometric.LivenessDecision,
                DateTime.UtcNow));
        }
    }

    /// <summary>
    /// Revokes the identity.
    /// </summary>
    /// <param name="reason">The reason for revocation.</param>
    /// <param name="revokedBy">The identifier of the user who revoked the identity.</param>
    public void Revoke(string reason, Guid? revokedBy = null)
    {
        if (Status == IdentityStatus.Revoked)
            return; // Already revoked
        
        Status = IdentityStatus.Revoked;
        RevokedAt = DateTime.UtcNow;
        Metadata["RevocationReason"] = reason ?? string.Empty;
        Metadata["RevokedBy"] = revokedBy?.ToString() ?? "System";
        
        AddEvent(new IdentityRevoked(Id, reason ?? string.Empty, RevokedAt.Value, revokedBy));
    }

    /// <summary>
    /// Updates the zone of the identity.
    /// </summary>
    /// <param name="newZone">The new zone.</param>
    /// <exception cref="InvalidOperationException">Thrown when the identity is revoked.</exception>
    public void UpdateZone(string? newZone)
    {
        if (Status == IdentityStatus.Revoked)
            throw new InvalidOperationException("Cannot update zone for revoked identity");
        
        var oldZone = Zone;
        Zone = newZone;
        
        AddEvent(new ZoneUpdated(Id, oldZone, newZone, DateTime.UtcNow));
    }

    /// <summary>
    /// Updates the contact information of the identity.
    /// </summary>
    /// <param name="newContactInfo">The new contact information.</param>
    /// <exception cref="InvalidOperationException">Thrown when the identity is revoked.</exception>
    public void UpdateContactInformation(ContactInformation newContactInfo)
    {
        if (Status == IdentityStatus.Revoked)
            throw new InvalidOperationException("Cannot update contact information for revoked identity");
        
        ContactInformation = newContactInfo ?? throw new ArgumentNullException(nameof(newContactInfo));
        
        AddEvent(new ContactInformationUpdated(Id, DateTime.UtcNow));
    }

    #endregion

    #region Authentication Methods

    /// <summary>
    /// Records a successful sign-in.
    /// </summary>
    public void SignIn()
    {
        if (Status == IdentityStatus.Revoked)
            throw new InvalidOperationException("Cannot sign in to revoked identity");
        
        if (LockedUntil.HasValue && LockedUntil.Value > DateTime.UtcNow)
            throw new InvalidOperationException("Account is locked");

        LastLoginAt = DateTime.UtcNow;
        FailedLoginAttempts = 0;
        LockedUntil = null;
        
        AddEvent(new IdentitySignedIn(Id, LastLoginAt.Value));
    }

    /// <summary>
    /// Records a sign-out.
    /// </summary>
    public void SignOut()
    {
        AddEvent(new IdentitySignedOut(Id, DateTime.UtcNow));
    }

    /// <summary>
    /// Increments the failed login attempts counter.
    /// </summary>
    public void IncrementFailedLoginAttempts()
    {
        FailedLoginAttempts++;
        
        AddEvent(new SignInFailed(Id, FailedLoginAttempts, DateTime.UtcNow));
    }

    /// <summary>
    /// Locks the account until the specified date and time.
    /// </summary>
    /// <param name="lockedUntil">The date and time to lock until.</param>
    public void LockAccount(DateTime lockedUntil)
    {
        LockedUntil = lockedUntil;
        
        AddEvent(new AccountLocked(Id, lockedUntil, DateTime.UtcNow));
    }

    /// <summary>
    /// Unlocks the account.
    /// </summary>
    public void UnlockAccount()
    {
        if (!LockedUntil.HasValue)
            return; // Already unlocked
        
        LockedUntil = null;
        FailedLoginAttempts = 0;
        
        AddEvent(new AccountUnlocked(Id, DateTime.UtcNow));
    }

    /// <summary>
    /// Enables multi-factor authentication.
    /// </summary>
    /// <param name="method">The preferred MFA method.</param>
    public void EnableMfa(MfaMethod method)
    {
        if (Status == IdentityStatus.Revoked)
            throw new InvalidOperationException("Cannot enable MFA for revoked identity");
        
        MultiFactorEnabled = true;
        MultiFactorEnabledAt = DateTime.UtcNow;
        PreferredMfaMethod = method;
        
        AddEvent(new MfaEnabled(Id, method, MultiFactorEnabledAt.Value));
    }

    /// <summary>
    /// Disables multi-factor authentication.
    /// </summary>
    public void DisableMfa()
    {
        if (!MultiFactorEnabled)
            return; // Already disabled
        
        MultiFactorEnabled = false;
        PreferredMfaMethod = null;
        
        AddEvent(new MfaDisabled(Id, DateTime.UtcNow));
    }

    /// <summary>
    /// Sets the preferred MFA method.
    /// </summary>
    /// <param name="method">The preferred MFA method.</param>
    public void SetPreferredMfaMethod(MfaMethod method)
    {
        if (!MultiFactorEnabled)
            throw new InvalidOperationException("MFA must be enabled before setting preferred method");
        
        PreferredMfaMethod = method;
        IncrementVersion();
    }

    /// <summary>
    /// Confirms the email address.
    /// </summary>
    public void ConfirmEmail()
    {
        if (EmailConfirmed)
            return; // Already confirmed
        
        EmailConfirmed = true;
        EmailConfirmedAt = DateTime.UtcNow;
        
        AddEvent(new EmailConfirmed(Id, EmailConfirmedAt.Value));
    }

    /// <summary>
    /// Confirms the phone number.
    /// </summary>
    public void ConfirmPhone()
    {
        if (PhoneConfirmed)
            return; // Already confirmed
        
        PhoneConfirmed = true;
        PhoneConfirmedAt = DateTime.UtcNow;
        
        AddEvent(new SmsConfirmed(Id, PhoneConfirmedAt.Value));
    }

    /// <summary>
    /// Changes the password.
    /// </summary>
    /// <param name="newPasswordHash">The new password hash.</param>
    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash cannot be null or empty", nameof(newPasswordHash));
        
        PasswordHash = newPasswordHash;
        PasswordChangedAt = DateTime.UtcNow;
        
        AddEvent(new PasswordChanged(Id, PasswordChangedAt.Value));
    }

    /// <summary>
    /// Sets the username and password for authentication.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="passwordHash">The password hash.</param>
    public void SetCredentials(string username, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty", nameof(username));
        
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be null or empty", nameof(passwordHash));
        
        Username = username;
        PasswordHash = passwordHash;
        PasswordChangedAt = DateTime.UtcNow;
        IncrementVersion();
    }

    /// <summary>
    /// Sets the Azure user identifier for Azure AD authentication.
    /// </summary>
    /// <param name="azureUserId">The Azure user identifier.</param>
    public void SetAzureUserId(string azureUserId)
    {
        if (string.IsNullOrWhiteSpace(azureUserId))
            throw new ArgumentException("Azure user ID cannot be null or empty", nameof(azureUserId));

        AzureUserId = azureUserId;
        IncrementVersion();
    }

    /// <summary>
    /// Sets the service identifier for service-to-service authentication.
    /// </summary>
    /// <param name="serviceId">The service identifier.</param>
    public void SetServiceId(string serviceId)
    {
        if (string.IsNullOrWhiteSpace(serviceId))
            throw new ArgumentException("Service ID cannot be null or empty", nameof(serviceId));

        ServiceId = serviceId;
        IncrementVersion();
    }

    #endregion
}

