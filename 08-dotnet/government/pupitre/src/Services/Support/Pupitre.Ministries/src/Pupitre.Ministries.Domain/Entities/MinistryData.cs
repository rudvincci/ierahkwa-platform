using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Text.Json;
using Pupitre.Ministries.Domain.Events;
using Pupitre.Ministries.Domain.Exceptions;
using Mamey.Types;

[assembly: InternalsVisibleTo("Pupitre.Ministries.Tests.Unit.Core.Entities")]
namespace Pupitre.Ministries.Domain.Entities;


internal class MinistryData : AggregateRoot<MinistryDataId>
{
    #region Fields
    private ISet<string> _tags = new HashSet<string>();
    #endregion


    public MinistryData(MinistryDataId id, string name, DateTime createdAt,
        DateTime? modifiedAt = null, IEnumerable<string>? tags = null, string? citizenId = null,
        string? programCode = null, string? credentialType = null, DateTime? completionDate = null,
        string? blockchainMetadata = null, string? governmentIdentityId = null,
        string? blockchainAccount = null, string? credentialDocumentId = null,
        string? credentialDocumentHash = null, string? ledgerTransactionId = null,
        DateTime? credentialIssuedAt = null, string? credentialStatus = null, string? nationality = null, int version = 0)
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
    /// A name for the ministrydata.
    /// </summary>
    [Description("The ministrydata's name")]
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
    /// Collection of MinistryData tags.
    /// </summary>
    [Description("Collection of MinistryData tags.")]
    public IEnumerable<string> Tags
    {
        get => _tags;
        private set
        {
            ValidateTags(value);
            _tags = new HashSet<string>(value);
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

    public static MinistryData Create(Guid id, string name, IEnumerable<string>? tags,
        string? citizenId = null, string? programCode = null, string? credentialType = null,
        DateTime? completionDate = null, IDictionary<string, string>? metadata = null, string? nationality = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new MissingMinistryDataNameException();
        }

        var ministrydata = new MinistryData(id, name, DateTime.UtcNow, tags: tags,
            citizenId: citizenId, programCode: programCode, credentialType: credentialType,
            completionDate: completionDate, nationality: nationality);
        ministrydata.SetBlockchainMetadata(metadata);
        ministrydata.AddEvent(new MinistryDataCreated(ministrydata));
        return ministrydata;
    }

    public void Update(string name, IEnumerable<string> tags, string? programCode = null,
        string? credentialType = null, DateTime? completionDate = null,
        IDictionary<string, string>? metadata = null)
    {
        Name = name;
        Tags = tags;
        ProgramCode = programCode ?? ProgramCode;
        CredentialType = credentialType ?? CredentialType;
        CompletionDate = completionDate ?? CompletionDate;
        SetBlockchainMetadata(metadata);
        ModifiedAt = DateTime.UtcNow;
        this.AddEvent(new MinistryDataModified(this));
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

        AddEvent(new MinistryDataBlockchainRegistered(Id.Value, identityId, ledgerTransactionId));
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

    private static void ValidateTags(IEnumerable<string> tags)
    {
        if (tags.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidMinistryDataTagsException();
        }
    }
}

