using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;
using NET10.Core.Models.ERP;

namespace NET10.API.Controllers.ERP
{
    [ApiController]
    [Route("api/erp/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        
        /// <summary>
        /// Get all customers for a company
        /// </summary>
        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<List<Customer>>> GetAll(Guid companyId)
        {
            var customers = await _customerService.GetAllAsync(companyId);
            return Ok(customers);
        }
        
        /// <summary>
        /// Get customer by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetById(Guid id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound();
            return Ok(customer);
        }
        
        /// <summary>
        /// Create new customer
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Customer>> Create([FromBody] Customer customer)
        {
            var created = await _customerService.CreateAsync(customer);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        
        /// <summary>
        /// Update customer
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<Customer>> Update(Guid id, [FromBody] Customer customer)
        {
            if (id != customer.Id)
                return BadRequest("ID mismatch");
            
            var updated = await _customerService.UpdateAsync(customer);
            return Ok(updated);
        }
        
        /// <summary>
        /// Delete customer
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _customerService.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
        
        /// <summary>
        /// Search customers
        /// </summary>
        [HttpGet("company/{companyId}/search")]
        public async Task<ActionResult<List<Customer>>> Search(Guid companyId, [FromQuery] string q)
        {
            if (string.IsNullOrEmpty(q))
                return BadRequest("Search term required");
            
            var customers = await _customerService.SearchAsync(companyId, q);
            return Ok(customers);
        }
        
        /// <summary>
        /// Get customer statement
        /// </summary>
        [HttpGet("{id}/statement")]
        public async Task<ActionResult<CustomerStatement>> GetStatement(
            Guid id, 
            [FromQuery] DateTime? fromDate, 
            [FromQuery] DateTime? toDate)
        {
            var from = fromDate ?? DateTime.UtcNow.AddMonths(-3);
            var to = toDate ?? DateTime.UtcNow;
            var statement = await _customerService.GetStatementAsync(id, from, to);
            return Ok(statement);
        }
    }
}
