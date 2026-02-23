using Mamey.Portal.Library.Application.Models;

namespace Mamey.Portal.Library.Application.Services;

public sealed record LibraryItemSnapshot(
    Guid Id,
    string TenantId,
    string Category,
    string Title,
    string? Summary,
    LibraryVisibility Visibility,
    LibraryContentStatus Status,
    string FileName,
    string ContentType,
    long Size,
    string StorageBucket,
    string StorageKey,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    string? CreatedBy,
    string? UpdatedBy,
    DateTimeOffset? PublishedAt);
