using Mamey.Government.Modules.Documents.Core.Domain.Events;
using Mamey.Government.Modules.Documents.Core.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.Government.Modules.Documents.Core.Domain.Entities;

/// <summary>
/// Document aggregate root - represents a document stored in MinIO.
/// Used for document library and file management.
/// </summary>
internal class Document : AggregateRoot<DocumentId>
{
    private Document() { }

    public Document(
        DocumentId id,
        TenantId tenantId,
        string fileName,
        string contentType,
        long fileSize,
        string storageBucket,
        string storageKey,
        string? category = null,
        int version = 0)
        : base(id, version)
    {
        TenantId = tenantId;
        FileName = fileName;
        ContentType = contentType;
        FileSize = fileSize;
        StorageBucket = storageBucket;
        StorageKey = storageKey;
        Category = category;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new DocumentCreated(Id, TenantId, FileName));
    }

    public TenantId TenantId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public string StorageBucket { get; private set; } = string.Empty;
    public string StorageKey { get; private set; } = string.Empty; // MinIO object key
    public string? Category { get; private set; }
    public string? Description { get; private set; }
    public Dictionary<string, string> Metadata { get; private set; } = new();
    public bool IsActive { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public void UpdateMetadata(string key, string value)
    {
        Metadata[key] = value;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new DocumentModified(this));
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new DocumentModified(this));
    }

    public void Delete()
    {
        IsActive = false;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new DocumentDeleted(Id));
    }
}
