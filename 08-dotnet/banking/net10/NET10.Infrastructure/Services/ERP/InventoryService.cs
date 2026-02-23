using NET10.Core.Interfaces;
using NET10.Core.Models.ERP;
using ERPInventoryTransactionType = NET10.Core.Models.ERP.InventoryTransactionType;

namespace NET10.Infrastructure.Services.ERP;

/// <summary>
/// Inventory Service - Stock management, warehouses, transactions
/// </summary>
public class InventoryService : IInventoryService
{
    private static readonly List<InventoryStock> _stocks = [];
    private static readonly List<InventoryTransaction> _transactions = [];
    private static readonly List<StockAdjustment> _adjustments = [];
    private static readonly List<Warehouse> _warehouses = [];
    private static bool _initialized = false;
    
    public InventoryService()
    {
        if (!_initialized)
        {
            InitializeDefaults();
            _initialized = true;
        }
    }
    
    public Task<List<InventoryStock>> GetStockLevelsAsync(Guid companyId)
    {
        var stock = _stocks.Where(s => s.CompanyId == companyId || s.CompanyId == Guid.Empty)
                           .ToList();
        return Task.FromResult(stock);
    }
    
    public Task<InventoryStock?> GetStockAsync(Guid productId, Guid warehouseId)
    {
        var stock = _stocks.FirstOrDefault(s => s.ProductId == productId && s.WarehouseId == warehouseId);
        return Task.FromResult(stock);
    }
    
    public Task<InventoryTransaction> RecordTransactionAsync(InventoryTransaction transaction)
    {
        transaction.Id = Guid.NewGuid();
        transaction.TransactionDate = DateTime.UtcNow;
        
        // Update stock
        var stock = _stocks.FirstOrDefault(s => s.ProductId == transaction.ProductId && 
                                                 s.WarehouseId == transaction.WarehouseId);
        if (stock != null)
        {
            switch (transaction.Type)
            {
                case ERPInventoryTransactionType.Purchase:
                case ERPInventoryTransactionType.Return:
                case ERPInventoryTransactionType.Adjustment_In:
                case ERPInventoryTransactionType.Opening:
                    stock.QuantityOnHand += transaction.Quantity;
                    break;
                case ERPInventoryTransactionType.Sale:
                case ERPInventoryTransactionType.Adjustment_Out:
                case ERPInventoryTransactionType.Damage:
                case ERPInventoryTransactionType.Expired:
                    stock.QuantityOnHand -= transaction.Quantity;
                    break;
                case ERPInventoryTransactionType.Transfer:
                    stock.QuantityOnHand -= transaction.Quantity;
                    // TODO: Add to destination warehouse
                    break;
            }
            stock.LastUpdated = DateTime.UtcNow;
        }
        
        _transactions.Add(transaction);
        return Task.FromResult(transaction);
    }
    
    public Task<List<InventoryTransaction>> GetTransactionsAsync(Guid productId, DateTime fromDate, DateTime toDate)
    {
        var transactions = _transactions
            .Where(t => t.ProductId == productId && 
                        t.TransactionDate >= fromDate && 
                        t.TransactionDate <= toDate)
            .OrderByDescending(t => t.TransactionDate)
            .ToList();
        return Task.FromResult(transactions);
    }
    
    public Task<StockAdjustment> CreateAdjustmentAsync(StockAdjustment adjustment)
    {
        adjustment.Id = Guid.NewGuid();
        adjustment.CreatedAt = DateTime.UtcNow;
        adjustment.Status = StockAdjustmentStatus.Pending_Approval;
        _adjustments.Add(adjustment);
        return Task.FromResult(adjustment);
    }
    
    public async Task<StockAdjustment> ApproveAdjustmentAsync(Guid adjustmentId, string approvedBy)
    {
        var adjustment = _adjustments.FirstOrDefault(a => a.Id == adjustmentId);
        if (adjustment != null)
        {
            adjustment.Status = StockAdjustmentStatus.Approved;
            adjustment.ApprovedBy = approvedBy;
            adjustment.ApprovedAt = DateTime.UtcNow;
            
            // Apply adjustment
            foreach (var line in adjustment.Items)
            {
                var transaction = new InventoryTransaction
                {
                    ProductId = line.ProductId,
                    WarehouseId = adjustment.WarehouseId,
                    Quantity = Math.Abs(line.Difference),
                    Type = line.Difference > 0 ? ERPInventoryTransactionType.Adjustment_In : ERPInventoryTransactionType.Adjustment_Out,
                    Reference = $"ADJ-{adjustment.Id}",
                    Notes = adjustment.Reason
                };
                await RecordTransactionAsync(transaction);
            }
        }
        return adjustment!;
    }
    
    public Task<List<Warehouse>> GetWarehousesAsync(Guid companyId)
    {
        var warehouses = _warehouses.Where(w => w.CompanyId == companyId || w.CompanyId == Guid.Empty)
                                    .ToList();
        return Task.FromResult(warehouses);
    }
    
    private void InitializeDefaults()
    {
        // Default warehouses
        var mainWarehouse = new Warehouse
        {
            Code = "WH-MAIN",
            Name = "Almacén Principal",
            Address = "123 Main Street",
            City = "New York",
            Country = "USA",
            IsDefault = true,
            IsActive = true
        };
        
        var secondaryWarehouse = new Warehouse
        {
            Code = "WH-SEC",
            Name = "Almacén Secundario",
            Address = "456 Secondary Ave",
            City = "Los Angeles",
            Country = "USA",
            IsDefault = false,
            IsActive = true
        };
        
        _warehouses.Add(mainWarehouse);
        _warehouses.Add(secondaryWarehouse);
        
        // Default stock
        _stocks.Add(new InventoryStock
        {
            ProductId = Guid.NewGuid(),
            ProductSKU = "PROD-001",
            ProductName = "Consultoría Técnica",
            WarehouseId = mainWarehouse.Id,
            WarehouseName = mainWarehouse.Name,
            QuantityOnHand = 999,
            QuantityReserved = 0,
            ReorderPoint = 10,
            UnitCost = 80
        });
    }
}
