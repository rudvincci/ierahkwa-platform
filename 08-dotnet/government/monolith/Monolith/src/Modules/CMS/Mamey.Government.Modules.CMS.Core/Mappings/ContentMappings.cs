using Mamey.Government.Modules.CMS.Core.Domain.Entities;
using Mamey.Government.Modules.CMS.Core.DTO;

namespace Mamey.Government.Modules.CMS.Core.Mappings;

internal static class ContentMappings
{
    public static ContentDto AsDto(this Content content)
        => new()
        {
            Id = content.Id.Value,
            TenantId = content.TenantId.Value,
            Title = content.Title,
            Slug = content.Slug,
            ContentType = content.ContentType,
            Body = content.Body,
            Excerpt = content.Excerpt,
            Status = content.Status.ToString(),
            PublishedAt = content.PublishedAt,
            Metadata = content.Metadata,
            CreatedAt = content.CreatedAt,
            UpdatedAt = content.UpdatedAt
        };

    public static ContentSummaryDto AsSummaryDto(this Content content)
        => new()
        {
            Id = content.Id.Value,
            Title = content.Title,
            Slug = content.Slug,
            ContentType = content.ContentType,
            Excerpt = content.Excerpt,
            Status = content.Status.ToString(),
            PublishedAt = content.PublishedAt,
            CreatedAt = content.CreatedAt
        };
}
