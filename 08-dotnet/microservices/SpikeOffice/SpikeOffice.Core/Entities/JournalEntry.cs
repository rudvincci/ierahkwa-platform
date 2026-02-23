namespace SpikeOffice.Core.Entities;

public class JournalEntry : BaseEntity
{
    public string Number { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? Reference { get; set; }
    public string? Description { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public bool IsPosted { get; set; }
    public DateTime? PostedAt { get; set; }

    public ICollection<JournalEntryLine> Lines { get; set; } = new List<JournalEntryLine>();
}
