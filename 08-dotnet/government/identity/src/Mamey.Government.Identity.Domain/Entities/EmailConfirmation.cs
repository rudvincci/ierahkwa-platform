using System.ComponentModel;
using Mamey.Government.Identity.Domain.Events;
using Mamey.Government.Identity.Domain.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Entities;

internal class EmailConfirmation : AggregateRoot<EmailConfirmationId>
{
    public EmailConfirmation(EmailConfirmationId id, UserId userId, string email, string confirmationCode,
        DateTime createdAt, DateTime expiresAt, EmailConfirmationStatus status = EmailConfirmationStatus.Pending,
        DateTime? confirmedAt = null, string? ipAddress = null, string? userAgent = null, int version = 0)
        : base(id, version)
    {
        UserId = userId;
        Email = email;
        ConfirmationCode = confirmationCode;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
        Status = status;
        ConfirmedAt = confirmedAt;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }

    #region Properties

    /// <summary>
    /// Reference to the user this confirmation belongs to.
    /// </summary>
    [Description("Reference to the user this confirmation belongs to")]
    public UserId UserId { get; private set; }

    /// <summary>
    /// Email address to be confirmed.
    /// </summary>
    [Description("Email address to be confirmed")]
    public string Email { get; private set; }

    /// <summary>
    /// Confirmation code sent to the email.
    /// </summary>
    [Description("Confirmation code sent to the email")]
    public string ConfirmationCode { get; private set; }

    /// <summary>
    /// Date and time the confirmation was created.
    /// </summary>
    [Description("Date and time the confirmation was created")]
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Date and time the confirmation expires.
    /// </summary>
    [Description("Date and time the confirmation expires")]
    public DateTime ExpiresAt { get; private set; }

    /// <summary>
    /// Current status of the confirmation.
    /// </summary>
    [Description("Current status of the confirmation")]
    public EmailConfirmationStatus Status { get; private set; }

    /// <summary>
    /// Date and time the email was confirmed.
    /// </summary>
    [Description("Date and time the email was confirmed")]
    public DateTime? ConfirmedAt { get; private set; }

    /// <summary>
    /// Number of attempts to confirm the email.
    /// </summary>
    [Description("Number of attempts to confirm the email")]
    public int AttemptCount { get; private set; }

    /// <summary>
    /// IP address from which the confirmation was requested.
    /// </summary>
    [Description("IP address from which the confirmation was requested")]
    public string? IpAddress { get; private set; }

    /// <summary>
    /// User agent string from the client.
    /// </summary>
    [Description("User agent string from the client")]
    public string? UserAgent { get; private set; }
    #endregion

    public static EmailConfirmation Create(Guid id, Guid userId, string email, string confirmationCode,
        DateTime expiresAt, string? ipAddress = null, string? userAgent = null)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new MissingEmailException();
        }

        if (string.IsNullOrWhiteSpace(confirmationCode))
        {
            throw new MissingConfirmationCodeException();
        }

        if (!IsValidEmail(email))
        {
            throw new InvalidEmailException();
        }

        if (expiresAt <= DateTime.UtcNow)
        {
            throw new InvalidExpirationTimeException();
        }

        var confirmation = new EmailConfirmation(id, userId, email, confirmationCode, DateTime.UtcNow, expiresAt, 
            ipAddress: ipAddress, userAgent: userAgent);
        confirmation.AddEvent(new EmailConfirmationCreated(confirmation));
        return confirmation;
    }

    public void Confirm(string confirmationCode)
    {
        if (Status == EmailConfirmationStatus.Confirmed)
        {
            throw new EmailAlreadyConfirmedException();
        }

        if (Status == EmailConfirmationStatus.Expired)
        {
            throw new EmailConfirmationExpiredException();
        }

        if (IsExpired())
        {
            Status = EmailConfirmationStatus.Expired;
            AddEvent(new EmailConfirmationExpired(this));
            throw new EmailConfirmationExpiredException();
        }

        if (!string.Equals(ConfirmationCode, confirmationCode, StringComparison.OrdinalIgnoreCase))
        {
            AttemptCount++;
            AddEvent(new EmailConfirmationFailed(this, AttemptCount));
            throw new InvalidConfirmationCodeException();
        }

        Status = EmailConfirmationStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
        AddEvent(new EmailConfirmationConfirmed(this));
    }

    public void Resend(string newConfirmationCode, DateTime newExpiresAt)
    {
        if (Status == EmailConfirmationStatus.Confirmed)
        {
            throw new EmailAlreadyConfirmedException();
        }

        if (string.IsNullOrWhiteSpace(newConfirmationCode))
        {
            throw new MissingConfirmationCodeException();
        }

        if (newExpiresAt <= DateTime.UtcNow)
        {
            throw new InvalidExpirationTimeException();
        }

        ConfirmationCode = newConfirmationCode;
        ExpiresAt = newExpiresAt;
        Status = EmailConfirmationStatus.Pending;
        AddEvent(new EmailConfirmationResent(this));
    }

    public void Expire()
    {
        if (Status == EmailConfirmationStatus.Confirmed)
        {
            throw new EmailAlreadyConfirmedException();
        }

        if (Status == EmailConfirmationStatus.Expired)
        {
            return; // Already expired
        }

        Status = EmailConfirmationStatus.Expired;
        AddEvent(new EmailConfirmationExpired(this));
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpiresAt || Status == EmailConfirmationStatus.Expired;
    }

    public bool IsValid()
    {
        return Status == EmailConfirmationStatus.Pending && !IsExpired();
    }

    public bool CanBeResent()
    {
        return Status == EmailConfirmationStatus.Pending && !IsExpired();
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}