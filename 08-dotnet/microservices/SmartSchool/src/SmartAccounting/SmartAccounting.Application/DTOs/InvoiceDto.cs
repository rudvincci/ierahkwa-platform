using SmartAccounting.Domain.Entities;

namespace SmartAccounting.Application.DTOs;

public class InvoiceDto
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public InvoiceType Type { get; set; }
    public DateTime InvoiceDate { get; set; }
    public int? StudentId { get; set; }
    public string? StudentName { get; set; }
    public int? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public string? Notes { get; set; }
    public IEnumerable<InvoiceItemDto> Items { get; set; } = new List<InvoiceItemDto>();
    public IEnumerable<InvoicePaymentDto> Payments { get; set; } = new List<InvoicePaymentDto>();
}

public class InvoiceItemDto
{
    public int Id { get; set; }
    public int? ProductId { get; set; }
    public string? ProductName { get; set; }
    public int? FeesId { get; set; }
    public string? FeesName { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
}

public class InvoicePaymentDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
}

public class CreateInvoiceDto
{
    public InvoiceType Type { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public int? StudentId { get; set; }
    public int? SupplierId { get; set; }
    public decimal DiscountPercent { get; set; } = 0;
    public string? Notes { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public decimal? PaidAmount { get; set; }
    public IEnumerable<CreateInvoiceItemDto> Items { get; set; } = new List<CreateInvoiceItemDto>();
}

public class CreateInvoiceItemDto
{
    public int? ProductId { get; set; }
    public int? FeesId { get; set; }
    public string? Description { get; set; }
    public decimal Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; } = 0;
}

public class CreatePaymentDto
{
    public int InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
}

public class CreateReturnInvoiceDto
{
    public int OriginalInvoiceId { get; set; }
    public string? Notes { get; set; }
    public IEnumerable<ReturnItemDto> Items { get; set; } = new List<ReturnItemDto>();
}

public class ReturnItemDto
{
    public int OriginalItemId { get; set; }
    public decimal Quantity { get; set; }
}
