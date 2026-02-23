using System;
using System.Collections.Generic;

namespace NET10.Core.Models.ERP
{
    /// <summary>
    /// Warehouse/Location
    /// </summary>
    public class Warehouse
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CompanyId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Manager { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Inventory Stock Level
    /// </summary>
    public class InventoryStock
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CompanyId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductSKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public decimal QuantityOnHand { get; set; } = 0;
        public decimal QuantityReserved { get; set; } = 0;
        public decimal QuantityAvailable => QuantityOnHand - QuantityReserved;
        public decimal QuantityOnOrder { get; set; } = 0;
        public decimal ReorderPoint { get; set; } = 10;
        public decimal UnitCost { get; set; } = 0;
        public string ShelfLocation { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Inventory Transaction/Movement
    /// </summary>
    public class InventoryTransaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CompanyId { get; set; }
        public string TransactionNumber { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public InventoryTransactionType Type { get; set; }
        
        // Product
        public Guid ProductId { get; set; }
        public string ProductSKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        
        // Warehouse
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public Guid? ToWarehouseId { get; set; } // For transfers
        
        // Quantity
        public decimal Quantity { get; set; } = 0;
        public decimal UnitCost { get; set; } = 0;
        public decimal TotalCost => Quantity * UnitCost;
        
        // Balance
        public decimal BalanceBefore { get; set; } = 0;
        public decimal BalanceAfter { get; set; } = 0;
        
        // Source
        public string SourceType { get; set; } = string.Empty; // Sale, Purchase, Adjustment
        public Guid? SourceId { get; set; }
        public string Reference { get; set; } = string.Empty;
        
        // Notes
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
    }
    
    public enum InventoryTransactionType
    {
        Purchase,
        Sale,
        Return,
        Adjustment_In,
        Adjustment_Out,
        Transfer,
        Opening,
        Damage,
        Expired
    }
    
    /// <summary>
    /// Stock Adjustment
    /// </summary>
    public class StockAdjustment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CompanyId { get; set; }
        public string AdjustmentNumber { get; set; } = string.Empty;
        public DateTime AdjustmentDate { get; set; } = DateTime.UtcNow;
        public Guid WarehouseId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public List<StockAdjustmentItem> Items { get; set; } = new();
        public StockAdjustmentStatus Status { get; set; } = StockAdjustmentStatus.Draft;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ApprovedAt { get; set; }
        public string ApprovedBy { get; set; } = string.Empty;
    }
    
    public class StockAdjustmentItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AdjustmentId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductSKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal CurrentQuantity { get; set; } = 0;
        public decimal NewQuantity { get; set; } = 0;
        public decimal Difference => NewQuantity - CurrentQuantity;
        public decimal UnitCost { get; set; } = 0;
        public string Reason { get; set; } = string.Empty;
    }
    
    public enum StockAdjustmentStatus
    {
        Draft,
        Pending_Approval,
        Approved,
        Rejected,
        Cancelled
    }
}
