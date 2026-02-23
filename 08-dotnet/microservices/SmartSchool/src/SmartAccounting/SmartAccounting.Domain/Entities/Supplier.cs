using Common.Domain.Entities;

namespace SmartAccounting.Domain.Entities;

public class Supplier : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? TaxNumber { get; set; }
    public decimal Balance { get; set; } = 0;
    public bool IsDefault { get; set; } = false;
    public bool IsActive { get; set; } = true;
    
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
