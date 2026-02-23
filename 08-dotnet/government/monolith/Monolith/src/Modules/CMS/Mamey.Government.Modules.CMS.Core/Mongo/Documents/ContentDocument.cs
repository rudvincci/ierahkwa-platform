using Mamey.Government.Modules.CMS.Core.Domain.Entities;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using GovTenantId = Mamey.Types.TenantId;

namespace Mamey.Government.Modules.CMS.Core.Mongo.Documents;

internal class ContentDocument : IIdentifiable<Guid>
{
    public ContentDocument()
    {
    }

    public ContentDocument(Content content)
    {
        Id = content.Id.Value;
        TenantId = content.TenantId.Value;
        Title = content.Title;
        Slug = content.Slug;
        ContentType = content.ContentType;
        Body = content.Body;
        Excerpt = content.Excerpt;
        Status = content.Status.ToString();
        PublishedAt = content.PublishedAt;
        Metadata = content.Metadata;
        CreatedAt = content.CreatedAt;
        UpdatedAt = content.UpdatedAt;
    }

    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string? Body { get; set; }
    public string? Excerpt { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? PublishedAt { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Content AsEntity()
    {
        var contentId = new Domain.ValueObjects.ContentId(Id);
        var tenantId = new GovTenantId(TenantId);
        var status = Enum.Parse<Domain.ValueObjects.ContentStatus>(Status);
        
        var content = new Content(
            contentId,
            tenantId,
            Title,
            Slug,
            ContentType,
            status);
        
        typeof(Content).GetProperty("Body")?.SetValue(content, Body);
        typeof(Content).GetProperty("Excerpt")?.SetValue(content, Excerpt);
        typeof(Content).GetProperty("PublishedAt")?.SetValue(content, PublishedAt);
        typeof(Content).GetProperty("Metadata")?.SetValue(content, Metadata);
        typeof(Content).GetProperty("CreatedAt")?.SetValue(content, CreatedAt);
        typeof(Content).GetProperty("UpdatedAt")?.SetValue(content, UpdatedAt);
        
        return content;
    }
}
