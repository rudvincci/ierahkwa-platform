using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;
using NET10.Core.Models.ERP;

namespace NET10.API.Controllers.ERP
{
    [ApiController]
    [Route("api/erp/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        
        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }
        
        /// <summary>
        /// Get all invoices for a company
        /// </summary>
        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<List<Invoice>>> GetAll(Guid companyId)
        {
            var invoices = await _invoiceService.GetAllAsync(companyId);
            return Ok(invoices);
        }
        
        /// <summary>
        /// Get invoice by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> GetById(Guid id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null)
                return NotFound();
            return Ok(invoice);
        }
        
        /// <summary>
        /// Get invoice by number
        /// </summary>
        [HttpGet("company/{companyId}/number/{invoiceNumber}")]
        public async Task<ActionResult<Invoice>> GetByNumber(Guid companyId, string invoiceNumber)
        {
            var invoice = await _invoiceService.GetByNumberAsync(companyId, invoiceNumber);
            if (invoice == null)
                return NotFound();
            return Ok(invoice);
        }
        
        /// <summary>
        /// Create new invoice
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Invoice>> Create([FromBody] Invoice invoice)
        {
            var created = await _invoiceService.CreateAsync(invoice);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        
        /// <summary>
        /// Update invoice
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<Invoice>> Update(Guid id, [FromBody] Invoice invoice)
        {
            if (id != invoice.Id)
                return BadRequest("ID mismatch");
            
            var updated = await _invoiceService.UpdateAsync(invoice);
            return Ok(updated);
        }
        
        /// <summary>
        /// Delete invoice
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _invoiceService.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
        
        /// <summary>
        /// Generate next invoice number
        /// </summary>
        [HttpGet("company/{companyId}/next-number")]
        public async Task<ActionResult<string>> GetNextNumber(Guid companyId)
        {
            var number = await _invoiceService.GenerateInvoiceNumberAsync(companyId);
            return Ok(new { invoiceNumber = number });
        }
        
        /// <summary>
        /// Send invoice by email
        /// </summary>
        [HttpPost("{id}/send")]
        public async Task<ActionResult<Invoice>> SendInvoice(Guid id, [FromBody] SendInvoiceRequest request)
        {
            var invoice = await _invoiceService.SendInvoiceAsync(id, request.Email);
            return Ok(invoice);
        }
        
        /// <summary>
        /// Get invoices by customer
        /// </summary>
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<List<Invoice>>> GetByCustomer(Guid customerId)
        {
            var invoices = await _invoiceService.GetByCustomerAsync(customerId);
            return Ok(invoices);
        }
        
        /// <summary>
        /// Get overdue invoices
        /// </summary>
        [HttpGet("company/{companyId}/overdue")]
        public async Task<ActionResult<List<Invoice>>> GetOverdue(Guid companyId)
        {
            var invoices = await _invoiceService.GetOverdueAsync(companyId);
            return Ok(invoices);
        }
        
        /// <summary>
        /// Get invoice summary/dashboard
        /// </summary>
        [HttpGet("company/{companyId}/summary")]
        public async Task<ActionResult<InvoiceSummary>> GetSummary(
            Guid companyId, 
            [FromQuery] DateTime? fromDate, 
            [FromQuery] DateTime? toDate)
        {
            var from = fromDate ?? DateTime.UtcNow.AddMonths(-1);
            var to = toDate ?? DateTime.UtcNow;
            var summary = await _invoiceService.GetSummaryAsync(companyId, from, to);
            return Ok(summary);
        }
    }
    
    public class SendInvoiceRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}
