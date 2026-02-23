using System;

namespace Mamey.Government.Modules.Documents.Core.DTO;

public class DocumentSummaryDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? Category { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}
