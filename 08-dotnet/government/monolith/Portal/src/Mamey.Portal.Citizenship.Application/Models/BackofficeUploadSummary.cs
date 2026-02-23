namespace Mamey.Portal.Citizenship.Application.Models;

public sealed record BackofficeUploadSummary(
    Guid Id,
    string Kind,
    string FileName,
    string ContentType,
    long Size,
    string StorageBucket,
    string StorageKey,
    DateTimeOffset UploadedAt);




