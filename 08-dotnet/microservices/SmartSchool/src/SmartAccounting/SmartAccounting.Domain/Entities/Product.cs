using Common.Domain.Entities;

namespace SmartAccounting.Domain.Entities;

public class Product : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Barcode { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public int UnitId { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal Quantity { get; set; }
    public decimal MinQuantity { get; set; } = 0;
    public string? Image { get; set; }
    public bool IsActive { get; set; } = true;
    
    public virtual Category? Category { get; set; }
    public virtual Unit? Unit { get; set; }
    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
}
