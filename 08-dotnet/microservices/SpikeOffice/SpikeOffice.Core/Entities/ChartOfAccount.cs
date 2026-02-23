using SpikeOffice.Core.Enums;

namespace SpikeOffice.Core.Entities;

/// <summary>Double entry accounting - Chart of Accounts.</summary>
public class ChartOfAccount : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public AccountType Type { get; set; }
    public Guid? ParentId { get; set; }
    public ChartOfAccount? Parent { get; set; }
    public decimal OpeningBalance { get; set; }
    public bool IsActive { get; set; } = true;
}
