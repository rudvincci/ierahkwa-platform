namespace NET10.Core.Interfaces;

// ═══════════════════════════════════════════════════════════════════════════════
// IERAHKWA STOCK MANAGEMENT SYSTEM
// Complete Inventory Management & Point of Sale
// ═══════════════════════════════════════════════════════════════════════════════

public interface IInventoryProductService
{
    Task<List<InventoryProduct>> GetAllAsync();
    Task<InventoryProduct?> GetByIdAsync(Guid id);
    Task<InventoryProduct?> GetBySKUAsync(string sku);
    Task<List<InventoryProduct>> GetByCategoryAsync(Guid categoryId);
    Task<List<InventoryProduct>> SearchAsync(string searchTerm);
    Task<InventoryProduct> CreateAsync(InventoryProduct product);
    Task<InventoryProduct> UpdateAsync(InventoryProduct product);
    Task<bool> DeleteAsync(Guid id);
    Task<decimal> GetStockLevelAsync(Guid productId);
    Task<List<InventoryProduct>> GetLowStockAsync(int threshold = 10);
}

public interface ICategoryService
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(Guid id);
    Task<Category> CreateAsync(Category category);
    Task<Category> UpdateAsync(Category category);
    Task<bool> DeleteAsync(Guid id);
}

public interface IUnitOfMeasureService
{
    Task<List<UnitOfMeasure>> GetAllAsync();
    Task<UnitOfMeasure?> GetByIdAsync(Guid id);
    Task<UnitOfMeasure?> GetByCodeAsync(string code);
    Task<UnitOfMeasure> CreateAsync(UnitOfMeasure uom);
    Task<UnitOfMeasure> UpdateAsync(UnitOfMeasure uom);
    Task<bool> DeleteAsync(Guid id);
}

public interface IStockTransactionService
{
    Task<StockTransaction> AddStockAsync(AddStockRequest request);
    Task<StockTransaction> ReturnStockAsync(ReturnStockRequest request);
    Task<StockTransaction> AdjustStockAsync(AdjustStockRequest request);
    Task<StockTransaction?> GetTransactionAsync(Guid id);
    Task<List<StockTransaction>> GetProductTransactionsAsync(Guid productId);
    Task<List<StockTransaction>> GetTransactionsByDateAsync(DateTime from, DateTime to);
    Task<List<StockTransaction>> GetAllTransactionsAsync();
    Task<decimal> GetCurrentStockAsync(Guid productId);
}

public interface IInventoryCustomerService
{
    Task<List<InventoryCustomer>> GetAllAsync();
    Task<InventoryCustomer?> GetByIdAsync(Guid id);
    Task<InventoryCustomer?> GetByCodeAsync(string customerCode);
    Task<List<InventoryCustomer>> SearchAsync(string searchTerm);
    Task<InventoryCustomer> CreateAsync(InventoryCustomer customer);
    Task<InventoryCustomer> UpdateAsync(InventoryCustomer customer);
    Task<bool> DeleteAsync(Guid id);
}

public interface IPointOfSaleService
{
    Task<Sale> CreateSaleAsync(CreateSaleRequest request);
    Task<Sale?> GetSaleAsync(Guid id);
    Task<List<Sale>> GetSalesByDateAsync(DateTime date);
    Task<List<Sale>> GetSalesByDateRangeAsync(DateTime from, DateTime to);
    Task<Sale> ProcessPaymentAsync(Guid saleId, InventoryPaymentRequest payment);
    Task<Sale> CancelSaleAsync(Guid saleId, string reason);
    Task<List<Sale>> GetTodaySalesAsync();
}

public interface IInventoryReportService
{
    Task<InventoryReport> GetInventoryReportAsync();
    Task<DailyInventoryReport> GetDailyReportAsync(DateTime date);
    Task<MonthlyInventoryReport> GetMonthlyReportAsync(int year, int month);
    Task<AllStockReport> GetAllStockReportAsync();
    Task<StockMovementReport> GetStockMovementReportAsync(DateTime from, DateTime to);
    Task<SalesReport> GetSalesReportAsync(DateTime from, DateTime to);
    Task<LowStockReport> GetLowStockReportAsync(int threshold = 10);
}

public interface IUserService
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> AuthenticateAsync(string username, string password);
}

// ═══════════════════════════════════════════════════════════════════════════════
// INVENTORY MODELS
// ═══════════════════════════════════════════════════════════════════════════════

public class InventoryProduct
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public Guid UnitOfMeasureId { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal CurrentStock { get; set; }
    public decimal MinimumStock { get; set; } = 10;
    public string? Barcode { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class UnitOfMeasure
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class StockTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TransactionNumber { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public InventoryTransactionType Type { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
    public Guid? CreatedById { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public decimal StockAfter { get; set; }
}

public enum InventoryTransactionType
{
    StockIn,
    StockOut,
    StockReturn,
    StockAdjustment,
    Sale,
    Purchase
}

public class AddStockRequest
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
}

public class ReturnStockRequest
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Reason { get; set; }
}

public class AdjustStockRequest
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class InventoryCustomer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string CustomerCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Sale
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string SaleNumber { get; set; } = string.Empty;
    public Guid? CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public List<SaleItem> Items { get; set; } = new();
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal Change => AmountPaid - TotalAmount;
    public InventoryPaymentMethod PaymentMethod { get; set; }
    public SaleStatus Status { get; set; } = SaleStatus.Completed;
    public Guid? CashierId { get; set; }
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;
}

public class SaleItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

public enum InventoryPaymentMethod
{
    Cash,
    CreditCard,
    MobilePayment,
    Crypto
}

public enum SaleStatus
{
    Completed,
    Cancelled,
    Refunded
}

public class CreateSaleRequest
{
    public Guid? CustomerId { get; set; }
    public List<SaleItemRequest> Items { get; set; } = new();
    public decimal? DiscountAmount { get; set; }
    public decimal TaxRate { get; set; } = 0;
    public InventoryPaymentMethod PaymentMethod { get; set; } = InventoryPaymentMethod.Cash;
    public decimal AmountPaid { get; set; }
    public Guid? CashierId { get; set; }
}

public class SaleItemRequest
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
}

public class InventoryPaymentRequest
{
    public InventoryPaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
}

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public UserRole Role { get; set; } = UserRole.Cashier;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum UserRole
{
    Admin,
    Manager,
    Cashier,
    StockKeeper
}

// Report Models
public class InventoryReport
{
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public int TotalProducts { get; set; }
    public int LowStockProducts { get; set; }
    public decimal TotalStockValue { get; set; }
}

public class DailyInventoryReport
{
    public DateTime Date { get; set; }
    public int StockInTransactions { get; set; }
    public int StockOutTransactions { get; set; }
    public decimal TotalStockIn { get; set; }
    public decimal TotalStockOut { get; set; }
    public List<StockTransaction> Transactions { get; set; } = new();
}

public class MonthlyInventoryReport
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int TotalTransactions { get; set; }
    public decimal TotalStockIn { get; set; }
    public decimal TotalStockOut { get; set; }
}

public class AllStockReport
{
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public List<StockItem> Items { get; set; } = new();
    public decimal TotalStockValue { get; set; }
}

public class StockItem
{
    public Guid ProductId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public decimal UnitCost { get; set; }
    public decimal StockValue { get; set; }
}

public class StockMovementReport
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public List<StockTransaction> Transactions { get; set; } = new();
}

public class SalesReport
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalSales { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalProfit { get; set; }
}

public class LowStockReport
{
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public int Threshold { get; set; }
    public List<InventoryProduct> Products { get; set; } = new();
}
