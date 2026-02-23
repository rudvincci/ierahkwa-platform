namespace InventoryManager.Models
{
    /// <summary>
    /// Product model for inventory management
    /// </summary>
    public class Product
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public int CurrentStock { get; set; }
        public int MinimumStock { get; set; }
        public int MaximumStock { get; set; }
        public string Unit { get; set; } = "PCS";
        public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public byte[]? Image { get; set; }
        public string? Notes { get; set; }

        // Calculated properties
        public decimal StockValue => CurrentStock * PurchasePrice;
        public decimal ProfitMargin => SalePrice > 0 ? ((SalePrice - PurchasePrice) / SalePrice) * 100 : 0;
        public bool IsLowStock => CurrentStock <= MinimumStock;
        public bool IsOverStock => CurrentStock >= MaximumStock;
        public string StockStatus => IsLowStock ? "Low" : (IsOverStock ? "Over" : "Normal");
    }
}
