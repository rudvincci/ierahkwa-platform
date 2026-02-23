using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text.Json;
using Pupitre.Educators.Domain.Events;
using Pupitre.Educators.Domain.Exceptions;
using Mamey.Types;

[assembly: InternalsVisibleTo("Pupitre.Educators.Tests.Unit.Core.Entities")]
namespace Pupitre.Educators.Domain.Entities;


internal class Educator : AggregateRoot<EducatorId>
{
    #region Fields
    private ISet<string> _tags = new HashSet<string>();
    #endregion


    public Educator(EducatorId id, string name, DateTime createdAt,
        DateTime? modifiedAt = null, IEnumerable<string>? tags = null,
        string? citizenId = null, string? nationality = null, string? programCode = null,
        string? credentialType = null, DateTime? completionDate = null,
        string? blockchainMetadata = null, string? governmentIdentityId = null,
        string? blockchainAccount = null, string? credentialDocumentId = null,
        string? credentialDocumentHash = null, string? ledgerTransactionId = null,
        DateTime? credentialIssuedAt = null, string? credentialStatus = null, int version = 0)
        : base(id, version)
    {
        Name = name;
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
        Tags = tags ?? Enumerable.Empty<string>();
        CitizenId = citizenId;
        Nationality = nationality;
        ProgramCode = programCode;
        CredentialType = credentialType;
        CompletionDate = completionDate;
        BlockchainMetadata = blockchainMetadata;
        GovernmentIdentityId = governmentIdentityId;
        BlockchainAccount = blockchainAccount;
        CredentialDocumentId = credentialDocumentId;
        CredentialDocumentHash = credentialDocumentHash;
        LedgerTransactionId = ledgerTransactionId;
        CredentialIssuedAt = credentialIssuedAt;
        CredentialStatus = credentialStatus;
    }

    #region Properties

    /// <summary>
    /// A name for the educator.
    /// </summary>
    [Description("The educator's name")]
    public string Name { get; private set; }

    /// <summary>
    /// Date and time the record was created.
    /// </summary>
    [Description("Date and time the record was created.")]
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Date and time the record was modified.
    /// </summary>
    [Description("Date and time the record was modified.")]
    public DateTime? ModifiedAt { get; private set; }

    /// <summary>
    /// Collection of Educator tags.
    /// </summary>
    [Description("Collection of Educator tags.")]
    public IEnumerable<string> Tags
    {
        get => _tags;
        private set
        {
            var sanitized = value ?? Enumerable.Empty<string>();
            ValidateTags(sanitized);
            _tags = new HashSet<string>(sanitized);
        }
    }
    public string? CitizenId { get; private set; }
    public string? Nationality { get; private set; }
    public string? ProgramCode { get; private set; }
    public string? CredentialType { get; private set; }
    public DateTime? CompletionDate { get; private set; }
    public string? GovernmentIdentityId { get; private set; }
    public string? BlockchainAccount { get; private set; }
    public string? CredentialDocumentId { get; private set; }
    public string? CredentialDocumentHash { get; private set; }
    public string? LedgerTransactionId { get; private set; }
    public DateTime? CredentialIssuedAt { get; private set; }
    public string? CredentialStatus { get; private set; }
    public string? BlockchainMetadata { get; private set; }
    #endregion

    public static Educator Create(Guid id, string name, IEnumerable<string>? tags,
        string? citizenId = null, string? nationality = null, string? programCode = null,
        string? credentialType = null, DateTime? completionDate = null,
        IDictionary<string, string>? metadata = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new MissingEducatorNameException();
        }

        var educator = new Educator(id, name, DateTime.UtcNow, tags: tags,
            citizenId: citizenId, nationality: nationality, programCode: programCode,
            credentialType: credentialType, completionDate: completionDate);
        educator.SetBlockchainMetadata(metadata);
        educator.AddEvent(new EducatorCreated(educator));
        return educator;
    }

    public void Update(string name, IEnumerable<string> tags, string? programCode = null,
        string? credentialType = null, DateTime? completionDate = null,
        IDictionary<string, string>? metadata = null, string? nationality = null)
    {
        Name = name;
        Tags = tags;
        ProgramCode = programCode ?? ProgramCode;
        CredentialType = credentialType ?? CredentialType;
        CompletionDate = completionDate ?? CompletionDate;
        Nationality = nationality ?? Nationality;
        SetBlockchainMetadata(metadata);
        ModifiedAt = DateTime.UtcNow;
        this.AddEvent(new EducatorModified(this));
    }

    private static void ValidateTags(IEnumerable<string> tags)
    {
        if (tags.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidEducatorTagsException();
        }
    }

    public void AttachBlockchainReceipt(string identityId, string? blockchainAccount,
        string? documentId, string? documentHash, string? ledgerTransactionId,
        DateTime issuedAt, string? status = "issued")
    {
        GovernmentIdentityId = identityId;
        BlockchainAccount = blockchainAccount;
        CredentialDocumentId = documentId;
        CredentialDocumentHash = documentHash;
        LedgerTransactionId = ledgerTransactionId;
        CredentialIssuedAt = issuedAt;
        CredentialStatus = status ?? CredentialStatus;
        ModifiedAt = issuedAt;

        AddEvent(new EducatorBlockchainRegistered(Id.Value, identityId, ledgerTransactionId));
    }

    public void SetBlockchainMetadata(IDictionary<string, string>? metadata)
    {
        BlockchainMetadata = metadata is { Count: > 0 }
            ? JsonSerializer.Serialize(metadata)
            : null;
    }

    public IReadOnlyDictionary<string, string>? GetBlockchainMetadata()
    {
        if (string.IsNullOrWhiteSpace(BlockchainMetadata))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(BlockchainMetadata!)!;
        }
        catch
        {
            return null;
        }
    }
}

