using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;
using NET10.Core.Models.ERP;

namespace NET10.API.Controllers.ERP;

/// <summary>
/// Supplier Controller - Manage vendors/suppliers
/// </summary>
[ApiController]
[Route("api/erp/[controller]")]
[Produces("application/json")]
public class SupplierController : ControllerBase
{
    private readonly ISupplierService _supplierService;
    
    public SupplierController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }
    
    /// <summary>
    /// Get all suppliers for a company
    /// </summary>
    [HttpGet("{companyId}")]
    public async Task<ActionResult<List<Supplier>>> GetAll(Guid companyId)
    {
        var suppliers = await _supplierService.GetAllAsync(companyId);
        return Ok(suppliers);
    }
    
    /// <summary>
    /// Get supplier by ID
    /// </summary>
    [HttpGet("detail/{id}")]
    public async Task<ActionResult<Supplier>> GetById(Guid id)
    {
        var supplier = await _supplierService.GetByIdAsync(id);
        if (supplier == null) return NotFound();
        return Ok(supplier);
    }
    
    /// <summary>
    /// Create a new supplier
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Supplier>> Create([FromBody] Supplier supplier)
    {
        var created = await _supplierService.CreateAsync(supplier);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
    
    /// <summary>
    /// Update supplier
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<Supplier>> Update(Guid id, [FromBody] Supplier supplier)
    {
        supplier.Id = id;
        var updated = await _supplierService.UpdateAsync(supplier);
        return Ok(updated);
    }
    
    /// <summary>
    /// Delete supplier
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _supplierService.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}
