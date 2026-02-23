using Common.Domain.Entities;

namespace SmartAccounting.Domain.Entities;

public class CostCenter : TenantEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentId { get; set; }
    public bool IsActive { get; set; } = true;
    
    public virtual CostCenter? Parent { get; set; }
    public virtual ICollection<CostCenter> Children { get; set; } = new List<CostCenter>();
    public virtual ICollection<Journal> Journals { get; set; } = new List<Journal>();
    public virtual ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();
}
