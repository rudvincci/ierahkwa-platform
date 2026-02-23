using Microsoft.AspNetCore.Mvc;
using IERAHKWA.Platform.Services;
using IERAHKWA.Platform.Models;

namespace IERAHKWA.Platform.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    /// <summary>
    /// Crear orden de pago
    /// </summary>
    [HttpPost("create")]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
    {
        try
        {
            var payment = await _paymentService.CreatePaymentAsync(request);
            return Ok(new { success = true, data = payment });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment");
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Procesar pago
    /// </summary>
    [HttpPost("process/{id}")]
    public async Task<IActionResult> ProcessPayment(string id)
    {
        try
        {
            var result = await _paymentService.ProcessPaymentAsync(id);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment");
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Verificar estado del pago
    /// </summary>
    [HttpGet("{id}/status")]
    public async Task<IActionResult> GetPaymentStatus(string id)
    {
        var status = await _paymentService.GetStatusAsync(id);
        return Ok(new { success = true, data = status });
    }

    /// <summary>
    /// Confirmar pago
    /// </summary>
    [HttpPost("{id}/confirm")]
    public async Task<IActionResult> ConfirmPayment(string id, [FromBody] ConfirmPaymentRequest request)
    {
        try
        {
            var result = await _paymentService.ConfirmPaymentAsync(id, request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Reembolsar pago
    /// </summary>
    [HttpPost("{id}/refund")]
    public async Task<IActionResult> RefundPayment(string id, [FromBody] RefundRequest request)
    {
        try
        {
            var result = await _paymentService.RefundAsync(id, request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Webhook para notificaciones de pago
    /// </summary>
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] WebhookPayload payload)
    {
        try
        {
            await _paymentService.ProcessWebhookAsync(payload);
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook");
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Generar código QR para pago
    /// </summary>
    [HttpGet("{id}/qr")]
    public async Task<IActionResult> GenerateQR(string id)
    {
        var qrCode = await _paymentService.GenerateQRAsync(id);
        return Ok(new { success = true, data = qrCode });
    }

    /// <summary>
    /// Obtener métodos de pago disponibles
    /// </summary>
    [HttpGet("methods")]
    public async Task<IActionResult> GetPaymentMethods()
    {
        var methods = await _paymentService.GetPaymentMethodsAsync();
        return Ok(new { success = true, data = methods });
    }

    /// <summary>
    /// Crear factura
    /// </summary>
    [HttpPost("invoice")]
    public async Task<IActionResult> CreateInvoice([FromBody] InvoiceRequest request)
    {
        try
        {
            var invoice = await _paymentService.CreateInvoiceAsync(request);
            return Ok(new { success = true, data = invoice });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Pago recurrente
    /// </summary>
    [HttpPost("recurring")]
    public async Task<IActionResult> CreateRecurringPayment([FromBody] RecurringPaymentRequest request)
    {
        try
        {
            var result = await _paymentService.CreateRecurringAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }
}
