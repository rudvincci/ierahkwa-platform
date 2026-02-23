using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;
using NET10.Core.Models.ERP;

namespace NET10.API.Controllers.ERP;

/// <summary>
/// Purchase Order Controller - Manage purchase orders and receiving
/// </summary>
[ApiController]
[Route("api/erp/[controller]")]
[Produces("application/json")]
public class PurchaseOrderController : ControllerBase
{
    private readonly IPurchaseOrderService _poService;
    
    public PurchaseOrderController(IPurchaseOrderService poService)
    {
        _poService = poService;
    }
    
    /// <summary>
    /// Get all purchase orders for a company
    /// </summary>
    [HttpGet("{companyId}")]
    public async Task<ActionResult<List<PurchaseOrder>>> GetAll(Guid companyId)
    {
        var orders = await _poService.GetAllAsync(companyId);
        return Ok(orders);
    }
    
    /// <summary>
    /// Get purchase order by ID
    /// </summary>
    [HttpGet("detail/{id}")]
    public async Task<ActionResult<PurchaseOrder>> GetById(Guid id)
    {
        var order = await _poService.GetByIdAsync(id);
        if (order == null) return NotFound();
        return Ok(order);
    }
    
    /// <summary>
    /// Create new purchase order
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PurchaseOrder>> Create([FromBody] PurchaseOrder po)
    {
        var created = await _poService.CreateAsync(po);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
    
    /// <summary>
    /// Update purchase order
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<PurchaseOrder>> Update(Guid id, [FromBody] PurchaseOrder po)
    {
        po.Id = id;
        var updated = await _poService.UpdateAsync(po);
        return Ok(updated);
    }
    
    /// <summary>
    /// Receive items against purchase order
    /// </summary>
    [HttpPost("{id}/receive")]
    public async Task<ActionResult<PurchaseOrder>> ReceiveItems(Guid id, [FromBody] List<ReceiveItem> items)
    {
        var updated = await _poService.ReceiveItemsAsync(id, items);
        return Ok(updated);
    }
    
    /// <summary>
    /// Generate next PO number
    /// </summary>
    [HttpGet("next-number/{companyId}")]
    public async Task<ActionResult<string>> GetNextNumber(Guid companyId)
    {
        var number = await _poService.GeneratePONumberAsync(companyId);
        return Ok(new { poNumber = number });
    }
}
