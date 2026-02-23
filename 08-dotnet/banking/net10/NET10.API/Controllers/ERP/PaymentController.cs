using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;
using NET10.Core.Models.ERP;

namespace NET10.API.Controllers.ERP;

/// <summary>
/// Payment Controller - Record and manage payments
/// </summary>
[ApiController]
[Route("api/erp/[controller]")]
[Produces("application/json")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    
    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
    
    /// <summary>
    /// Get all payments for a company
    /// </summary>
    [HttpGet("{companyId}")]
    public async Task<ActionResult<List<Payment>>> GetAll(Guid companyId)
    {
        var payments = await _paymentService.GetAllAsync(companyId);
        return Ok(payments);
    }
    
    /// <summary>
    /// Get payment by ID
    /// </summary>
    [HttpGet("detail/{id}")]
    public async Task<ActionResult<Payment>> GetById(Guid id)
    {
        var payment = await _paymentService.GetByIdAsync(id);
        if (payment == null) return NotFound();
        return Ok(payment);
    }
    
    /// <summary>
    /// Get payments by customer
    /// </summary>
    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<List<Payment>>> GetByCustomer(Guid customerId)
    {
        var payments = await _paymentService.GetByCustomerAsync(customerId);
        return Ok(payments);
    }
    
    /// <summary>
    /// Create a new payment
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Payment>> Create([FromBody] Payment payment)
    {
        var created = await _paymentService.CreateAsync(payment);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
    
    /// <summary>
    /// Apply payment to an invoice
    /// </summary>
    [HttpPost("{paymentId}/apply/{invoiceId}")]
    public async Task<ActionResult<Payment>> ApplyToInvoice(Guid paymentId, Guid invoiceId, [FromQuery] decimal amount)
    {
        var payment = await _paymentService.ApplyToInvoiceAsync(paymentId, invoiceId, amount);
        return Ok(payment);
    }
    
    /// <summary>
    /// Generate next payment number
    /// </summary>
    [HttpGet("next-number/{companyId}")]
    public async Task<ActionResult<string>> GetNextNumber(Guid companyId)
    {
        var number = await _paymentService.GeneratePaymentNumberAsync(companyId);
        return Ok(new { paymentNumber = number });
    }
}
