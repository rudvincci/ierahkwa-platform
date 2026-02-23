namespace DocumentFlow.Core.Models;

public class DocumentVersion
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public int VersionNumber { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? HashSHA256 { get; set; }
    public string? ChangeNotes { get; set; }
    public Guid CreatedBy { get; set; }
    public string? CreatedByName { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public Document? Document { get; set; }
}
