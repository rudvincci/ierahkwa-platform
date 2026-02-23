using Common.Domain.Entities;

namespace SmartAccounting.Domain.Entities;

public class InvoiceItem : TenantEntity
{
    public int InvoiceId { get; set; }
    public int? ProductId { get; set; }
    public int? FeesId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; } = 0;
    public decimal DiscountAmount { get; set; } = 0;
    public decimal TaxPercent { get; set; } = 0;
    public decimal TaxAmount { get; set; } = 0;
    public decimal TotalAmount { get; set; }
    
    public virtual Invoice? Invoice { get; set; }
    public virtual Product? Product { get; set; }
    public virtual Fees? Fees { get; set; }
}

public class InvoicePayment : TenantEntity
{
    public int InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public string? Reference { get; set; }
    public string? Notes { get; set; }
    
    public virtual Invoice? Invoice { get; set; }
}
