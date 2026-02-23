using Mamey.Government.Modules.CMS.Core.Domain.Events;
using Mamey.Government.Modules.CMS.Core.Domain.ValueObjects;
using Mamey.Types;
using GovTenantId = Mamey.Types.TenantId;

namespace Mamey.Government.Modules.CMS.Core.Domain.Entities;

/// <summary>
/// CMS Content aggregate root - represents a content item (page, article, etc.).
/// </summary>
public class Content : AggregateRoot<ContentId>
{
    private Content() { }

    public Content(
        ContentId id,
        GovTenantId tenantId,
        string title,
        string slug,
        string contentType,
        ContentStatus status = ContentStatus.Draft,
        int version = 0)
        : base(id, version)
    {
        TenantId = tenantId;
        Title = title;
        Slug = slug;
        ContentType = contentType;
        Status = status;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new ContentCreated(Id, TenantId, Title, Slug));
    }

    public GovTenantId TenantId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty; // URL-friendly identifier
    public string ContentType { get; private set; } = string.Empty; // page, article, etc.
    public string? Body { get; private set; } // HTML/Markdown content
    public string? Excerpt { get; private set; }
    public ContentStatus Status { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public Dictionary<string, string> Metadata { get; private set; } = new();
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public void UpdateContent(string? body, string? excerpt)
    {
        Body = body;
        Excerpt = excerpt;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new ContentModified(this));
    }

    public void Publish()
    {
        Status = ContentStatus.Published;
        PublishedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new ContentPublished(Id, PublishedAt.Value));
    }

    public void Archive()
    {
        Status = ContentStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new ContentModified(this));
    }

    public void UpdateStatus(ContentStatus newStatus)
    {
        if (Status == newStatus) return;
        
        Status = newStatus;
        if (newStatus == ContentStatus.Published && PublishedAt == null)
        {
            PublishedAt = DateTime.UtcNow;
        }
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new ContentModified(this));
    }

    public void UpdateMetadata(string key, string value)
    {
        Metadata[key] = value;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new ContentModified(this));
    }
}
