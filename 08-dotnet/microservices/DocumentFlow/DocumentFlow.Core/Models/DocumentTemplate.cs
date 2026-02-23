namespace DocumentFlow.Core.Models;

public class DocumentTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DocumentType DocumentType { get; set; }
    public string? Category { get; set; }
    public string? Department { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string? TemplateFields { get; set; } // JSON definition of merge fields
    public Guid? DefaultWorkflowId { get; set; }
    public bool IsActive { get; set; } = true;
    public int UsageCount { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
