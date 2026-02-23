using Microsoft.AspNetCore.Mvc;
using IERAHKWA.Platform.Services;
using IERAHKWA.Platform.Models;

namespace IERAHKWA.Platform.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;
    private readonly ILogger<CheckoutController> _logger;

    public CheckoutController(ICheckoutService checkoutService, ILogger<CheckoutController> logger)
    {
        _checkoutService = checkoutService;
        _logger = logger;
    }

    /// <summary>
    /// Crear sesión de checkout
    /// </summary>
    [HttpPost("session/create")]
    public async Task<IActionResult> CreateSession([FromBody] CheckoutSessionRequest request)
    {
        try
        {
            var session = await _checkoutService.CreateSessionAsync(request);
            return Ok(new { success = true, data = session });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating checkout session");
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener sesión de checkout
    /// </summary>
    [HttpGet("session/{id}")]
    public async Task<IActionResult> GetSession(string id)
    {
        var session = await _checkoutService.GetSessionAsync(id);
        if (session == null)
            return NotFound(new { success = false, error = "Session not found" });
        
        return Ok(new { success = true, data = session });
    }

    /// <summary>
    /// Procesar pago del checkout
    /// </summary>
    [HttpPost("session/{id}/pay")]
    public async Task<IActionResult> ProcessPayment(string id, [FromBody] CheckoutPaymentRequest request)
    {
        try
        {
            var result = await _checkoutService.ProcessPaymentAsync(id, request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing checkout payment");
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Cancelar sesión de checkout
    /// </summary>
    [HttpPost("session/{id}/cancel")]
    public async Task<IActionResult> CancelSession(string id)
    {
        try
        {
            await _checkoutService.CancelSessionAsync(id);
            return Ok(new { success = true, message = "Session cancelled" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Aplicar cupón de descuento
    /// </summary>
    [HttpPost("session/{id}/coupon")]
    public async Task<IActionResult> ApplyCoupon(string id, [FromBody] CouponRequest request)
    {
        try
        {
            var result = await _checkoutService.ApplyCouponAsync(id, request.CouponCode);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener página de checkout embebida
    /// </summary>
    [HttpGet("embed/{sessionId}")]
    public async Task<IActionResult> GetEmbedPage(string sessionId)
    {
        var session = await _checkoutService.GetSessionAsync(sessionId);
        if (session == null)
            return NotFound("Session not found");

        var html = GenerateCheckoutPage(session);
        return Content(html, "text/html");
    }

    private string GenerateCheckoutPage(CheckoutSession session)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>IERAHKWA Checkout</title>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{ 
            font-family: 'Inter', -apple-system, sans-serif; 
            background: linear-gradient(135deg, #0a0e17 0%, #1a1f35 100%);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 20px;
        }}
        .checkout-container {{
            background: #0d1a2d;
            border: 1px solid #1e3a5f;
            border-radius: 20px;
            padding: 40px;
            max-width: 450px;
            width: 100%;
            color: #fff;
        }}
        .logo {{ text-align: center; margin-bottom: 30px; }}
        .logo h1 {{ 
            background: linear-gradient(90deg, #9D4EDD, #FFD700);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            font-size: 1.8em;
        }}
        .amount {{
            text-align: center;
            margin: 30px 0;
            padding: 25px;
            background: #142238;
            border-radius: 15px;
        }}
        .amount-label {{ color: #718096; font-size: 0.9em; }}
        .amount-value {{ 
            font-size: 3em; 
            font-weight: 700;
            background: linear-gradient(90deg, #fff, #FFD700);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
        }}
        .amount-currency {{ color: #a0aec0; }}
        .description {{ 
            text-align: center; 
            color: #a0aec0; 
            margin-bottom: 30px;
            padding: 15px;
            background: #142238;
            border-radius: 10px;
        }}
        .payment-methods {{ margin: 25px 0; }}
        .method {{
            display: flex;
            align-items: center;
            gap: 15px;
            padding: 15px;
            background: #142238;
            border: 2px solid transparent;
            border-radius: 12px;
            margin-bottom: 10px;
            cursor: pointer;
            transition: all 0.2s;
        }}
        .method:hover, .method.selected {{ border-color: #9D4EDD; }}
        .method-icon {{ 
            width: 45px; 
            height: 45px; 
            background: linear-gradient(135deg, #9D4EDD, #FFD700);
            border-radius: 10px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 20px;
        }}
        .method-name {{ font-weight: 600; }}
        .method-desc {{ font-size: 0.85em; color: #718096; }}
        .btn {{
            width: 100%;
            padding: 18px;
            background: linear-gradient(135deg, #9D4EDD, #7c3aed);
            border: none;
            border-radius: 12px;
            color: #fff;
            font-size: 1.1em;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.2s;
        }}
        .btn:hover {{ transform: translateY(-2px); filter: brightness(1.1); }}
        .btn:disabled {{ opacity: 0.5; cursor: not-allowed; }}
        .secure {{ 
            text-align: center; 
            margin-top: 20px; 
            color: #00FF41;
            font-size: 0.85em;
        }}
        .timer {{
            text-align: center;
            color: #FFD700;
            margin-bottom: 20px;
            font-size: 0.9em;
        }}
    </style>
</head>
<body>
    <div class='checkout-container'>
        <div class='logo'>
            <h1>🏛️ IERAHKWA Pay</h1>
        </div>
        
        <div class='timer'>
            Session expires in: <span id='timer'>30:00</span>
        </div>
        
        <div class='amount'>
            <div class='amount-label'>Amount to Pay</div>
            <div class='amount-value'>{session.Amount:N2}</div>
            <div class='amount-currency'>{session.Currency}</div>
        </div>
        
        <div class='description'>
            {session.Description ?? "Payment"}
        </div>
        
        <div class='payment-methods'>
            <div class='method selected' onclick='selectMethod(this, ""crypto"")'>
                <div class='method-icon'>🪙</div>
                <div>
                    <div class='method-name'>Crypto (IGT/USDT/ETH)</div>
                    <div class='method-desc'>Pay with cryptocurrency</div>
                </div>
            </div>
            <div class='method' onclick='selectMethod(this, ""card"")'>
                <div class='method-icon'>💳</div>
                <div>
                    <div class='method-name'>Credit/Debit Card</div>
                    <div class='method-desc'>Visa, Mastercard, Amex</div>
                </div>
            </div>
            <div class='method' onclick='selectMethod(this, ""bank"")'>
                <div class='method-icon'>🏦</div>
                <div>
                    <div class='method-name'>Bank Transfer</div>
                    <div class='method-desc'>SWIFT / Wire Transfer</div>
                </div>
            </div>
        </div>
        
        <button class='btn' onclick='pay()'>
            Pay {session.Amount:N2} {session.Currency}
        </button>
        
        <div class='secure'>
            🔒 Secured by IERAHKWA Sovereign Banking
        </div>
    </div>
    
    <script>
        let selectedMethod = 'crypto';
        let timeLeft = 1800;
        
        function selectMethod(el, method) {{
            document.querySelectorAll('.method').forEach(m => m.classList.remove('selected'));
            el.classList.add('selected');
            selectedMethod = method;
        }}
        
        async function pay() {{
            const btn = document.querySelector('.btn');
            btn.disabled = true;
            btn.textContent = 'Processing...';
            
            try {{
                const response = await fetch('/api/checkout/session/{session.Id}/pay', {{
                    method: 'POST',
                    headers: {{ 'Content-Type': 'application/json' }},
                    body: JSON.stringify({{ paymentMethod: selectedMethod }})
                }});
                
                const result = await response.json();
                
                if (result.success) {{
                    btn.style.background = 'linear-gradient(135deg, #00FF41, #00cc33)';
                    btn.textContent = '✓ Payment Successful!';
                    
                    if ('{session.SuccessUrl}') {{
                        setTimeout(() => window.location.href = '{session.SuccessUrl}', 2000);
                    }}
                }} else {{
                    throw new Error(result.error);
                }}
            }} catch (error) {{
                btn.style.background = 'linear-gradient(135deg, #ef4444, #dc2626)';
                btn.textContent = 'Payment Failed - Try Again';
                btn.disabled = false;
                setTimeout(() => {{
                    btn.style.background = 'linear-gradient(135deg, #9D4EDD, #7c3aed)';
                    btn.textContent = 'Pay {session.Amount:N2} {session.Currency}';
                }}, 3000);
            }}
        }}
        
        setInterval(() => {{
            timeLeft--;
            const mins = Math.floor(timeLeft / 60);
            const secs = timeLeft % 60;
            document.getElementById('timer').textContent = mins + ':' + (secs < 10 ? '0' : '') + secs;
            
            if (timeLeft <= 0) {{
                window.location.href = '{session.CancelUrl ?? "/"}';
            }}
        }}, 1000);
    </script>
</body>
</html>";
    }
}

public class CouponRequest
{
    public string CouponCode { get; set; } = "";
}
