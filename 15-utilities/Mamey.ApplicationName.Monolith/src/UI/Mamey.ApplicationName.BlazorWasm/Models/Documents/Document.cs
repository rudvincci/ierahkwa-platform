namespace Mamey.ApplicationName.BlazorWasm.Models.Documents;

public class Document
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FileName { get; set; }
    public string Category { get; set; }
    public DateTime UploadedDate { get; set; }
    public string Description { get; set; }
}