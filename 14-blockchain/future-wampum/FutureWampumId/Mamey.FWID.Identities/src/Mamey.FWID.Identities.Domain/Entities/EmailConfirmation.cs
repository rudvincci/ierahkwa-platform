using System.Text.Json.Serialization;
using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Represents an email confirmation token.
/// </summary>
public class EmailConfirmation : AggregateRoot<EmailConfirmationId>
{
    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private EmailConfirmation()
    {
    }

    /// <summary>
    /// Initializes a new instance of the EmailConfirmation aggregate root.
    /// </summary>
    /// <param name="id">The email confirmation identifier.</param>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="email">The email address to confirm.</param>
    /// <param name="token">The confirmation token.</param>
    /// <param name="expiresAt">The expiration date and time.</param>
    [JsonConstructor]
    public EmailConfirmation(
        EmailConfirmationId id,
        IdentityId identityId,
        string email,
        string token,
        DateTime expiresAt)
        : base(id)
    {
        IdentityId = identityId ?? throw new ArgumentNullException(nameof(identityId));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Token = token ?? throw new ArgumentNullException(nameof(token));
        ExpiresAt = expiresAt;
        Status = ConfirmationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        
        AddEvent(new EmailConfirmationSent(IdentityId, Email, Token, CreatedAt, ExpiresAt));
    }

    /// <summary>
    /// The identity identifier.
    /// </summary>
    public IdentityId IdentityId { get; private set; } = null!;

    /// <summary>
    /// The email address to confirm.
    /// </summary>
    public string Email { get; private set; } = null!;

    /// <summary>
    /// The confirmation token.
    /// </summary>
    public string Token { get; private set; } = null!;

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
    /// Confirms the email address.
    /// </summary>
    public void Confirm()
    {
        if (Status == ConfirmationStatus.Confirmed)
            return; // Already confirmed
        
        if (Status == ConfirmationStatus.Expired)
            throw new InvalidOperationException("Cannot confirm expired email confirmation");
        
        if (ExpiresAt < DateTime.UtcNow)
        {
            Status = ConfirmationStatus.Expired;
            AddEvent(new EmailConfirmationExpired(Id, IdentityId, DateTime.UtcNow));
            throw new InvalidOperationException("Email confirmation has expired");
        }
        
        Status = ConfirmationStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
        
        AddEvent(new EmailConfirmed(IdentityId, ConfirmedAt.Value));
    }

    /// <summary>
    /// Marks the confirmation as expired.
    /// </summary>
    public void Expire()
    {
        if (Status == ConfirmationStatus.Expired)
            return; // Already expired
        
        Status = ConfirmationStatus.Expired;
        
        AddEvent(new EmailConfirmationExpired(Id, IdentityId, DateTime.UtcNow));
    }

    /// <summary>
    /// Checks if the confirmation is valid.
    /// </summary>
    public bool IsValid()
    {
        return Status == ConfirmationStatus.Pending && ExpiresAt > DateTime.UtcNow;
    }
}

