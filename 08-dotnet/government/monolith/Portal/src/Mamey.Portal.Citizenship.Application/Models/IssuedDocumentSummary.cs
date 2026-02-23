namespace Mamey.Portal.Citizenship.Application.Models;

public sealed record IssuedDocumentSummary(
    Guid Id,
    Guid ApplicationId,
    string Kind,
    string? DocumentNumber,
    DateTimeOffset? ExpiresAt,
    string FileName,
    string ContentType,
    long Size,
    string StorageBucket,
    string StorageKey,
    DateTimeOffset CreatedAt);


