namespace Mamey.Portal.Citizenship.Infrastructure.Persistence;

public sealed class CitizenshipIssuedDocumentRow
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }

    public string Kind { get; set; } = string.Empty;
    public string? DocumentNumber { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }

    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }

    public string StorageBucket { get; set; } = string.Empty;
    public string StorageKey { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }
}


