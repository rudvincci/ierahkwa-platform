using System.Collections.Generic;
using System.Text.Json;
using Pupitre.Lessons.Application.DTO;
using Pupitre.Lessons.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.Lessons.Tests.Integration.Async")]
namespace Pupitre.Lessons.Infrastructure.Mongo.Documents;

internal class LessonDocument : IIdentifiable<Guid>
{
    public LessonDocument()
    {

    }

    public LessonDocument(Lesson lesson)
    {
        if (lesson is null)
        {
            throw new NullReferenceException();
        }

        Id = lesson.Id.Value;
        Name = lesson.Name;
        CreatedAt = lesson.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = lesson.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = lesson.Tags;
        Version = lesson.Version;
        CitizenId = lesson.CitizenId;
        Nationality = lesson.Nationality;
        ProgramCode = lesson.ProgramCode;
        CredentialType = lesson.CredentialType;
        CompletionDate = lesson.CompletionDate?.ToUnixTimeMilliseconds();
        GovernmentIdentityId = lesson.GovernmentIdentityId;
        BlockchainAccount = lesson.BlockchainAccount;
        CredentialDocumentId = lesson.CredentialDocumentId;
        CredentialDocumentHash = lesson.CredentialDocumentHash;
        LedgerTransactionId = lesson.LedgerTransactionId;
        CredentialIssuedAt = lesson.CredentialIssuedAt?.ToUnixTimeMilliseconds();
        CredentialStatus = lesson.CredentialStatus;
        BlockchainMetadata = lesson.GetBlockchainMetadata();
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public int Version { get; set; }
    public string? CitizenId { get; set; }
    public string? Nationality { get; set; }
    public string? ProgramCode { get; set; }
    public string? CredentialType { get; set; }
    public long? CompletionDate { get; set; }
    public string? GovernmentIdentityId { get; set; }
    public string? BlockchainAccount { get; set; }
    public string? CredentialDocumentId { get; set; }
    public string? CredentialDocumentHash { get; set; }
    public string? LedgerTransactionId { get; set; }
    public long? CredentialIssuedAt { get; set; }
    public string? CredentialStatus { get; set; }
    public IReadOnlyDictionary<string, string>? BlockchainMetadata { get; set; }

    public Lesson AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags,
            CitizenId, Nationality, ProgramCode, CredentialType, CompletionDate?.GetDate(),
            SerializeMetadata(BlockchainMetadata), GovernmentIdentityId,
            BlockchainAccount, CredentialDocumentId, CredentialDocumentHash,
            LedgerTransactionId, CredentialIssuedAt?.GetDate(), CredentialStatus, Version);

    public LessonDto AsDto()
        => new LessonDto(Id, Name, Tags, ProgramCode, CredentialType, CitizenId,
            GovernmentIdentityId, BlockchainAccount, CredentialStatus, Nationality, BlockchainMetadata);
    public LessonDetailsDto AsDetailsDto()
        => new LessonDetailsDto(
            this.AsDto(),
            CreatedAt.GetDate(),
            ModifiedAt?.GetDate(),
            CompletionDate?.GetDate(),
            CredentialIssuedAt?.GetDate(),
            CredentialDocumentId,
            LedgerTransactionId);

    private static string? SerializeMetadata(IReadOnlyDictionary<string, string>? metadata)
        => metadata is { Count: > 0 } ? JsonSerializer.Serialize(metadata) : null;
}

