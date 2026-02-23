using System;
using System.Collections.Generic;
using System.Linq;

namespace NET10.Core.Models.ERP
{
    /// <summary>
    /// Sales Invoice / Bill
    /// </summary>
    public class Invoice
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CompanyId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        
        // Customer
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerTaxId { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        
        // Dates
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(30);
        
        // Items
        public List<InvoiceItem> Items { get; set; } = new();
        
        // Totals
        public decimal Subtotal => Items.Sum(i => i.LineTotal);
        public decimal DiscountAmount { get; set; } = 0;
        public decimal DiscountPercent { get; set; } = 0;
        public decimal TaxableAmount => Subtotal - DiscountAmount;
        public decimal TaxAmount => Items.Sum(i => i.TaxAmount);
        public decimal Total => TaxableAmount + TaxAmount;
        public decimal AmountPaid { get; set; } = 0;
        public decimal Balance => Total - AmountPaid;
        
        // Tax Details
        public List<TaxDetail> TaxDetails { get; set; } = new();
        
        // Status
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
        public PaymentStatus PaymentStatus => GetPaymentStatus();
        
        // Notes
        public string Notes { get; set; } = string.Empty;
        public string Terms { get; set; } = string.Empty;
        public string InternalNotes { get; set; } = string.Empty;
        
        // Metadata
        public string Reference { get; set; } = string.Empty;
        public string PurchaseOrder { get; set; } = string.Empty;
        public string SalesRep { get; set; } = string.Empty;
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SentAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        
        private PaymentStatus GetPaymentStatus()
        {
            if (AmountPaid >= Total) return PaymentStatus.Paid;
            if (AmountPaid > 0) return PaymentStatus.Partial;
            if (DueDate < DateTime.UtcNow) return PaymentStatus.Overdue;
            return PaymentStatus.Unpaid;
        }
    }
    
    /// <summary>
    /// Invoice Line Item
    /// </summary>
    public class InvoiceItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid InvoiceId { get; set; }
        public int LineNumber { get; set; }
        
        // Product
        public Guid? ProductId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        // Quantity & Price
        public decimal Quantity { get; set; } = 1;
        public string Unit { get; set; } = "pcs";
        public decimal UnitPrice { get; set; } = 0;
        public decimal DiscountPercent { get; set; } = 0;
        public decimal DiscountAmount => (UnitPrice * Quantity) * (DiscountPercent / 100);
        public decimal LineTotal => (UnitPrice * Quantity) - DiscountAmount;
        
        // Tax
        public bool IsTaxable { get; set; } = true;
        public decimal TaxRate { get; set; } = 16.0m;
        public string TaxCode { get; set; } = "IVA";
        public decimal TaxAmount => IsTaxable ? LineTotal * (TaxRate / 100) : 0;
        public decimal TotalWithTax => LineTotal + TaxAmount;
    }
    
    /// <summary>
    /// Tax Breakdown Detail
    /// </summary>
    public class TaxDetail
    {
        public string TaxCode { get; set; } = string.Empty;
        public string TaxName { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public decimal TaxableAmount { get; set; }
        public decimal TaxAmount { get; set; }
    }
    
    public enum InvoiceStatus
    {
        Draft,
        Sent,
        Viewed,
        Approved,
        Cancelled,
        Voided
    }
    
    public enum PaymentStatus
    {
        Unpaid,
        Partial,
        Paid,
        Overdue,
        Refunded
    }
}
