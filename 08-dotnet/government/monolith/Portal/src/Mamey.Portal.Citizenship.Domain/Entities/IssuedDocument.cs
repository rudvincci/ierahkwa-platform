using Mamey.Portal.Citizenship.Domain.Events;
using Mamey.Portal.Citizenship.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.Portal.Citizenship.Domain.Entities;

public sealed class IssuedDocument : AggregateRoot<Guid>
{
    public Guid ApplicationId { get; private set; }
    public DocumentKind Kind { get; private set; }
    public DocumentNumber? DocumentNumber { get; private set; }
    public DateTimeOffset? ExpiresAt { get; private set; }

    public string FileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long Size { get; private set; }

    public string StorageBucket { get; private set; } = string.Empty;
    public string StorageKey { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; private set; }

    private IssuedDocument() { }

    public IssuedDocument(
        Guid id,
        Guid applicationId,
        DocumentKind kind,
        DocumentNumber? documentNumber,
        DateTimeOffset? expiresAt,
        string fileName,
        string contentType,
        long size,
        string storageBucket,
        string storageKey,
        DateTimeOffset createdAt)
        : base(id)
    {
        ApplicationId = applicationId;
        Kind = kind;
        DocumentNumber = documentNumber;
        ExpiresAt = expiresAt;
        FileName = fileName;
        ContentType = contentType;
        Size = size;
        StorageBucket = storageBucket;
        StorageKey = storageKey;
        CreatedAt = createdAt;
        AddEvent(new DocumentIssued(id, applicationId, kind.Value, createdAt));
    }

    public static IssuedDocument Rehydrate(
        Guid id,
        Guid applicationId,
        DocumentKind kind,
        DocumentNumber? documentNumber,
        DateTimeOffset? expiresAt,
        string fileName,
        string contentType,
        long size,
        string storageBucket,
        string storageKey,
        DateTimeOffset createdAt)
    {
        return new IssuedDocument
        {
            Id = id,
            ApplicationId = applicationId,
            Kind = kind,
            DocumentNumber = documentNumber,
            ExpiresAt = expiresAt,
            FileName = fileName,
            ContentType = contentType,
            Size = size,
            StorageBucket = storageBucket,
            StorageKey = storageKey,
            CreatedAt = createdAt
        };
    }
}
