using Mamey.Portal.Library.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.Portal.Library.Domain.Entities;

public sealed class LibraryItem : AggregateRoot<Guid>
{
    public string TenantId { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string? Summary { get; private set; }
    public LibraryVisibility Visibility { get; private set; }
    public LibraryContentStatus Status { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long Size { get; private set; }
    public string StorageBucket { get; private set; } = string.Empty;
    public string StorageKey { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public string? CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }
    public DateTimeOffset? PublishedAt { get; private set; }

    private LibraryItem() { }

    public LibraryItem(
        Guid id,
        string tenantId,
        string category,
        string title,
        string? summary,
        LibraryVisibility visibility,
        LibraryContentStatus status,
        string fileName,
        string contentType,
        long size,
        string storageBucket,
        string storageKey,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
        : base(id)
    {
        TenantId = tenantId;
        Category = category;
        Title = title;
        Summary = summary;
        Visibility = visibility;
        Status = status;
        FileName = fileName;
        ContentType = contentType;
        Size = size;
        StorageBucket = storageBucket;
        StorageKey = storageKey;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static LibraryItem Rehydrate(
        Guid id,
        string tenantId,
        string category,
        string title,
        string? summary,
        LibraryVisibility visibility,
        LibraryContentStatus status,
        string fileName,
        string contentType,
        long size,
        string storageBucket,
        string storageKey,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt,
        string? createdBy,
        string? updatedBy,
        DateTimeOffset? publishedAt)
    {
        var item = new LibraryItem(
            id,
            tenantId,
            category,
            title,
            summary,
            visibility,
            status,
            fileName,
            contentType,
            size,
            storageBucket,
            storageKey,
            createdAt,
            updatedAt)
        {
            CreatedBy = createdBy,
            UpdatedBy = updatedBy,
            PublishedAt = publishedAt
        };

        return item;
    }
}
