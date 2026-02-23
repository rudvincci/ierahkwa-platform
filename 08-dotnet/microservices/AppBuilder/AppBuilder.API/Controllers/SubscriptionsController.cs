using System.Security.Claims;
using AppBuilder.Core.Interfaces;
using AppBuilder.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBuilder.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subs;
    private readonly IPaymentService _pay;
    private readonly IAuthService _auth;
    private readonly ILogger<SubscriptionsController> _log;

    public SubscriptionsController(ISubscriptionService subs, IPaymentService pay, IAuthService auth, ILogger<SubscriptionsController> log)
    {
        _subs = subs;
        _pay = pay;
        _auth = auth;
        _log = log;
    }

    private string? UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    [HttpGet("plans")]
    public IActionResult GetPlans() => Ok(_subs.GetPlans());

    [HttpGet("my")]
    [Authorize]
    public IActionResult My()
    {
        var id = UserId!;
        var sub = _subs.GetUserSubscription(id);
        var plan = sub != null ? _subs.GetPlans().FirstOrDefault(p => p.Id == sub.PlanId) : _subs.GetPlanByTier(PlanTier.Free);
        var u = _auth.GetUserById(id);
        return Ok(new { subscription = sub, plan, buildCredits = u?.BuildCredits ?? 0, hasCredits = _subs.HasBuildCredits(id) });
    }

    [HttpPost("subscribe")]
    [Authorize]
    public IActionResult Subscribe([FromBody] SubscribeRequest r, [FromQuery] string? returnUrl, [FromQuery] string? cancelUrl)
    {
        var id = UserId!;
        var plan = _subs.GetPlanById(r.PlanId ?? "");
        if (plan == null) return BadRequest(new { error = "Plan not found" });
        if (plan.Tier == PlanTier.Free) return BadRequest(new { error = "Use plans endpoint to see Free tier; no subscription needed" });

        if (r.PaymentMethod == PaymentMethod.PayPal)
        {
            var url = _pay.CreatePayPalSubscription(id, r.PlanId!, returnUrl ?? "/dashboard", cancelUrl ?? "/pricing");
            return Ok(new { redirectUrl = url, message = "Redirect user to PayPal. On return, call POST /api/payments/paypal-callback with query paymentId, userId, planId." });
        }
        if (r.PaymentMethod == PaymentMethod.BankTransfer)
        {
            var inv = _pay.CreateBankTransferInvoice(id, plan.PriceMonthly, $"Subscription {plan.Name} – Bank Transfer");
            return Ok(new { invoiceId = inv.Id, invoiceNumber = inv.Number, amount = inv.Amount, currency = inv.Currency, message = "Pay via bank transfer. Reference: " + inv.Number });
        }
        return BadRequest(new { error = "Unsupported payment method" });
    }

    [HttpGet("build-credits")]
    [Authorize]
    public IActionResult BuildCredits()
    {
        var id = UserId!;
        var u = _auth.GetUserById(id);
        return Ok(new { buildCredits = u?.BuildCredits ?? 0, hasCredits = _subs.HasBuildCredits(id) });
    }
}

public class SubscribeRequest { public string? PlanId { get; set; } public PaymentMethod PaymentMethod { get; set; } }
