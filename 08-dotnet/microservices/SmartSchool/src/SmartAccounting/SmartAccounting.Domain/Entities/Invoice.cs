using Common.Domain.Entities;

namespace SmartAccounting.Domain.Entities;

public class Invoice : TenantEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public InvoiceType Type { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public int? StudentId { get; set; }
    public int? SupplierId { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount => TotalAmount - PaidAmount;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public PaymentMethod? PaymentMethod { get; set; }
    public string? Notes { get; set; }
    public int? OriginalInvoiceId { get; set; } // For returns
    public bool IsActive { get; set; } = true;
    
    public virtual Invoice? OriginalInvoice { get; set; }
    public virtual Supplier? Supplier { get; set; }
    public virtual ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    public virtual ICollection<InvoicePayment> Payments { get; set; } = new List<InvoicePayment>();
}

public enum InvoiceType
{
    FeesInvoice,
    FeesReturn,
    PurchaseInvoice,
    PurchaseReturn
}

public enum PaymentStatus
{
    Pending,
    PartiallyPaid,
    Paid,
    Cancelled
}

public enum PaymentMethod
{
    Cash,
    Card,
    BankTransfer,
    Check,
    Online
}
