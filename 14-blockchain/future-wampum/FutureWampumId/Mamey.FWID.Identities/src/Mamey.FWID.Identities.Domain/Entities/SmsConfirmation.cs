using System.Text.Json.Serialization;
using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Represents an SMS confirmation code.
/// </summary>
public class SmsConfirmation : AggregateRoot<SmsConfirmationId>
{
    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private SmsConfirmation()
    {
    }

    /// <summary>
    /// Initializes a new instance of the SmsConfirmation aggregate root.
    /// </summary>
    /// <param name="id">The SMS confirmation identifier.</param>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="phoneNumber">The phone number to confirm.</param>
    /// <param name="code">The confirmation code.</param>
    /// <param name="expiresAt">The expiration date and time.</param>
    [JsonConstructor]
    public SmsConfirmation(
        SmsConfirmationId id,
        IdentityId identityId,
        string phoneNumber,
        string code,
        DateTime expiresAt)
        : base(id)
    {
        IdentityId = identityId ?? throw new ArgumentNullException(nameof(identityId));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        Code = code ?? throw new ArgumentNullException(nameof(code));
        ExpiresAt = expiresAt;
        Status = ConfirmationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        
        AddEvent(new SmsConfirmationSent(IdentityId, PhoneNumber, Code, CreatedAt, ExpiresAt));
    }

    /// <summary>
    /// The identity identifier.
    /// </summary>
    public IdentityId IdentityId { get; private set; } = null!;

    /// <summary>
    /// The phone number to confirm.
    /// </summary>
    public string PhoneNumber { get; private set; } = null!;

    /// <summary>
    /// The confirmation code.
    /// </summary>
    public string Code { get; private set; } = null!;

    /// <summary>
    /// The expiration date and time.
    /// </summary>
    public DateTime ExpiresAt { get; private set; }

    /// <summary>
    /// The status of the confirmation.
    /// </summary>
    public ConfirmationStatus Status { get; private set; }

    /// <summary>
    /// Date and time the confirmation was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Date and time the confirmation was confirmed.
    /// </summary>
    public DateTime? ConfirmedAt { get; private set; }

    /// <summary>
    /// Number of verification attempts.
    /// </summary>
    public int VerificationAttempts { get; private set; }

    /// <summary>
    /// Confirms the phone number.
    /// </summary>
    /// <param name="code">The confirmation code.</param>
    public void Confirm(string code)
    {
        if (Status == ConfirmationStatus.Confirmed)
            return; // Already confirmed
        
        if (Status == ConfirmationStatus.Expired)
            throw new InvalidOperationException("Cannot confirm expired SMS confirmation");
        
        if (ExpiresAt < DateTime.UtcNow)
        {
            Status = ConfirmationStatus.Expired;
            AddEvent(new SmsConfirmationExpired(Id, IdentityId, DateTime.UtcNow));
            throw new InvalidOperationException("SMS confirmation has expired");
        }
        
        VerificationAttempts++;
        
        if (Code != code)
        {
            if (VerificationAttempts >= 3)
            {
                Status = ConfirmationStatus.Expired;
                AddEvent(new SmsConfirmationExpired(Id, IdentityId, DateTime.UtcNow));
                throw new InvalidOperationException("Too many failed verification attempts");
            }
            throw new InvalidOperationException("Invalid confirmation code");
        }
        
        Status = ConfirmationStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
        
        AddEvent(new SmsConfirmed(IdentityId, ConfirmedAt.Value));
    }

    /// <summary>
    /// Marks the confirmation as expired.
    /// </summary>
    public void Expire()
    {
        if (Status == ConfirmationStatus.Expired)
            return; // Already expired
        
        Status = ConfirmationStatus.Expired;
        
        AddEvent(new SmsConfirmationExpired(Id, IdentityId, DateTime.UtcNow));
    }

    /// <summary>
    /// Checks if the confirmation is valid.
    /// </summary>
    public bool IsValid()
    {
        return Status == ConfirmationStatus.Pending && ExpiresAt > DateTime.UtcNow && VerificationAttempts < 3;
    }
}

