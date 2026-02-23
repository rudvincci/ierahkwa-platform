using AppBuilder.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AppBuilder.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _pay;
    private readonly ILogger<PaymentsController> _log;

    public PaymentsController(IPaymentService pay, ILogger<PaymentsController> log)
    {
        _pay = pay;
        _log = log;
    }

    [HttpPost("paypal-callback")]
    public IActionResult PayPalCallback([FromQuery] string paymentId, [FromQuery] string userId, [FromQuery] string planId)
    {
        if (string.IsNullOrEmpty(paymentId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(planId))
            return BadRequest(new { error = "paymentId, userId, planId required" });
        var (ok, err) = _pay.ProcessPayPalCallback(paymentId, userId, planId);
        if (!ok) return BadRequest(new { error = err ?? "Callback failed" });
        return Ok(new { success = true, message = "Subscription activated" });
    }
}
