using Common.Domain.Entities;

namespace SmartAccounting.Domain.Entities;

public class Account : TenantEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public AccountType Type { get; set; }
    public int? ParentId { get; set; }
    public int Level { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public bool IsSystemAccount { get; set; } = false;
    public decimal OpeningBalance { get; set; } = 0;
    public decimal CurrentBalance { get; set; } = 0;
    
    public virtual Account? Parent { get; set; }
    public virtual ICollection<Account> Children { get; set; } = new List<Account>();
    public virtual ICollection<JournalEntry> DebitEntries { get; set; } = new List<JournalEntry>();
    public virtual ICollection<JournalEntry> CreditEntries { get; set; } = new List<JournalEntry>();
}

public enum AccountType
{
    Asset,
    Liability,
    Equity,
    Revenue,
    Expense
}
