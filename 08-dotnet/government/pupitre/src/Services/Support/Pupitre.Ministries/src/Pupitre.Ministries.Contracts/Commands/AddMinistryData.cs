using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Pupitre.Ministries.Tests.Integration.Async")]
namespace Pupitre.Ministries.Contracts.Commands;

[Contract]
public record AddMinistryData : ICommand
{
    public AddMinistryData(
        Guid id,
        string? name,
        IEnumerable<string> tags,
        string? citizenId = null,
        string? firstName = null,
        string? lastName = null,
        DateOnly? dateOfBirth = null,
        string? programCode = null,
        string? credentialType = null,
        DateTime? completionDate = null,
        bool publishToLedger = false)
    {
        Id = id;
        Name = name;
        Tags = tags;
        CitizenId = citizenId;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        ProgramCode = programCode;
        CredentialType = credentialType;
        CompletionDate = completionDate;
        PublishToLedger = publishToLedger;
    }

    public Guid Id { get; init; } = Guid.NewGuid();
    public string? Name { get; init; }
    public IEnumerable<string> Tags { get; init; } = Array.Empty<string>();
    public string? CitizenId { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public DateOnly? DateOfBirth { get; init; }
    public string? Nationality { get; init; }
    public string? ProgramCode { get; init; }
    public string? CredentialType { get; init; }
    public DateTime? CompletionDate { get; init; }
    public string? CredentialDocumentBase64 { get; init; }
    public string CredentialMimeType { get; init; } = "application/pdf";
    public bool PublishToLedger { get; init; }
    public IDictionary<string, string>? Metadata { get; init; }
    public string? IdentityId { get; init; }
    public string? BlockchainAccount { get; init; }
}

