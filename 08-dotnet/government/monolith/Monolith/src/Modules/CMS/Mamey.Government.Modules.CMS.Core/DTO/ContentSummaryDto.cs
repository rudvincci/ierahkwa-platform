using System;

namespace Mamey.Government.Modules.CMS.Core.DTO;

public class ContentSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
