using System;
using System.Collections.Generic;

namespace Mamey.Government.Modules.Documents.Core.DTO;

public class DocumentDto
{
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
    public string Status => IsActive ? "Active" : "Deleted";
}
