using System;
using System.Collections.Generic;

namespace NET10.Core.Models.ERP
{
    /// <summary>
    /// Product/Service/Inventory Item
    /// </summary>
    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CompanyId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string Category { get; set; } = string.Empty;
        
        // Pricing
        public decimal CostPrice { get; set; } = 0;
        public decimal SalePrice { get; set; } = 0;
        public decimal WholesalePrice { get; set; } = 0;
        public decimal MinimumPrice { get; set; } = 0;
        public decimal Margin => SalePrice > 0 ? ((SalePrice - CostPrice) / SalePrice) * 100 : 0;
        
        // Tax
        public bool IsTaxable { get; set; } = true;
        public decimal TaxRate { get; set; } = 16.0m;
        public string TaxCode { get; set; } = "IVA";
        
        // Inventory
        public ProductType Type { get; set; } = ProductType.Product;
        public bool TrackInventory { get; set; } = true;
        public decimal StockQuantity { get; set; } = 0;
        public decimal ReorderLevel { get; set; } = 10;
        public decimal ReorderQuantity { get; set; } = 50;
        public string Unit { get; set; } = "pcs";
        
        // Warehouse
        public string WarehouseLocation { get; set; } = string.Empty;
        public string ShelfNumber { get; set; } = string.Empty;
        
        // Status
        public ProductStatus Status { get; set; } = ProductStatus.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastSaleDate { get; set; }
        
        // Image
        public string ImageUrl { get; set; } = string.Empty;
    }
    
    public enum ProductType
    {
        Product,
        Service,
        Kit,
        Raw_Material
    }
    
    public enum ProductStatus
    {
        Active,
        Inactive,
        Discontinued,
        OutOfStock
    }
    
    /// <summary>
    /// Product Category
    /// </summary>
    public class ProductCategory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CompanyId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? ParentCategoryId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
