using Dapper;
using InventoryManager.Models;

namespace InventoryManager.Data
{
    /// <summary>
    /// Repository for stock movement operations
    /// </summary>
    public class StockMovementRepository
    {
        private readonly ProductRepository _productRepository = new();

        /// <summary>
        /// Get all movements
        /// </summary>
        public List<StockMovement> GetAll(DateTime? startDate = null, DateTime? endDate = null)
        {
            using var connection = DatabaseManager.GetConnection();
            
            var sql = @"
                SELECT m.*, p.Name as ProductName, p.Code as ProductCode, u.FullName as UserName
                FROM StockMovements m
                INNER JOIN Products p ON m.ProductId = p.Id
                INNER JOIN Users u ON m.UserId = u.Id
                WHERE 1=1";
            
            if (startDate.HasValue)
                sql += " AND m.MovementDate >= @StartDate";
            if (endDate.HasValue)
                sql += " AND m.MovementDate <= @EndDate";
            
            sql += " ORDER BY m.MovementDate DESC LIMIT 1000";
            
            return connection.Query<StockMovement>(sql, 
                new { StartDate = startDate?.ToString("yyyy-MM-dd"), EndDate = endDate?.ToString("yyyy-MM-dd 23:59:59") }).ToList();
        }

        /// <summary>
        /// Get movements by product
        /// </summary>
        public List<StockMovement> GetByProduct(int productId, int limit = 100)
        {
            using var connection = DatabaseManager.GetConnection();
            
            return connection.Query<StockMovement>(@"
                SELECT m.*, p.Name as ProductName, p.Code as ProductCode, u.FullName as UserName
                FROM StockMovements m
                INNER JOIN Products p ON m.ProductId = p.Id
                INNER JOIN Users u ON m.UserId = u.Id
                WHERE m.ProductId = @ProductId
                ORDER BY m.MovementDate DESC
                LIMIT @Limit", new { ProductId = productId, Limit = limit }).ToList();
        }

        /// <summary>
        /// Get movements by type
        /// </summary>
        public List<StockMovement> GetByType(MovementType type, DateTime? startDate = null, DateTime? endDate = null)
        {
            using var connection = DatabaseManager.GetConnection();
            
            var sql = @"
                SELECT m.*, p.Name as ProductName, p.Code as ProductCode, u.FullName as UserName
                FROM StockMovements m
                INNER JOIN Products p ON m.ProductId = p.Id
                INNER JOIN Users u ON m.UserId = u.Id
                WHERE m.Type = @Type";
            
            if (startDate.HasValue)
                sql += " AND m.MovementDate >= @StartDate";
            if (endDate.HasValue)
                sql += " AND m.MovementDate <= @EndDate";
            
            sql += " ORDER BY m.MovementDate DESC";
            
            return connection.Query<StockMovement>(sql, 
                new { Type = (int)type, StartDate = startDate?.ToString("yyyy-MM-dd"), EndDate = endDate?.ToString("yyyy-MM-dd 23:59:59") }).ToList();
        }

        /// <summary>
        /// Create stock movement and update product stock
        /// </summary>
        public int CreateMovement(StockMovement movement)
        {
            using var connection = DatabaseManager.GetConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Get current product stock
                var product = connection.QueryFirstOrDefault<Product>(
                    "SELECT * FROM Products WHERE Id = @Id", 
                    new { Id = movement.ProductId }, transaction);

                if (product == null)
                    throw new Exception("Product not found");

                movement.PreviousStock = product.CurrentStock;

                // Calculate new stock based on movement type
                switch (movement.Type)
                {
                    case MovementType.Purchase:
                    case MovementType.Return:
                    case MovementType.Initial:
                        movement.NewStock = product.CurrentStock + movement.Quantity;
                        break;
                    case MovementType.Sale:
                    case MovementType.Damage:
                        movement.NewStock = product.CurrentStock - movement.Quantity;
                        break;
                    case MovementType.Adjustment:
                        movement.NewStock = movement.Quantity; // Direct set
                        movement.Quantity = movement.Quantity - product.CurrentStock;
                        break;
                    case MovementType.Transfer:
                        movement.NewStock = product.CurrentStock - movement.Quantity;
                        break;
                }

                movement.TotalValue = movement.Quantity * movement.UnitPrice;

                // Insert movement
                var movementId = connection.ExecuteScalar<int>(@"
                    INSERT INTO StockMovements (
                        ProductId, Type, Quantity, PreviousStock, NewStock,
                        UnitPrice, TotalValue, Reference, Notes, UserId, DocumentNumber)
                    VALUES (
                        @ProductId, @Type, @Quantity, @PreviousStock, @NewStock,
                        @UnitPrice, @TotalValue, @Reference, @Notes, @UserId, @DocumentNumber);
                    SELECT last_insert_rowid();", movement, transaction);

                // Update product stock
                connection.Execute(@"
                    UPDATE Products SET 
                        CurrentStock = @NewStock,
                        UpdatedAt = datetime('now', 'localtime'),
                        UpdatedBy = @UserId
                    WHERE Id = @ProductId",
                    new { ProductId = movement.ProductId, NewStock = movement.NewStock, UserId = movement.UserId },
                    transaction);

                transaction.Commit();
                return movementId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Batch stock entry for quick stock updates
        /// </summary>
        public void BatchStockEntry(List<(int ProductId, int Quantity, decimal UnitPrice)> items, 
            MovementType type, int userId, string reference)
        {
            foreach (var item in items)
            {
                CreateMovement(new StockMovement
                {
                    ProductId = item.ProductId,
                    Type = type,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    UserId = userId,
                    Reference = reference
                });
            }
        }

        /// <summary>
        /// Generate document number
        /// </summary>
        public string GenerateDocumentNumber(MovementType type)
        {
            using var connection = DatabaseManager.GetConnection();
            
            var prefix = type switch
            {
                MovementType.Purchase => "PUR",
                MovementType.Sale => "SAL",
                MovementType.Return => "RET",
                MovementType.Adjustment => "ADJ",
                MovementType.Transfer => "TRF",
                MovementType.Damage => "DMG",
                MovementType.Initial => "INI",
                _ => "MOV"
            };

            var date = DateTime.Now.ToString("yyyyMMdd");
            
            var lastNumber = connection.ExecuteScalar<string?>(@"
                SELECT DocumentNumber FROM StockMovements 
                WHERE DocumentNumber LIKE @Prefix
                ORDER BY Id DESC LIMIT 1",
                new { Prefix = $"{prefix}{date}%" });

            if (string.IsNullOrEmpty(lastNumber))
                return $"{prefix}{date}001";

            var numberPart = lastNumber.Substring(prefix.Length + 8);
            if (int.TryParse(numberPart, out int number))
                return $"{prefix}{date}{(number + 1):D3}";

            return $"{prefix}{date}001";
        }

        /// <summary>
        /// Get movement statistics
        /// </summary>
        public (decimal TotalPurchases, decimal TotalSales, int TotalMovements) GetStatistics(
            DateTime startDate, DateTime endDate)
        {
            using var connection = DatabaseManager.GetConnection();
            
            var result = connection.QueryFirst<dynamic>(@"
                SELECT 
                    COALESCE(SUM(CASE WHEN Type = 1 THEN TotalValue ELSE 0 END), 0) as TotalPurchases,
                    COALESCE(SUM(CASE WHEN Type = 2 THEN TotalValue ELSE 0 END), 0) as TotalSales,
                    COUNT(*) as TotalMovements
                FROM StockMovements
                WHERE MovementDate BETWEEN @StartDate AND @EndDate",
                new { StartDate = startDate.ToString("yyyy-MM-dd"), EndDate = endDate.ToString("yyyy-MM-dd 23:59:59") });
            
            return ((decimal)result.TotalPurchases, (decimal)result.TotalSales, (int)result.TotalMovements);
        }
    }
}
