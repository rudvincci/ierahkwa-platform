using System;
using System.Collections.Generic;

namespace NET10.Core.Models.ERP;

/// <summary>
/// Supplier / Vendor for purchases
/// </summary>
public class Supplier
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CompanyId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    
    // Contact
    public string ContactName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    
    // Address
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    
    // Banking
    public string BankName { get; set; } = string.Empty;
    public string BankAccount { get; set; } = string.Empty;
    public string BankRouting { get; set; } = string.Empty;
    public string SwiftCode { get; set; } = string.Empty;
    
    // Terms
    public int PaymentTermDays { get; set; } = 30;
    public decimal CreditLimit { get; set; } = 0;
    public string Currency { get; set; } = "USD";
    public Guid? DefaultExpenseAccountId { get; set; }
    
    // Status
    public bool IsActive { get; set; } = true;
    public string Notes { get; set; } = string.Empty;
    
    // Balance
    public decimal Balance { get; set; } = 0;
    public decimal TotalPurchases { get; set; } = 0;
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Bill / Purchase Invoice from Supplier
/// </summary>
public class Bill
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CompanyId { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public string SupplierBillNumber { get; set; } = string.Empty;
    
    // Supplier
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    
    // Dates
    public DateTime BillDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(30);
    
    // Items
    public List<BillItem> Items { get; set; } = [];
    
    // Totals
    public decimal Subtotal => Items.Sum(i => i.LineTotal);
    public decimal TaxAmount => Items.Sum(i => i.TaxAmount);
    public decimal Total => Subtotal + TaxAmount;
    public decimal AmountPaid { get; set; } = 0;
    public decimal Balance => Total - AmountPaid;
    
    // Status
    public BillStatus Status { get; set; } = BillStatus.Draft;
    public PaymentStatus PaymentStatus => GetPaymentStatus();
    
    // Linked PO
    public Guid? PurchaseOrderId { get; set; }
    
    // Notes
    public string Notes { get; set; } = string.Empty;
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    
    private PaymentStatus GetPaymentStatus()
    {
        if (AmountPaid >= Total) return PaymentStatus.Paid;
        if (AmountPaid > 0) return PaymentStatus.Partial;
        if (DueDate < DateTime.UtcNow) return PaymentStatus.Overdue;
        return PaymentStatus.Unpaid;
    }
}

public class BillItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BillId { get; set; }
    public int LineNumber { get; set; }
    
    // Product
    public Guid? ProductId { get; set; }
    public string Description { get; set; } = string.Empty;
    
    // Account
    public Guid AccountId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    
    // Quantity & Price
    public decimal Quantity { get; set; } = 1;
    public string Unit { get; set; } = "pcs";
    public decimal UnitCost { get; set; } = 0;
    public decimal LineTotal => Quantity * UnitCost;
    
    // Tax
    public bool IsTaxable { get; set; } = true;
    public decimal TaxRate { get; set; } = 16.0m;
    public string TaxCode { get; set; } = "IVA";
    public decimal TaxAmount => IsTaxable ? LineTotal * (TaxRate / 100) : 0;
}

public enum BillStatus
{
    Draft,
    Received,
    Approved,
    Paid,
    Cancelled,
    Voided
}

