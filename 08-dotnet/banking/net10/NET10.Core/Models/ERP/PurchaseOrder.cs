using System;
using System.Collections.Generic;
using System.Linq;

namespace NET10.Core.Models.ERP
{
    /// <summary>
    /// Purchase Order
    /// </summary>
    public class PurchaseOrder
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CompanyId { get; set; }
        public string PONumber { get; set; } = string.Empty;
        
        // Supplier
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        
        // Dates
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpectedDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        
        // Warehouse
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        
        // Items
        public List<PurchaseOrderItem> Items { get; set; } = new();
        
        // Totals
        public decimal Subtotal => Items.Sum(i => i.LineTotal);
        public decimal TaxAmount => Items.Sum(i => i.TaxAmount);
        public decimal Total => Subtotal + TaxAmount;
        
        // Status
        public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
        public string Notes { get; set; } = string.Empty;
        
        // Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
    }
    
    public class PurchaseOrderItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PurchaseOrderId { get; set; }
        public int LineNumber { get; set; }
        
        // Product
        public Guid ProductId { get; set; }
        public string ProductSKU { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        // Quantity
        public decimal QuantityOrdered { get; set; } = 0;
        public decimal QuantityReceived { get; set; } = 0;
        public decimal QuantityPending => QuantityOrdered - QuantityReceived;
        public string Unit { get; set; } = "pcs";
        
        // Pricing
        public decimal UnitCost { get; set; } = 0;
        public decimal LineTotal => QuantityOrdered * UnitCost;
        
        // Tax
        public decimal TaxRate { get; set; } = 16.0m;
        public decimal TaxAmount => LineTotal * (TaxRate / 100);
    }
    
    public enum PurchaseOrderStatus
    {
        Draft,
        Sent,
        Confirmed,
        Partial_Received,
        Received,
        Cancelled
    }
    
    /// <summary>
    /// Purchase Bill/Invoice from Supplier
    /// </summary>
    public class PurchaseBill
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CompanyId { get; set; }
        public string BillNumber { get; set; } = string.Empty;
        public string SupplierInvoiceNumber { get; set; } = string.Empty;
        
        // Supplier
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        
        // Dates
        public DateTime BillDate { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(30);
        
        // Reference
        public Guid? PurchaseOrderId { get; set; }
        
        // Items
        public List<PurchaseBillItem> Items { get; set; } = new();
        
        // Totals
        public decimal Subtotal => Items.Sum(i => i.LineTotal);
        public decimal TaxAmount => Items.Sum(i => i.TaxAmount);
        public decimal Total => Subtotal + TaxAmount;
        public decimal AmountPaid { get; set; } = 0;
        public decimal Balance => Total - AmountPaid;
        
        // Status
        public BillStatus Status { get; set; } = BillStatus.Draft;
        public string Notes { get; set; } = string.Empty;
        
        // Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
    }
    
    public class PurchaseBillItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid BillId { get; set; }
        public Guid? ProductId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Quantity { get; set; } = 0;
        public decimal UnitCost { get; set; } = 0;
        public decimal LineTotal => Quantity * UnitCost;
        public decimal TaxRate { get; set; } = 16.0m;
        public decimal TaxAmount => LineTotal * (TaxRate / 100);
    }
}
