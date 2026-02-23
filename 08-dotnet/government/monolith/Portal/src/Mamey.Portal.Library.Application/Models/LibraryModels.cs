namespace Mamey.Portal.Library.Application.Models;

public enum LibraryVisibility
{
    Public = 0,
    Citizen = 1,
    Government = 2,
}

public enum LibraryContentStatus
{
    Draft = 0,
    Published = 1,
    Unpublished = 2,
}

public sealed record LibraryItem(
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




