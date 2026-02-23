using Dapper;
using InventoryManager.Models;

namespace InventoryManager.Data
{
    /// <summary>
    /// Repository for product operations
    /// </summary>
    public class ProductRepository
    {
        /// <summary>
        /// Get all products with category and supplier names
        /// </summary>
        public List<Product> GetAll(bool includeInactive = false)
        {
            using var connection = DatabaseManager.GetConnection();
            
            var sql = @"
                SELECT p.*, c.Name as CategoryName, s.Name as SupplierName
                FROM Products p
                LEFT JOIN Categories c ON p.CategoryId = c.Id
                LEFT JOIN Suppliers s ON p.SupplierId = s.Id
                " + (includeInactive ? "" : "WHERE p.IsActive = 1") + @"
                ORDER BY p.Name";
            
            return connection.Query<Product>(sql).ToList();
        }

        /// <summary>
        /// Search products by code, barcode, or name
        /// </summary>
        public List<Product> Search(string searchTerm, bool includeInactive = false)
        {
            using var connection = DatabaseManager.GetConnection();
            
            var sql = @"
                SELECT p.*, c.Name as CategoryName, s.Name as SupplierName
                FROM Products p
                LEFT JOIN Categories c ON p.CategoryId = c.Id
                LEFT JOIN Suppliers s ON p.SupplierId = s.Id
                WHERE (p.Code LIKE @Search 
                    OR p.Barcode LIKE @Search 
                    OR p.Name LIKE @Search
                    OR p.Description LIKE @Search)
                " + (includeInactive ? "" : "AND p.IsActive = 1") + @"
                ORDER BY p.Name
                LIMIT 100";
            
            return connection.Query<Product>(sql, new { Search = $"%{searchTerm}%" }).ToList();
        }

        /// <summary>
        /// Quick search for autocomplete (returns top 10 matches)
        /// </summary>
        public List<Product> QuickSearch(string searchTerm)
        {
            using var connection = DatabaseManager.GetConnection();
            
            return connection.Query<Product>(@"
                SELECT Id, Code, Barcode, Name, CurrentStock, SalePrice, Unit
                FROM Products
                WHERE IsActive = 1 AND (
                    Code LIKE @Search 
                    OR Barcode LIKE @Search 
                    OR Name LIKE @Search)
                ORDER BY 
                    CASE WHEN Code LIKE @ExactSearch THEN 0
                         WHEN Barcode LIKE @ExactSearch THEN 1
                         WHEN Name LIKE @ExactSearch THEN 2
                         ELSE 3 END,
                    Name
                LIMIT 10",
                new { Search = $"%{searchTerm}%", ExactSearch = $"{searchTerm}%" }).ToList();
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        public Product? GetById(int id)
        {
            using var connection = DatabaseManager.GetConnection();
            
            return connection.QueryFirstOrDefault<Product>(@"
                SELECT p.*, c.Name as CategoryName, s.Name as SupplierName
                FROM Products p
                LEFT JOIN Categories c ON p.CategoryId = c.Id
                LEFT JOIN Suppliers s ON p.SupplierId = s.Id
                WHERE p.Id = @Id", new { Id = id });
        }

        /// <summary>
        /// Get product by code
        /// </summary>
        public Product? GetByCode(string code)
        {
            using var connection = DatabaseManager.GetConnection();
            
            return connection.QueryFirstOrDefault<Product>(@"
                SELECT p.*, c.Name as CategoryName, s.Name as SupplierName
                FROM Products p
                LEFT JOIN Categories c ON p.CategoryId = c.Id
                LEFT JOIN Suppliers s ON p.SupplierId = s.Id
                WHERE p.Code = @Code", new { Code = code });
        }

        /// <summary>
        /// Get product by barcode
        /// </summary>
        public Product? GetByBarcode(string barcode)
        {
            using var connection = DatabaseManager.GetConnection();
            
            return connection.QueryFirstOrDefault<Product>(@"
                SELECT p.*, c.Name as CategoryName, s.Name as SupplierName
                FROM Products p
                LEFT JOIN Categories c ON p.CategoryId = c.Id
                LEFT JOIN Suppliers s ON p.SupplierId = s.Id
                WHERE p.Barcode = @Barcode", new { Barcode = barcode });
        }

        /// <summary>
        /// Get products with low stock
        /// </summary>
        public List<Product> GetLowStock()
        {
            using var connection = DatabaseManager.GetConnection();
            
            return connection.Query<Product>(@"
                SELECT p.*, c.Name as CategoryName, s.Name as SupplierName
                FROM Products p
                LEFT JOIN Categories c ON p.CategoryId = c.Id
                LEFT JOIN Suppliers s ON p.SupplierId = s.Id
                WHERE p.IsActive = 1 AND p.CurrentStock <= p.MinimumStock
                ORDER BY (p.CurrentStock - p.MinimumStock) ASC").ToList();
        }

        /// <summary>
        /// Get products by category
        /// </summary>
        public List<Product> GetByCategory(int categoryId)
        {
            using var connection = DatabaseManager.GetConnection();
            
            return connection.Query<Product>(@"
                SELECT p.*, c.Name as CategoryName, s.Name as SupplierName
                FROM Products p
                LEFT JOIN Categories c ON p.CategoryId = c.Id
                LEFT JOIN Suppliers s ON p.SupplierId = s.Id
                WHERE p.CategoryId = @CategoryId AND p.IsActive = 1
                ORDER BY p.Name", new { CategoryId = categoryId }).ToList();
        }

        /// <summary>
        /// Get products by supplier
        /// </summary>
        public List<Product> GetBySupplier(int supplierId)
        {
            using var connection = DatabaseManager.GetConnection();
            
            return connection.Query<Product>(@"
                SELECT p.*, c.Name as CategoryName, s.Name as SupplierName
                FROM Products p
                LEFT JOIN Categories c ON p.CategoryId = c.Id
                LEFT JOIN Suppliers s ON p.SupplierId = s.Id
                WHERE p.SupplierId = @SupplierId AND p.IsActive = 1
                ORDER BY p.Name", new { SupplierId = supplierId }).ToList();
        }

        /// <summary>
        /// Create new product
        /// </summary>
        public int Create(Product product)
        {
            using var connection = DatabaseManager.GetConnection();
            
            return connection.ExecuteScalar<int>(@"
                INSERT INTO Products (
                    Code, Barcode, Name, Description, CategoryId, SupplierId,
                    PurchasePrice, SalePrice, CurrentStock, MinimumStock, MaximumStock,
                    Unit, Location, IsActive, CreatedBy, UpdatedBy, Image, Notes)
                VALUES (
                    @Code, @Barcode, @Name, @Description, @CategoryId, @SupplierId,
                    @PurchasePrice, @SalePrice, @CurrentStock, @MinimumStock, @MaximumStock,
                    @Unit, @Location, @IsActive, @CreatedBy, @UpdatedBy, @Image, @Notes);
                SELECT last_insert_rowid();", product);
        }

        /// <summary>
        /// Update product
        /// </summary>
        public bool Update(Product product)
        {
            using var connection = DatabaseManager.GetConnection();
            
            product.UpdatedAt = DateTime.Now;
            
            var result = connection.Execute(@"
                UPDATE Products SET
                    Code = @Code,
                    Barcode = @Barcode,
                    Name = @Name,
                    Description = @Description,
                    CategoryId = @CategoryId,
                    SupplierId = @SupplierId,
                    PurchasePrice = @PurchasePrice,
                    SalePrice = @SalePrice,
                    MinimumStock = @MinimumStock,
                    MaximumStock = @MaximumStock,
                    Unit = @Unit,
                    Location = @Location,
                    IsActive = @IsActive,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy,
                    Image = @Image,
                    Notes = @Notes
                WHERE Id = @Id", product);
            
            return result > 0;
        }

        /// <summary>
        /// Update stock quantity
        /// </summary>
        public bool UpdateStock(int productId, int newStock, int userId)
        {
            using var connection = DatabaseManager.GetConnection();
            
            var result = connection.Execute(@"
                UPDATE Products SET
                    CurrentStock = @NewStock,
                    UpdatedAt = datetime('now', 'localtime'),
                    UpdatedBy = @UserId
                WHERE Id = @ProductId",
                new { ProductId = productId, NewStock = newStock, UserId = userId });
            
            return result > 0;
        }

        /// <summary>
        /// Delete product (soft delete)
        /// </summary>
        public bool Delete(int id)
        {
            using var connection = DatabaseManager.GetConnection();
            var result = connection.Execute(
                "UPDATE Products SET IsActive = 0 WHERE Id = @Id", new { Id = id });
            return result > 0;
        }

        /// <summary>
        /// Generate next product code
        /// </summary>
        public string GenerateNextCode(string prefix = "PRD")
        {
            using var connection = DatabaseManager.GetConnection();
            
            var lastCode = connection.ExecuteScalar<string?>(@"
                SELECT Code FROM Products 
                WHERE Code LIKE @Prefix
                ORDER BY Code DESC LIMIT 1",
                new { Prefix = $"{prefix}%" });

            if (string.IsNullOrEmpty(lastCode))
                return $"{prefix}0001";

            var numberPart = lastCode.Substring(prefix.Length);
            if (int.TryParse(numberPart, out int number))
                return $"{prefix}{(number + 1):D4}";

            return $"{prefix}0001";
        }

        /// <summary>
        /// Check if code exists
        /// </summary>
        public bool CodeExists(string code, int? excludeId = null)
        {
            using var connection = DatabaseManager.GetConnection();
            
            var sql = "SELECT COUNT(*) FROM Products WHERE Code = @Code";
            if (excludeId.HasValue)
                sql += " AND Id != @ExcludeId";
            
            return connection.ExecuteScalar<int>(sql, 
                new { Code = code, ExcludeId = excludeId ?? 0 }) > 0;
        }

        /// <summary>
        /// Get stock value summary
        /// </summary>
        public (decimal TotalValue, int TotalItems, int LowStockCount) GetStockSummary()
        {
            using var connection = DatabaseManager.GetConnection();
            
            var result = connection.QueryFirst<dynamic>(@"
                SELECT 
                    COALESCE(SUM(CurrentStock * PurchasePrice), 0) as TotalValue,
                    COALESCE(SUM(CurrentStock), 0) as TotalItems,
                    (SELECT COUNT(*) FROM Products WHERE IsActive = 1 AND CurrentStock <= MinimumStock) as LowStockCount
                FROM Products WHERE IsActive = 1");
            
            return ((decimal)result.TotalValue, (int)result.TotalItems, (int)result.LowStockCount);
        }
    }
}
