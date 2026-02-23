using NET10.Core.Interfaces;
using NET10.Core.Models.ERP;

namespace NET10.Infrastructure.Services.ERP;

/// <summary>
/// Purchase Order Service - Manage purchase orders and receiving
/// </summary>
public class PurchaseOrderService : IPurchaseOrderService
{
    private static readonly List<PurchaseOrder> _orders = [];
    private static readonly Dictionary<Guid, int> _poCounters = [];
    
    public Task<List<PurchaseOrder>> GetAllAsync(Guid companyId)
    {
        var orders = _orders.Where(o => o.CompanyId == companyId)
                            .OrderByDescending(o => o.OrderDate)
                            .ToList();
        return Task.FromResult(orders);
    }
    
    public Task<PurchaseOrder?> GetByIdAsync(Guid id)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);
        return Task.FromResult(order);
    }
    
    public async Task<PurchaseOrder> CreateAsync(PurchaseOrder po)
    {
        po.Id = Guid.NewGuid();
        po.CreatedAt = DateTime.UtcNow;
        
        if (string.IsNullOrEmpty(po.PONumber))
        {
            po.PONumber = await GeneratePONumberAsync(po.CompanyId);
        }
        
        _orders.Add(po);
        return po;
    }
    
    public Task<PurchaseOrder> UpdateAsync(PurchaseOrder po)
    {
        var index = _orders.FindIndex(o => o.Id == po.Id);
        if (index >= 0)
        {
            _orders[index] = po;
        }
        return Task.FromResult(po);
    }
    
    public Task<PurchaseOrder> ReceiveItemsAsync(Guid poId, List<ReceiveItem> items)
    {
        var order = _orders.FirstOrDefault(o => o.Id == poId);
        if (order != null)
        {
            foreach (var item in items)
            {
                var line = order.Items.FirstOrDefault(l => l.ProductId == item.ProductId);
                if (line != null)
                {
                    line.QuantityReceived += item.Quantity;
                }
            }
            
            // Update status
            var allReceived = order.Items.All(l => l.QuantityReceived >= l.QuantityOrdered);
            var anyReceived = order.Items.Any(l => l.QuantityReceived > 0);
            
            if (allReceived)
                order.Status = PurchaseOrderStatus.Received;
            else if (anyReceived)
                order.Status = PurchaseOrderStatus.Partial_Received;
        }
        return Task.FromResult(order!);
    }
    
    public Task<string> GeneratePONumberAsync(Guid companyId)
    {
        if (!_poCounters.ContainsKey(companyId))
        {
            _poCounters[companyId] = 1000;
        }
        
        _poCounters[companyId]++;
        var number = $"PO-{_poCounters[companyId]:D6}";
        return Task.FromResult(number);
    }
}
