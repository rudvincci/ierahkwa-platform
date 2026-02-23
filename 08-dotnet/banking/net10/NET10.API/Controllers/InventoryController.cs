using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;

namespace NET10.API.Controllers;

/// <summary>
/// Inventory Controller - Ierahkwa Stock Management & Point of Sale System
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IUnitOfMeasureService _uomService;
    private readonly IStockTransactionService _stockService;
    private readonly IInventoryCustomerService _customerService;
    private readonly IPointOfSaleService _posService;
    private readonly IInventoryReportService _reportService;
    private readonly IUserService _userService;
    
    public InventoryController(
        IInventoryProductService productService,
        ICategoryService categoryService,
        IUnitOfMeasureService uomService,
        IStockTransactionService stockService,
        IInventoryCustomerService customerService,
        IPointOfSaleService posService,
        IInventoryReportService reportService,
        IUserService userService)
    {
        _productService = productService;
        _categoryService = categoryService;
        _uomService = uomService;
        _stockService = stockService;
        _customerService = customerService;
        _posService = posService;
        _reportService = reportService;
        _userService = userService;
    }
    
    // ═══════════════════════════════════════════════════════════════
    // PRODUCTS
    // ═══════════════════════════════════════════════════════════════
    
    [HttpGet("products")]
    public async Task<ActionResult<List<InventoryProduct>>> GetInventoryProducts()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }
    
    [HttpGet("products/{id}")]
    public async Task<ActionResult<InventoryProduct>> GetInventoryProduct(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }
    
    [HttpGet("products/sku/{sku}")]
    public async Task<ActionResult<InventoryProduct>> GetInventoryProductBySKU(string sku)
    {
        var product = await _productService.GetBySKUAsync(sku);
        if (product == null) return NotFound();
        return Ok(product);
    }
    
    [HttpGet("products/search")]
    public async Task<ActionResult<List<InventoryProduct>>> SearchInventoryProducts([FromQuery] string term)
    {
        var products = await _productService.SearchAsync(term);
        return Ok(products);
    }
    
    [HttpGet("products/low-stock")]
    public async Task<ActionResult<List<InventoryProduct>>> GetLowStock([FromQuery] int threshold = 10)
    {
        var products = await _productService.GetLowStockAsync(threshold);
        return Ok(products);
    }
    
    [HttpPost("products")]
    public async Task<ActionResult<InventoryProduct>> CreateInventoryProduct([FromBody] InventoryProduct product)
    {
        var created = await _productService.CreateAsync(product);
        return CreatedAtAction(nameof(GetInventoryProduct), new { id = created.Id }, created);
    }
    
    [HttpPut("products/{id}")]
    public async Task<ActionResult<InventoryProduct>> UpdateInventoryProduct(Guid id, [FromBody] InventoryProduct product)
    {
        product.Id = id;
        var updated = await _productService.UpdateAsync(product);
        return Ok(updated);
    }
    
    [HttpDelete("products/{id}")]
    public async Task<ActionResult> DeleteInventoryProduct(Guid id)
    {
        var result = await _productService.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
    
    // ═══════════════════════════════════════════════════════════════
    // CATEGORIES
    // ═══════════════════════════════════════════════════════════════
    
    [HttpGet("categories")]
    public async Task<ActionResult<List<Category>>> GetCategories()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(categories);
    }
    
    [HttpGet("categories/{id}")]
    public async Task<ActionResult<Category>> GetCategory(Guid id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null) return NotFound();
        return Ok(category);
    }
    
    [HttpPost("categories")]
    public async Task<ActionResult<Category>> CreateCategory([FromBody] Category category)
    {
        var created = await _categoryService.CreateAsync(category);
        return CreatedAtAction(nameof(GetCategory), new { id = created.Id }, created);
    }
    
    [HttpPut("categories/{id}")]
    public async Task<ActionResult<Category>> UpdateCategory(Guid id, [FromBody] Category category)
    {
        category.Id = id;
        var updated = await _categoryService.UpdateAsync(category);
        return Ok(updated);
    }
    
    [HttpDelete("categories/{id}")]
    public async Task<ActionResult> DeleteCategory(Guid id)
    {
        var result = await _categoryService.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
    
    // ═══════════════════════════════════════════════════════════════
    // UNIT OF MEASURE
    // ═══════════════════════════════════════════════════════════════
    
    [HttpGet("uom")]
    public async Task<ActionResult<List<UnitOfMeasure>>> GetUOMs()
    {
        var uoms = await _uomService.GetAllAsync();
        return Ok(uoms);
    }
    
    [HttpGet("uom/{id}")]
    public async Task<ActionResult<UnitOfMeasure>> GetUOM(Guid id)
    {
        var uom = await _uomService.GetByIdAsync(id);
        if (uom == null) return NotFound();
        return Ok(uom);
    }
    
    [HttpPost("uom")]
    public async Task<ActionResult<UnitOfMeasure>> CreateUOM([FromBody] UnitOfMeasure uom)
    {
        var created = await _uomService.CreateAsync(uom);
        return CreatedAtAction(nameof(GetUOM), new { id = created.Id }, created);
    }
    
    [HttpPut("uom/{id}")]
    public async Task<ActionResult<UnitOfMeasure>> UpdateUOM(Guid id, [FromBody] UnitOfMeasure uom)
    {
        uom.Id = id;
        var updated = await _uomService.UpdateAsync(uom);
        return Ok(updated);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // STOCK TRANSACTIONS
    // ═══════════════════════════════════════════════════════════════
    
    [HttpPost("stock/add")]
    public async Task<ActionResult<StockTransaction>> AddStock([FromBody] AddStockRequest request)
    {
        try
        {
            var transaction = await _stockService.AddStockAsync(request);
            return Ok(transaction);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    [HttpPost("stock/return")]
    public async Task<ActionResult<StockTransaction>> ReturnStock([FromBody] ReturnStockRequest request)
    {
        try
        {
            var transaction = await _stockService.ReturnStockAsync(request);
            return Ok(transaction);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    [HttpPost("stock/adjust")]
    public async Task<ActionResult<StockTransaction>> AdjustStock([FromBody] AdjustStockRequest request)
    {
        try
        {
            var transaction = await _stockService.AdjustStockAsync(request);
            return Ok(transaction);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    [HttpGet("stock/transactions")]
    public async Task<ActionResult<List<StockTransaction>>> GetTransactions(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        if (from.HasValue && to.HasValue)
        {
            var transactions = await _stockService.GetTransactionsByDateAsync(from.Value, to.Value);
            return Ok(transactions);
        }
        var all = await _stockService.GetAllTransactionsAsync();
        return Ok(all);
    }
    
    [HttpGet("stock/products/{productId}/transactions")]
    public async Task<ActionResult<List<StockTransaction>>> GetInventoryProductTransactions(Guid productId)
    {
        var transactions = await _stockService.GetProductTransactionsAsync(productId);
        return Ok(transactions);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // CUSTOMERS
    // ═══════════════════════════════════════════════════════════════
    
    [HttpGet("customers")]
    public async Task<ActionResult<List<InventoryCustomer>>> GetInventoryCustomers()
    {
        var customers = await _customerService.GetAllAsync();
        return Ok(customers);
    }
    
    [HttpGet("customers/{id}")]
    public async Task<ActionResult<InventoryCustomer>> GetInventoryCustomer(Guid id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer == null) return NotFound();
        return Ok(customer);
    }
    
    [HttpGet("customers/search")]
    public async Task<ActionResult<List<InventoryCustomer>>> SearchInventoryCustomers([FromQuery] string term)
    {
        var customers = await _customerService.SearchAsync(term);
        return Ok(customers);
    }
    
    [HttpPost("customers")]
    public async Task<ActionResult<InventoryCustomer>> CreateInventoryCustomer([FromBody] InventoryCustomer customer)
    {
        var created = await _customerService.CreateAsync(customer);
        return CreatedAtAction(nameof(GetInventoryCustomer), new { id = created.Id }, created);
    }
    
    [HttpPut("customers/{id}")]
    public async Task<ActionResult<InventoryCustomer>> UpdateInventoryCustomer(Guid id, [FromBody] InventoryCustomer customer)
    {
        customer.Id = id;
        var updated = await _customerService.UpdateAsync(customer);
        return Ok(updated);
    }
    
    [HttpDelete("customers/{id}")]
    public async Task<ActionResult> DeleteInventoryCustomer(Guid id)
    {
        var result = await _customerService.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
    
    // ═══════════════════════════════════════════════════════════════
    // POINT OF SALE
    // ═══════════════════════════════════════════════════════════════
    
    [HttpPost("pos/sale")]
    public async Task<ActionResult<Sale>> CreateSale([FromBody] CreateSaleRequest request)
    {
        try
        {
            var sale = await _posService.CreateSaleAsync(request);
            return Ok(sale);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    [HttpGet("pos/sales/{id}")]
    public async Task<ActionResult<Sale>> GetSale(Guid id)
    {
        var sale = await _posService.GetSaleAsync(id);
        if (sale == null) return NotFound();
        return Ok(sale);
    }
    
    [HttpGet("pos/sales/today")]
    public async Task<ActionResult<List<Sale>>> GetTodaySales()
    {
        var sales = await _posService.GetTodaySalesAsync();
        return Ok(sales);
    }
    
    [HttpGet("pos/sales")]
    public async Task<ActionResult<List<Sale>>> GetSales(
        [FromQuery] DateTime? date = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        if (date.HasValue)
        {
            var sales = await _posService.GetSalesByDateAsync(date.Value);
            return Ok(sales);
        }
        if (from.HasValue && to.HasValue)
        {
            var sales = await _posService.GetSalesByDateRangeAsync(from.Value, to.Value);
            return Ok(sales);
        }
        var today = await _posService.GetTodaySalesAsync();
        return Ok(today);
    }
    
    [HttpPost("pos/sales/{id}/cancel")]
    public async Task<ActionResult<Sale>> CancelSale(Guid id, [FromQuery] string? reason = null)
    {
        try
        {
            var sale = await _posService.CancelSaleAsync(id, reason ?? "Cancelled by user");
            return Ok(sale);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    // ═══════════════════════════════════════════════════════════════
    // REPORTS
    // ═══════════════════════════════════════════════════════════════
    
    [HttpGet("reports/inventory")]
    public async Task<ActionResult<InventoryReport>> GetInventoryReport()
    {
        var report = await _reportService.GetInventoryReportAsync();
        return Ok(report);
    }
    
    [HttpGet("reports/daily")]
    public async Task<ActionResult<DailyInventoryReport>> GetDailyReport([FromQuery] DateTime? date = null)
    {
        var report = await _reportService.GetDailyReportAsync(date ?? DateTime.UtcNow);
        return Ok(report);
    }
    
    [HttpGet("reports/monthly")]
    public async Task<ActionResult<MonthlyInventoryReport>> GetMonthlyReport(
        [FromQuery] int year,
        [FromQuery] int month)
    {
        var report = await _reportService.GetMonthlyReportAsync(year, month);
        return Ok(report);
    }
    
    [HttpGet("reports/all-stock")]
    public async Task<ActionResult<AllStockReport>> GetAllStockReport()
    {
        var report = await _reportService.GetAllStockReportAsync();
        return Ok(report);
    }
    
    [HttpGet("reports/stock-movement")]
    public async Task<ActionResult<StockMovementReport>> GetStockMovementReport(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var report = await _reportService.GetStockMovementReportAsync(
            from ?? DateTime.UtcNow.AddDays(-30),
            to ?? DateTime.UtcNow);
        return Ok(report);
    }
    
    [HttpGet("reports/sales")]
    public async Task<ActionResult<SalesReport>> GetSalesReport(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var report = await _reportService.GetSalesReportAsync(
            from ?? DateTime.UtcNow.AddDays(-30),
            to ?? DateTime.UtcNow);
        return Ok(report);
    }
    
    [HttpGet("reports/low-stock")]
    public async Task<ActionResult<LowStockReport>> GetLowStockReport([FromQuery] int threshold = 10)
    {
        var report = await _reportService.GetLowStockReportAsync(threshold);
        return Ok(report);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // USERS
    // ═══════════════════════════════════════════════════════════════
    
    [HttpGet("users")]
    public async Task<ActionResult<List<User>>> GetUsers()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }
    
    [HttpGet("users/{id}")]
    public async Task<ActionResult<User>> GetUser(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }
    
    [HttpPost("users")]
    public async Task<ActionResult<User>> CreateUser([FromBody] User user)
    {
        var created = await _userService.CreateAsync(user);
        return CreatedAtAction(nameof(GetUser), new { id = created.Id }, created);
    }
    
    [HttpPut("users/{id}")]
    public async Task<ActionResult<User>> UpdateUser(Guid id, [FromBody] User user)
    {
        user.Id = id;
        var updated = await _userService.UpdateAsync(user);
        return Ok(updated);
    }
    
    [HttpPost("users/authenticate")]
    public async Task<ActionResult<bool>> Authenticate([FromBody] LoginRequest request)
    {
        var result = await _userService.AuthenticateAsync(request.Username, request.Password);
        return Ok(new { authenticated = result });
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
