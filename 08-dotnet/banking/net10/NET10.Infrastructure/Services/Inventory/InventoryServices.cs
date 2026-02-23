using NET10.Core.Interfaces;

namespace NET10.Infrastructure.Services.Inventory;

// ═══════════════════════════════════════════════════════════════════════════════
// IERAHKWA STOCK MANAGEMENT SYSTEM
// Complete Inventory Management & Point of Sale
// ═══════════════════════════════════════════════════════════════════════════════

public class InventoryProductService : IInventoryProductService
{
    private static readonly List<InventoryProduct> _products = InitializeDemoProducts();
    private static int _skuCounter = 1001;
    
    private static List<InventoryProduct> InitializeDemoProducts()
    {
        return new List<InventoryProduct>
        {
            new() { SKU = "PROD-1001", Name = "Laptop Computer", Description = "High-performance laptop", CostPrice = 450, SellingPrice = 650, CurrentStock = 25, MinimumStock = 10, CategoryName = "Electronics", UnitOfMeasure = "Unit" },
            new() { SKU = "PROD-1002", Name = "Wireless Mouse", Description = "Ergonomic wireless mouse", CostPrice = 15, SellingPrice = 25, CurrentStock = 150, MinimumStock = 50, CategoryName = "Electronics", UnitOfMeasure = "Unit" },
            new() { SKU = "PROD-1003", Name = "Office Chair", Description = "Ergonomic office chair", CostPrice = 120, SellingPrice = 180, CurrentStock = 8, MinimumStock = 5, CategoryName = "Furniture", UnitOfMeasure = "Unit" },
            new() { SKU = "PROD-1004", Name = "Printer Paper", Description = "A4 printer paper", CostPrice = 8, SellingPrice = 12, CurrentStock = 500, MinimumStock = 100, CategoryName = "Office Supplies", UnitOfMeasure = "Pack" },
            new() { SKU = "PROD-1005", Name = "Ink Cartridge", Description = "Printer ink cartridge", CostPrice = 25, SellingPrice = 40, CurrentStock = 45, MinimumStock = 20, CategoryName = "Office Supplies", UnitOfMeasure = "Unit" }
        };
    }
    
    public Task<List<InventoryProduct>> GetAllAsync() => Task.FromResult(_products.Where(p => p.IsActive).ToList());
    
    public Task<InventoryProduct?> GetByIdAsync(Guid id) => Task.FromResult(_products.FirstOrDefault(p => p.Id == id));
    
    public Task<InventoryProduct?> GetBySKUAsync(string sku) => 
        Task.FromResult(_products.FirstOrDefault(p => p.SKU.Equals(sku, StringComparison.OrdinalIgnoreCase)));
    
    public Task<List<InventoryProduct>> GetByCategoryAsync(Guid categoryId) => 
        Task.FromResult(_products.Where(p => p.CategoryId == categoryId && p.IsActive).ToList());
    
    public Task<List<InventoryProduct>> SearchAsync(string searchTerm) =>
        Task.FromResult(_products.Where(p => 
            (p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
             p.SKU.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
             (p.Description != null && p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))) &&
            p.IsActive).ToList());
    
    public Task<InventoryProduct> CreateAsync(InventoryProduct product)
    {
        product.Id = Guid.NewGuid();
        product.SKU = $"PROD-{_skuCounter++:D4}";
        product.CreatedAt = DateTime.UtcNow;
        _products.Add(product);
        return Task.FromResult(product);
    }
    
    public Task<InventoryProduct> UpdateAsync(InventoryProduct product)
    {
        var index = _products.FindIndex(p => p.Id == product.Id);
        if (index >= 0)
        {
            _products[index] = product;
        }
        return Task.FromResult(product);
    }
    
    public Task<bool> DeleteAsync(Guid id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product != null) { product.IsActive = false; return Task.FromResult(true); }
        return Task.FromResult(false);
    }
    
    public Task<decimal> GetStockLevelAsync(Guid productId)
    {
        var product = _products.FirstOrDefault(p => p.Id == productId);
        return Task.FromResult(product?.CurrentStock ?? 0);
    }
    
    public Task<List<InventoryProduct>> GetLowStockAsync(int threshold = 10) =>
        Task.FromResult(_products.Where(p => p.IsActive && p.CurrentStock <= threshold).ToList());
}

public class CategoryService : ICategoryService
{
    private static readonly List<Category> _categories = InitializeDemoCategories();
    private static int _codeCounter = 1;
    
    private static List<Category> InitializeDemoCategories()
    {
        return new List<Category>
        {
            new() { Code = "CAT-001", Name = "Electronics", Description = "Electronic devices and accessories" },
            new() { Code = "CAT-002", Name = "Furniture", Description = "Office and home furniture" },
            new() { Code = "CAT-003", Name = "Office Supplies", Description = "Office consumables and supplies" },
            new() { Code = "CAT-004", Name = "Software", Description = "Software licenses and products" },
            new() { Code = "CAT-005", Name = "Hardware", Description = "Computer hardware components" }
        };
    }
    
    public Task<List<Category>> GetAllAsync() => Task.FromResult(_categories.Where(c => c.IsActive).ToList());
    
    public Task<Category?> GetByIdAsync(Guid id) => Task.FromResult(_categories.FirstOrDefault(c => c.Id == id));
    
    public Task<Category> CreateAsync(Category category)
    {
        category.Id = Guid.NewGuid();
        category.Code = $"CAT-{_codeCounter++:D3}";
        category.CreatedAt = DateTime.UtcNow;
        _categories.Add(category);
        return Task.FromResult(category);
    }
    
    public Task<Category> UpdateAsync(Category category)
    {
        var index = _categories.FindIndex(c => c.Id == category.Id);
        if (index >= 0) _categories[index] = category;
        return Task.FromResult(category);
    }
    
    public Task<bool> DeleteAsync(Guid id)
    {
        var category = _categories.FirstOrDefault(c => c.Id == id);
        if (category != null) { category.IsActive = false; return Task.FromResult(true); }
        return Task.FromResult(false);
    }
}

public class UnitOfMeasureService : IUnitOfMeasureService
{
    private static readonly List<UnitOfMeasure> _uoms = InitializeDemoUOMs();
    
    private static List<UnitOfMeasure> InitializeDemoUOMs()
    {
        return new List<UnitOfMeasure>
        {
            new() { Code = "PCS", Name = "Pieces" },
            new() { Code = "PKG", Name = "Package" },
            new() { Code = "BOX", Name = "Box" },
            new() { Code = "KG", Name = "Kilogram" },
            new() { Code = "L", Name = "Liter" },
            new() { Code = "M", Name = "Meter" }
        };
    }
    
    public Task<List<UnitOfMeasure>> GetAllAsync() => Task.FromResult(_uoms.Where(u => u.IsActive).ToList());
    
    public Task<UnitOfMeasure?> GetByIdAsync(Guid id) => Task.FromResult(_uoms.FirstOrDefault(u => u.Id == id));
    
    public Task<UnitOfMeasure?> GetByCodeAsync(string code) =>
        Task.FromResult(_uoms.FirstOrDefault(u => u.Code.Equals(code, StringComparison.OrdinalIgnoreCase)));
    
    public Task<UnitOfMeasure> CreateAsync(UnitOfMeasure uom)
    {
        uom.Id = Guid.NewGuid();
        uom.CreatedAt = DateTime.UtcNow;
        _uoms.Add(uom);
        return Task.FromResult(uom);
    }
    
    public Task<UnitOfMeasure> UpdateAsync(UnitOfMeasure uom)
    {
        var index = _uoms.FindIndex(u => u.Id == uom.Id);
        if (index >= 0) _uoms[index] = uom;
        return Task.FromResult(uom);
    }
    
    public Task<bool> DeleteAsync(Guid id)
    {
        var uom = _uoms.FirstOrDefault(u => u.Id == id);
        if (uom != null) { uom.IsActive = false; return Task.FromResult(true); }
        return Task.FromResult(false);
    }
}

public class StockTransactionService : IStockTransactionService
{
    private static readonly List<StockTransaction> _transactions = new();
    private static int _transactionCounter = 1001;
    private readonly IInventoryProductService _productService;
    
    public StockTransactionService(IInventoryProductService productService)
    {
        _productService = productService;
    }
    
    public async Task<StockTransaction> AddStockAsync(AddStockRequest request)
    {
        InventoryProduct? product = await _productService.GetByIdAsync(request.ProductId);
        if (product == null) throw new InvalidOperationException("Product not found");
        
        product.CurrentStock += request.Quantity;
        await _productService.UpdateAsync(product);
        
        var transaction = new StockTransaction
        {
            TransactionNumber = $"STK-{DateTime.UtcNow:yyyyMMdd}-{_transactionCounter++:D4}",
            ProductId = request.ProductId,
            ProductName = product.Name,
            Type = InventoryTransactionType.StockIn,
            Quantity = request.Quantity,
            UnitCost = request.UnitCost,
            TotalCost = request.Quantity * request.UnitCost,
            ReferenceNumber = request.ReferenceNumber,
            Notes = request.Notes,
            TransactionDate = DateTime.UtcNow,
            StockAfter = product.CurrentStock
        };
        
        _transactions.Add(transaction);
        return transaction;
    }
    
    public async Task<StockTransaction> ReturnStockAsync(ReturnStockRequest request)
    {
        var product = await _productService.GetByIdAsync(request.ProductId);
        if (product == null) throw new InvalidOperationException("Product not found");
        
        product.CurrentStock += request.Quantity;
        await _productService.UpdateAsync(product);
        
        var transaction = new StockTransaction
        {
            TransactionNumber = $"RTN-{DateTime.UtcNow:yyyyMMdd}-{_transactionCounter++:D4}",
            ProductId = request.ProductId,
            ProductName = product.Name,
            Type = InventoryTransactionType.StockReturn,
            Quantity = request.Quantity,
            UnitCost = product.CostPrice,
            TotalCost = request.Quantity * product.CostPrice,
            ReferenceNumber = request.ReferenceNumber,
            Notes = request.Reason,
            TransactionDate = DateTime.UtcNow,
            StockAfter = product.CurrentStock
        };
        
        _transactions.Add(transaction);
        return transaction;
    }
    
    public async Task<StockTransaction> AdjustStockAsync(AdjustStockRequest request)
    {
        InventoryProduct? product = await _productService.GetByIdAsync(request.ProductId);
        if (product == null) throw new InvalidOperationException("Product not found");
        
        product.CurrentStock = request.Quantity;
        await _productService.UpdateAsync(product);
        
        var transaction = new StockTransaction
        {
            TransactionNumber = $"ADJ-{DateTime.UtcNow:yyyyMMdd}-{_transactionCounter++:D4}",
            ProductId = request.ProductId,
            ProductName = product.Name,
            Type = InventoryTransactionType.StockAdjustment,
            Quantity = request.Quantity,
            UnitCost = product.CostPrice,
            TotalCost = request.Quantity * product.CostPrice,
            Notes = request.Reason,
            TransactionDate = DateTime.UtcNow,
            StockAfter = product.CurrentStock
        };
        
        _transactions.Add(transaction);
        return transaction;
    }
    
    public Task<StockTransaction?> GetTransactionAsync(Guid id) =>
        Task.FromResult(_transactions.FirstOrDefault(t => t.Id == id));
    
    public Task<List<StockTransaction>> GetProductTransactionsAsync(Guid productId) =>
        Task.FromResult(_transactions.Where(t => t.ProductId == productId).OrderByDescending(t => t.TransactionDate).ToList());
    
    public Task<List<StockTransaction>> GetTransactionsByDateAsync(DateTime from, DateTime to) =>
        Task.FromResult(_transactions.Where(t => t.TransactionDate >= from && t.TransactionDate <= to).OrderByDescending(t => t.TransactionDate).ToList());
    
    public Task<List<StockTransaction>> GetAllTransactionsAsync() =>
        Task.FromResult(_transactions.OrderByDescending(t => t.TransactionDate).ToList());
    
    public async Task<decimal> GetCurrentStockAsync(Guid productId)
    {
        var product = await _productService.GetByIdAsync(productId);
        return product?.CurrentStock ?? 0;
    }
}

public class InventoryCustomerService : IInventoryCustomerService
{
    private static readonly List<InventoryCustomer> _customers = InitializeDemoCustomers();
    private static int _codeCounter = 1001;
    
    private static List<InventoryCustomer> InitializeDemoCustomers()
    {
        return new List<InventoryCustomer>
        {
            new() { CustomerCode = "CUST-1001", Name = "Akwesasne General Store", Email = "store@akwesasne.ca", Phone = "+1-613-555-0101", Address = "123 Main Street, Akwesasne" },
            new() { CustomerCode = "CUST-1002", Name = "Mohawk Trading Post", Email = "trading@mohawk.ca", Phone = "+1-613-555-0102", Address = "456 Commerce Blvd, Akwesasne" },
            new() { CustomerCode = "CUST-1003", Name = "John Smith", Email = "john.smith@email.com", Phone = "+1-613-555-0103", Address = "789 Oak Street" },
            new() { CustomerCode = "CUST-1004", Name = "Mary Johnson", Email = "mary.j@email.com", Phone = "+1-613-555-0104", Address = "321 Elm Avenue" }
        };
    }
    
    public Task<List<InventoryCustomer>> GetAllAsync() => Task.FromResult(_customers.Where(c => c.IsActive).ToList());
    
    public Task<InventoryCustomer?> GetByIdAsync(Guid id) => Task.FromResult(_customers.FirstOrDefault(c => c.Id == id));
    
    public Task<InventoryCustomer?> GetByCodeAsync(string customerCode) =>
        Task.FromResult(_customers.FirstOrDefault(c => c.CustomerCode.Equals(customerCode, StringComparison.OrdinalIgnoreCase)));
    
    public Task<List<InventoryCustomer>> SearchAsync(string searchTerm) =>
        Task.FromResult(_customers.Where(c => 
            (c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
             c.CustomerCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
             c.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) &&
            c.IsActive).ToList());
    
    public Task<InventoryCustomer> CreateAsync(InventoryCustomer customer)
    {
        customer.Id = Guid.NewGuid();
        customer.CustomerCode = $"CUST-{_codeCounter++:D4}";
        customer.CreatedAt = DateTime.UtcNow;
        _customers.Add(customer);
        return Task.FromResult(customer);
    }
    
    public Task<InventoryCustomer> UpdateAsync(InventoryCustomer customer)
    {
        var index = _customers.FindIndex(c => c.Id == customer.Id);
        if (index >= 0) _customers[index] = customer;
        return Task.FromResult(customer);
    }
    
    public Task<bool> DeleteAsync(Guid id)
    {
        var customer = _customers.FirstOrDefault(c => c.Id == id);
        if (customer != null) { customer.IsActive = false; return Task.FromResult(true); }
        return Task.FromResult(false);
    }
}

public class PointOfSaleService : IPointOfSaleService
{
    private static readonly List<Sale> _sales = new();
    private static int _saleCounter = 1001;
    private readonly IInventoryProductService _productService;
    private readonly IStockTransactionService _stockService;
    
    public PointOfSaleService(IInventoryProductService productService, IStockTransactionService stockService)
    {
        _productService = productService;
        _stockService = stockService;
    }
    
    public async Task<Sale> CreateSaleAsync(CreateSaleRequest request)
    {
        var sale = new Sale
        {
            SaleNumber = $"SALE-{DateTime.UtcNow:yyyyMMdd}-{_saleCounter++:D4}",
            CustomerId = request.CustomerId,
            PaymentMethod = request.PaymentMethod,
            AmountPaid = request.AmountPaid,
            CashierId = request.CashierId,
            SaleDate = DateTime.UtcNow,
            Status = SaleStatus.Completed
        };
        
        decimal subTotal = 0;
        
        foreach (var itemRequest in request.Items)
        {
            InventoryProduct? product = await _productService.GetByIdAsync(itemRequest.ProductId);
            if (product == null) throw new InvalidOperationException($"Product {itemRequest.ProductId} not found");
            
            if (product.CurrentStock < itemRequest.Quantity)
                throw new InvalidOperationException($"Insufficient stock for {product.Name}");
            
            var unitPrice = itemRequest.UnitPrice ?? product.SellingPrice;
            var lineTotal = itemRequest.Quantity * unitPrice;
            
            var saleItem = new SaleItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = itemRequest.Quantity,
                UnitPrice = unitPrice,
                LineTotal = lineTotal
            };
            
            sale.Items.Add(saleItem);
            subTotal += lineTotal;
            
            // Update stock
            product.CurrentStock -= itemRequest.Quantity;
            await _productService.UpdateAsync(product);
        }
        
        sale.SubTotal = subTotal;
        sale.DiscountAmount = request.DiscountAmount ?? 0;
        sale.TaxAmount = (subTotal - sale.DiscountAmount) * (request.TaxRate / 100m);
        sale.TotalAmount = subTotal - sale.DiscountAmount + sale.TaxAmount;
        
        _sales.Add(sale);
        return sale;
    }
    
    public Task<Sale?> GetSaleAsync(Guid id) => Task.FromResult(_sales.FirstOrDefault(s => s.Id == id));
    
    public Task<List<Sale>> GetSalesByDateAsync(DateTime date) =>
        Task.FromResult(_sales.Where(s => s.SaleDate.Date == date.Date).ToList());
    
    public Task<List<Sale>> GetSalesByDateRangeAsync(DateTime from, DateTime to) =>
        Task.FromResult(_sales.Where(s => s.SaleDate >= from && s.SaleDate <= to).ToList());
    
    public Task<Sale> ProcessPaymentAsync(Guid saleId, InventoryPaymentRequest payment)
    {
        var sale = _sales.FirstOrDefault(s => s.Id == saleId);
        if (sale == null) throw new InvalidOperationException("Sale not found");
        
        sale.PaymentMethod = payment.Method;
        sale.AmountPaid = payment.Amount;
        
        return Task.FromResult(sale);
    }
    
    public Task<Sale> CancelSaleAsync(Guid saleId, string reason)
    {
        var sale = _sales.FirstOrDefault(s => s.Id == saleId);
        if (sale == null) throw new InvalidOperationException("Sale not found");
        
        sale.Status = SaleStatus.Cancelled;
        // TODO: Restore stock
        return Task.FromResult(sale);
    }
    
    public Task<List<Sale>> GetTodaySalesAsync() =>
        Task.FromResult(_sales.Where(s => s.SaleDate.Date == DateTime.UtcNow.Date).ToList());
}

public class InventoryReportService : IInventoryReportService
{
    private readonly IInventoryProductService _productService;
    private readonly IStockTransactionService _stockService;
    private readonly IPointOfSaleService _posService;
    
    public InventoryReportService(IInventoryProductService productService, IStockTransactionService stockService, IPointOfSaleService posService)
    {
        _productService = productService;
        _stockService = stockService;
        _posService = posService;
    }
    
    public async Task<InventoryReport> GetInventoryReportAsync()
    {
        var products = await _productService.GetAllAsync();
        var lowStock = await _productService.GetLowStockAsync(10);
        
        return new InventoryReport
        {
            TotalProducts = products.Count,
            LowStockProducts = lowStock.Count,
            TotalStockValue = products.Sum(p => p.CurrentStock * p.CostPrice)
        };
    }
    
    public async Task<DailyInventoryReport> GetDailyReportAsync(DateTime date)
    {
        var transactions = await _stockService.GetTransactionsByDateAsync(date.Date, date.Date.AddDays(1).AddTicks(-1));
        
        return new DailyInventoryReport
        {
            Date = date,
            StockInTransactions = transactions.Count(t => t.Type == InventoryTransactionType.StockIn),
            StockOutTransactions = transactions.Count(t => t.Type == InventoryTransactionType.StockOut),
            TotalStockIn = transactions.Where(t => t.Type == InventoryTransactionType.StockIn).Sum(t => t.Quantity),
            TotalStockOut = transactions.Where(t => t.Type == InventoryTransactionType.StockOut).Sum(t => t.Quantity),
            Transactions = transactions
        };
    }
    
    public async Task<MonthlyInventoryReport> GetMonthlyReportAsync(int year, int month)
    {
        var from = new DateTime(year, month, 1);
        var to = from.AddMonths(1).AddTicks(-1);
        var transactions = await _stockService.GetTransactionsByDateAsync(from, to);
        
        return new MonthlyInventoryReport
        {
            Year = year,
            Month = month,
            TotalTransactions = transactions.Count,
            TotalStockIn = transactions.Where(t => t.Type == InventoryTransactionType.StockIn).Sum(t => t.Quantity),
            TotalStockOut = transactions.Where(t => t.Type == InventoryTransactionType.StockOut).Sum(t => t.Quantity)
        };
    }
    
    public async Task<AllStockReport> GetAllStockReportAsync()
    {
        var products = await _productService.GetAllAsync();
        
        return new AllStockReport
        {
            Items = products.Select(p => new StockItem
            {
                ProductId = p.Id,
                SKU = p.SKU,
                ProductName = p.Name,
                CurrentStock = p.CurrentStock,
                UnitCost = p.CostPrice,
                StockValue = p.CurrentStock * p.CostPrice
            }).ToList(),
            TotalStockValue = products.Sum(p => p.CurrentStock * p.CostPrice)
        };
    }
    
    public async Task<StockMovementReport> GetStockMovementReportAsync(DateTime from, DateTime to)
    {
        var transactions = await _stockService.GetTransactionsByDateAsync(from, to);
        
        return new StockMovementReport
        {
            FromDate = from,
            ToDate = to,
            Transactions = transactions
        };
    }
    
    public async Task<SalesReport> GetSalesReportAsync(DateTime from, DateTime to)
    {
        var sales = await _posService.GetSalesByDateRangeAsync(from, to);
        
        return new SalesReport
        {
            FromDate = from,
            ToDate = to,
            TotalSales = sales.Count,
            TotalRevenue = sales.Sum(s => s.TotalAmount),
            TotalProfit = sales.Sum(s => s.Items.Sum(i => (i.UnitPrice - (s.Items.First(it => it.ProductId == i.ProductId).UnitPrice * 0.7m)) * i.Quantity))
        };
    }
    
    public async Task<LowStockReport> GetLowStockReportAsync(int threshold = 10)
    {
        var lowStock = await _productService.GetLowStockAsync(threshold);
        
        return new LowStockReport
        {
            Threshold = threshold,
            Products = lowStock
        };
    }
}

public class UserService : IUserService
{
    private static readonly List<User> _users = InitializeDemoUsers();
    
    private static List<User> InitializeDemoUsers()
    {
        return new List<User>
        {
            new() { Username = "admin", Password = "admin", FirstName = "Admin", LastName = "User", Role = UserRole.Admin },
            new() { Username = "manager", Password = "manager", FirstName = "Manager", LastName = "User", Role = UserRole.Manager },
            new() { Username = "cashier1", Password = "cashier", FirstName = "Cashier", LastName = "One", Role = UserRole.Cashier },
            new() { Username = "stock1", Password = "stock", FirstName = "Stock", LastName = "Keeper", Role = UserRole.StockKeeper }
        };
    }
    
    public Task<List<User>> GetAllAsync() => Task.FromResult(_users.Where(u => u.IsActive).ToList());
    
    public Task<User?> GetByIdAsync(Guid id) => Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
    
    public Task<User?> GetByUsernameAsync(string username) =>
        Task.FromResult(_users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)));
    
    public Task<User> CreateAsync(User user)
    {
        user.Id = Guid.NewGuid();
        user.CreatedAt = DateTime.UtcNow;
        _users.Add(user);
        return Task.FromResult(user);
    }
    
    public Task<User> UpdateAsync(User user)
    {
        var index = _users.FindIndex(u => u.Id == user.Id);
        if (index >= 0) _users[index] = user;
        return Task.FromResult(user);
    }
    
    public Task<bool> DeleteAsync(Guid id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user != null) { user.IsActive = false; return Task.FromResult(true); }
        return Task.FromResult(false);
    }
    
    public Task<bool> AuthenticateAsync(string username, string password)
    {
        var user = _users.FirstOrDefault(u => 
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && 
            u.Password == password && 
            u.IsActive);
        return Task.FromResult(user != null);
    }
}
