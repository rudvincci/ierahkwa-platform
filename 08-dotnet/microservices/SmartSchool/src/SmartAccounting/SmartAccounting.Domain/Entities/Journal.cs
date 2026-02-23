using Common.Domain.Entities;

namespace SmartAccounting.Domain.Entities;

public class Journal : TenantEntity
{
    public string JournalNumber { get; set; } = string.Empty;
    public DateTime JournalDate { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }
    public string? Reference { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public JournalStatus Status { get; set; } = JournalStatus.Draft;
    public int? CostCenterId { get; set; }
    public bool IsPosted { get; set; } = false;
    public DateTime? PostedAt { get; set; }
    public int? PostedBy { get; set; }
    
    public virtual CostCenter? CostCenter { get; set; }
    public virtual ICollection<JournalEntry> Entries { get; set; } = new List<JournalEntry>();
}

public enum JournalStatus
{
    Draft,
    Pending,
    Approved,
    Posted,
    Cancelled
}

public class JournalEntry : TenantEntity
{
    public int JournalId { get; set; }
    public int AccountId { get; set; }
    public string? Description { get; set; }
    public decimal DebitAmount { get; set; } = 0;
    public decimal CreditAmount { get; set; } = 0;
    public int? CostCenterId { get; set; }
    
    public virtual Journal? Journal { get; set; }
    public virtual Account? Account { get; set; }
    public virtual CostCenter? CostCenter { get; set; }
}
