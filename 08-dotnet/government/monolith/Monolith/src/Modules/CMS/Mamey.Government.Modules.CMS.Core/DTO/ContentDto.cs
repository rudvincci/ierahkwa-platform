using System;
using System.Collections.Generic;

namespace Mamey.Government.Modules.CMS.Core.DTO;

public class ContentDto
{
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
}
