namespace Mamey.ApplicationName.BlazorWasm.Models.Statements;

public class Statement
{
    public Guid Id { get; set; }
    public string AccountName { get; set; }
    public DateTime Date { get; set; }
    public string Type { get; set; } // e.g., Monthly, Yearly, Custom
    public string Format { get; set; } // e.g., PDF, CSV
    public decimal Amount { get; set; }
}
public class Document
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string Category { get; set; }
    public DateTime UploadedDate { get; set; }
    public string Description { get; set; }
}