using Mamey.Government.Modules.Documents.Core.Domain.Entities;
using System.Text.Json;
using Mamey.Types;

namespace Mamey.Government.Modules.Documents.Core.Mongo.Documents;

internal class DocumentDocument : MicroMonolith.Infrastructure.Mongo.IIdentifiable<Guid>
{
    public DocumentDocument()
    {
    }

    public DocumentDocument(Document document)
    {
        Id = document.Id.Value;
        TenantId = document.TenantId.Value;
        FileName = document.FileName;
        ContentType = document.ContentType;
        FileSize = document.FileSize;
        StorageBucket = document.StorageBucket;
        StorageKey = document.StorageKey;
        Category = document.Category;
        Description = document.Description;
        Metadata = document.Metadata;
        IsActive = document.IsActive;
        DeletedAt = document.DeletedAt;
        CreatedAt = document.CreatedAt;
        UpdatedAt = document.UpdatedAt;
    }

    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string StorageBucket { get; set; } = string.Empty;
    public string StorageKey { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Description { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Document AsEntity()
    {
        var documentId = new Domain.ValueObjects.DocumentId(Id);
        var tenantId = new TenantId(TenantId);
        
        var document = new Document(
            documentId,
            tenantId,
            FileName,
            ContentType,
            FileSize,
            StorageBucket,
            StorageKey,
            Category);
        
        typeof(Document).GetProperty("Description")?.SetValue(document, Description);
        typeof(Document).GetProperty("Metadata")?.SetValue(document, Metadata);
        typeof(Document).GetProperty("IsActive")?.SetValue(document, IsActive);
        typeof(Document).GetProperty("DeletedAt")?.SetValue(document, DeletedAt);
        typeof(Document).GetProperty("CreatedAt")?.SetValue(document, CreatedAt);
        typeof(Document).GetProperty("UpdatedAt")?.SetValue(document, UpdatedAt);
        
        return document;
    }
}
