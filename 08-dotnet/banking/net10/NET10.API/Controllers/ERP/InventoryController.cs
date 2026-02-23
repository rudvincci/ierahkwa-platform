using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;
using NET10.Core.Models.ERP;

namespace NET10.API.Controllers.ERP;

/// <summary>
/// Inventory Controller - Stock management, warehouses, transactions
/// </summary>
[ApiController]
[Route("api/erp/[controller]")]
[Produces("application/json")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    
    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }
    
    // ═══════════════════════════════════════════════════════════════
    // STOCK LEVELS
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get all stock levels for a company
    /// </summary>
    [HttpGet("stock/{companyId}")]
    public async Task<ActionResult<List<InventoryStock>>> GetStockLevels(Guid companyId)
    {
        var stock = await _inventoryService.GetStockLevelsAsync(companyId);
        return Ok(stock);
    }
    
    /// <summary>
    /// Get stock for specific product in warehouse
    /// </summary>
    [HttpGet("stock/{productId}/{warehouseId}")]
    public async Task<ActionResult<InventoryStock>> GetStock(Guid productId, Guid warehouseId)
    {
        var stock = await _inventoryService.GetStockAsync(productId, warehouseId);
        if (stock == null) return NotFound();
        return Ok(stock);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // TRANSACTIONS
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Record inventory transaction (receipt, shipment, adjustment)
    /// </summary>
    [HttpPost("transactions")]
    public async Task<ActionResult<InventoryTransaction>> RecordTransaction([FromBody] InventoryTransaction transaction)
    {
        var recorded = await _inventoryService.RecordTransactionAsync(transaction);
        return Ok(recorded);
    }
    
    /// <summary>
    /// Get transactions for a product
    /// </summary>
    [HttpGet("transactions/{productId}")]
    public async Task<ActionResult<List<InventoryTransaction>>> GetTransactions(
        Guid productId, 
        [FromQuery] DateTime fromDate, 
        [FromQuery] DateTime toDate)
    {
        var transactions = await _inventoryService.GetTransactionsAsync(productId, fromDate, toDate);
        return Ok(transactions);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // ADJUSTMENTS
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Create stock adjustment
    /// </summary>
    [HttpPost("adjustments")]
    public async Task<ActionResult<StockAdjustment>> CreateAdjustment([FromBody] StockAdjustment adjustment)
    {
        var created = await _inventoryService.CreateAdjustmentAsync(adjustment);
        return Ok(created);
    }
    
    /// <summary>
    /// Approve stock adjustment
    /// </summary>
    [HttpPost("adjustments/{id}/approve")]
    public async Task<ActionResult<StockAdjustment>> ApproveAdjustment(Guid id, [FromQuery] string approvedBy)
    {
        var approved = await _inventoryService.ApproveAdjustmentAsync(id, approvedBy);
        return Ok(approved);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // WAREHOUSES
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get all warehouses for a company
    /// </summary>
    [HttpGet("warehouses/{companyId}")]
    public async Task<ActionResult<List<Warehouse>>> GetWarehouses(Guid companyId)
    {
        var warehouses = await _inventoryService.GetWarehousesAsync(companyId);
        return Ok(warehouses);
    }
}
