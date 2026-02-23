using System.Runtime.CompilerServices;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.Types;

[assembly: InternalsVisibleTo("Mamey.FWID.Identities.Tests.Unit.Core.Entities")]
namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Represents a disclosure aggregate root for selective data sharing.
/// </summary>
internal class Disclosure : AggregateRoot<DisclosureId>
{
    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private Disclosure()
    {
        DataFields = new List<DataField>();
        Recipients = new List<string>();
        Policies = new List<DisclosurePolicy>();
        Metadata = new Dictionary<string, object>();
    }

    /// <summary>
    /// Initializes a new instance of the Disclosure aggregate root.
    /// </summary>
    /// <param name="id">The disclosure identifier.</param>
    /// <param name="identityId">The identity that owns the data.</param>
    /// <param name="templateId">The disclosure template identifier.</param>
    /// <param name="purpose">The purpose of the disclosure.</param>
    /// <param name="recipients">The recipients of the disclosure.</param>
    public Disclosure(
        DisclosureId id,
        IdentityId identityId,
        string templateId,
        string purpose,
        List<string> recipients)
        : base(id)
    {
        IdentityId = identityId ?? throw new ArgumentNullException(nameof(identityId));
        TemplateId = templateId ?? throw new ArgumentNullException(nameof(templateId));
        Purpose = purpose ?? throw new ArgumentNullException(nameof(purpose));
        Recipients = recipients ?? new List<string>();
        Status = DisclosureStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        DataFields = new List<DataField>();
        Policies = new List<DisclosurePolicy>();
        Metadata = new Dictionary<string, object>();
        Version = 1;

        AddEvent(new DisclosureCreated(Id, IdentityId, TemplateId, Purpose, Recipients, CreatedAt));
    }

    #region Properties

    /// <summary>
    /// The identity that owns the disclosed data.
    /// </summary>
    public IdentityId IdentityId { get; private set; }

    /// <summary>
    /// The disclosure template identifier.
    /// </summary>
    public string TemplateId { get; private set; }

    /// <summary>
    /// The purpose of the disclosure.
    /// </summary>
    public string Purpose { get; private set; }

    /// <summary>
    /// The recipients of the disclosure.
    /// </summary>
    public List<string> Recipients { get; private set; }

    /// <summary>
    /// The current status of the disclosure.
    /// </summary>
    public DisclosureStatus Status { get; private set; }

    /// <summary>
    /// When the disclosure was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// When the disclosure was approved/issued.
    /// </summary>
    public DateTime? IssuedAt { get; private set; }

    /// <summary>
    /// When the disclosure expires.
    /// </summary>
    public DateTime? ExpiresAt { get; private set; }

    /// <summary>
    /// The verifiable presentation JWT.
    /// </summary>
    public string? VerifiablePresentation { get; private set; }

    /// <summary>
    /// The data fields being disclosed.
    /// </summary>
    public List<DataField> DataFields { get; private set; }

    /// <summary>
    /// The disclosure policies applied.
    /// </summary>
    public List<DisclosurePolicy> Policies { get; private set; }

    /// <summary>
    /// Additional metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; private set; }

    #endregion

    #region Domain Methods

    /// <summary>
    /// Approves the disclosure request.
    /// </summary>
    /// <param name="verifiablePresentation">The signed verifiable presentation.</param>
    /// <param name="expiresAt">When the disclosure expires.</param>
    public void Approve(string verifiablePresentation, DateTime? expiresAt = null)
    {
        if (Status != DisclosureStatus.Pending)
            throw new InvalidOperationException("Disclosure must be pending to approve");

        Status = DisclosureStatus.Approved;
        VerifiablePresentation = verifiablePresentation;
        IssuedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt ?? IssuedAt.Value.AddHours(24); // Default 24 hours
        IncrementVersion();

        AddEvent(new DisclosureApproved(Id, verifiablePresentation, IssuedAt.Value, ExpiresAt.Value));
    }

    /// <summary>
    /// Rejects the disclosure request.
    /// </summary>
    /// <param name="reason">The reason for rejection.</param>
    public void Reject(string reason)
    {
        if (Status != DisclosureStatus.Pending)
            throw new InvalidOperationException("Disclosure must be pending to reject");

        Status = DisclosureStatus.Rejected;
        IncrementVersion();

        AddEvent(new DisclosureRejected(Id, reason, DateTime.UtcNow));
    }

    /// <summary>
    /// Revokes an approved disclosure.
    /// </summary>
    /// <param name="reason">The reason for revocation.</param>
    public void Revoke(string reason)
    {
        if (Status != DisclosureStatus.Approved)
            throw new InvalidOperationException("Only approved disclosures can be revoked");

        Status = DisclosureStatus.Revoked;
        IncrementVersion();

        AddEvent(new DisclosureRevoked(Id, reason, DateTime.UtcNow));
    }

    /// <summary>
    /// Adds a data field to the disclosure.
    /// </summary>
    /// <param name="field">The data field to add.</param>
    public void AddDataField(DataField field)
    {
        if (Status != DisclosureStatus.Pending)
            throw new InvalidOperationException("Cannot modify data fields after approval");

        DataFields.Add(field);
        IncrementVersion();
    }

    /// <summary>
    /// Adds a disclosure policy.
    /// </summary>
    /// <param name="policy">The policy to add.</param>
    public void AddPolicy(DisclosurePolicy policy)
    {
        Policies.Add(policy);
        IncrementVersion();
    }

    /// <summary>
    /// Checks if the disclosure is currently valid.
    /// </summary>
    /// <returns>True if the disclosure is approved and not expired.</returns>
    public bool IsValid()
    {
        return Status == DisclosureStatus.Approved &&
               (ExpiresAt == null || ExpiresAt > DateTime.UtcNow);
    }

    /// <summary>
    /// Checks if a recipient is authorized for this disclosure.
    /// </summary>
    /// <param name="recipient">The recipient to check.</param>
    /// <returns>True if the recipient is authorized.</returns>
    public bool IsRecipientAuthorized(string recipient)
    {
        return Recipients.Contains(recipient) || Recipients.Contains("*"); // Wildcard for public disclosures
    }

    /// <summary>
    /// Gets the disclosed data fields for a specific recipient.
    /// </summary>
    /// <param name="recipient">The recipient requesting the data.</param>
    /// <returns>The data fields that can be disclosed to this recipient.</returns>
    public IEnumerable<DataField> GetDisclosedFieldsForRecipient(string recipient)
    {
        if (!IsRecipientAuthorized(recipient) || !IsValid())
            return Enumerable.Empty<DataField>();

        // Apply policies to filter fields
        return DataFields.Where(field =>
            Policies.All(policy => policy.AllowsDisclosure(field, recipient)));
    }

    #endregion
}

/// <summary>
/// Represents the status of a disclosure.
/// </summary>
internal enum DisclosureStatus
{
    Pending,
    Approved,
    Rejected,
    Revoked,
    Expired
}

/// <summary>
/// Represents a data field that can be disclosed.
/// </summary>
internal class DataField : IEquatable<DataField>
{
    public string Name { get; set; }
    public string Value { get; set; }
    public string Type { get; set; }
    public bool IsSensitive { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();

    public DataField(string name, string value, string type, bool isSensitive = false)
    {
        Name = name;
        Value = value;
        Type = type;
        IsSensitive = isSensitive;
    }

    public bool Equals(DataField? other)
    {
        return other != null && Name == other.Name;
    }

    public override bool Equals(object? obj) => Equals(obj as DataField);
    public override int GetHashCode() => Name.GetHashCode();
}

/// <summary>
/// Represents a disclosure policy.
/// </summary>
internal class DisclosurePolicy : IEquatable<DisclosurePolicy>
{
    public string Name { get; set; }
    public string Type { get; set; }
    public Dictionary<string, object> Rules { get; set; } = new();

    public DisclosurePolicy(string name, string type)
    {
        Name = name;
        Type = type;
    }

    public bool AllowsDisclosure(DataField field, string recipient)
    {
        // Implement policy logic based on type
        switch (Type.ToLower())
        {
            case "purpose":
                return Rules.TryGetValue("allowedPurposes", out var purposes) &&
                       purposes is List<string> purposeList &&
                       purposeList.Contains(recipient);

            case "recipient":
                return Rules.TryGetValue("allowedRecipients", out var recipients) &&
                       recipients is List<string> recipientList &&
                       recipientList.Contains(recipient);

            case "field":
                return !field.IsSensitive ||
                       (Rules.TryGetValue("allowSensitive", out var allowSensitive) &&
                        allowSensitive is bool allow && allow);

            default:
                return true; // Default allow
        }
    }

    public bool Equals(DisclosurePolicy? other)
    {
        return other != null && Name == other.Name;
    }

    public override bool Equals(object? obj) => Equals(obj as DisclosurePolicy);
    public override int GetHashCode() => Name.GetHashCode();
}
