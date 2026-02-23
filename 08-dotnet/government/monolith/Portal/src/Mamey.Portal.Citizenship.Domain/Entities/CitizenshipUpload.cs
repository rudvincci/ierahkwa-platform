using Mamey.Portal.Citizenship.Domain.ValueObjects;

namespace Mamey.Portal.Citizenship.Domain.Entities;

public sealed class CitizenshipUpload
{
    public Guid Id { get; private set; }
    public Guid ApplicationId { get; private set; }
    public DocumentKind Kind { get; private set; }

    public string FileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long Size { get; private set; }

    public string StorageBucket { get; private set; } = string.Empty;
    public string StorageKey { get; private set; } = string.Empty;

    public DateTimeOffset UploadedAt { get; private set; }

    private CitizenshipUpload() { }

    public CitizenshipUpload(
        Guid id,
        Guid applicationId,
        DocumentKind kind,
        string fileName,
        string contentType,
        long size,
        string storageBucket,
        string storageKey,
        DateTimeOffset uploadedAt)
    {
        Id = id;
        ApplicationId = applicationId;
        Kind = kind;
        FileName = fileName;
        ContentType = contentType;
        Size = size;
        StorageBucket = storageBucket;
        StorageKey = storageKey;
        UploadedAt = uploadedAt;
    }
}
