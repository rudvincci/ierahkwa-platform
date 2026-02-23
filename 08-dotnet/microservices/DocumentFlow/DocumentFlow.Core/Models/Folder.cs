namespace DocumentFlow.Core.Models;

public class Folder
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentFolderId { get; set; }
    public string Path { get; set; } = string.Empty;
    public string? Department { get; set; }
    public Guid OwnerId { get; set; }
    public string? OwnerName { get; set; }
    public bool IsShared { get; set; }
    public SecurityLevel SecurityLevel { get; set; }
    public string? Color { get; set; }
    public string? Icon { get; set; }
    public int DocumentCount { get; set; }
    public long TotalSize { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Folder? ParentFolder { get; set; }
    public List<Folder> SubFolders { get; set; } = new();
    public List<Document> Documents { get; set; } = new();
}
