using Common.Domain.Entities;

namespace SmartAccounting.Domain.Entities;

public class SchoolYear : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsCurrent { get; set; } = false;
    public bool IsActive { get; set; } = true;
    
    public virtual ICollection<Fees> Fees { get; set; } = new List<Fees>();
}
