using Common.Domain.Entities;

namespace SmartAccounting.Domain.Entities;

public class Unit : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public bool IsActive { get; set; } = true;
    
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
