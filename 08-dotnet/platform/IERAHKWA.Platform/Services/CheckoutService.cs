using System.Security.Cryptography;
using IERAHKWA.Platform.Models;

namespace IERAHKWA.Platform.Services;

public interface ICheckoutService
{
    Task<CheckoutSession> CreateSessionAsync(CheckoutSessionRequest request);
    Task<CheckoutSession?> GetSessionAsync(string id);
    Task<CheckoutResult> ProcessPaymentAsync(string sessionId, CheckoutPaymentRequest request);
    Task CancelSessionAsync(string id);
    Task<CheckoutSession> ApplyCouponAsync(string sessionId, string couponCode);
}

public class CheckoutService : ICheckoutService
{
    private readonly ILogger<CheckoutService> _logger;
    private readonly ITransactionService _transactionService;
    private static readonly List<CheckoutSession> _sessions = new();
    private static readonly List<Coupon> _coupons = new();
    private static readonly object _lock = new();

    public CheckoutService(ILogger<CheckoutService> logger, ITransactionService transactionService)
    {
        _logger = logger;
        _transactionService = transactionService;
        InitializeCoupons();
    }

    private void InitializeCoupons()
    {
        lock (_lock)
        {
            if (_coupons.Count == 0)
            {
                _coupons.AddRange(new[]
                {
                    new Coupon { Code = "WELCOME10", Type = "percent", Value = 10, MaxUses = 1000, IsActive = true },
                    new Coupon { Code = "IERAHKWA20", Type = "percent", Value = 20, MaxUses = 500, IsActive = true },
                    new Coupon { Code = "SAVE50", Type = "fixed", Value = 50, Currency = "USD", MinAmount = 200, IsActive = true },
                    new Coupon { Code = "VIP100", Type = "fixed", Value = 100, Currency = "USD", MinAmount = 500, IsActive = true }
                });
            }
        }
    }

    public async Task<CheckoutSession> CreateSessionAsync(CheckoutSessionRequest request)
    {
        await Task.Delay(100);

        var sessionId = $"cs_{Guid.NewGuid().ToString("N")[..12]}";
        var baseUrl = "http://localhost:3000"; // In production, get from config

        var session = new CheckoutSession
        {
            Id = sessionId,
            MerchantId = "merchant_default",
            Amount = request.Amount,
            Currency = request.Currency,
            Description = request.Description,
            Items = request.Items,
            CustomerEmail = request.CustomerEmail,
            CustomerName = request.CustomerName,
            SuccessUrl = request.SuccessUrl,
            CancelUrl = request.CancelUrl,
            WebhookUrl = request.WebhookUrl,
            Metadata = request.Metadata,
            PaymentUrl = $"{baseUrl}/api/checkout/embed/{sessionId}",
            EmbedUrl = $"{baseUrl}/api/checkout/embed/{sessionId}",
            ExpiresAt = DateTime.UtcNow.AddMinutes(request.ExpiresInMinutes)
        };

        lock (_lock)
        {
            _sessions.Add(session);
        }

        _logger.LogInformation("Checkout session created: {SessionId} - {Amount} {Currency}",
            sessionId, request.Amount, request.Currency);

        return session;
    }

    public async Task<CheckoutSession?> GetSessionAsync(string id)
    {
        await Task.Delay(50);
        lock (_lock)
        {
            var session = _sessions.FirstOrDefault(s => s.Id == id);
            
            // Check if expired
            if (session != null && session.Status == "pending" && DateTime.UtcNow > session.ExpiresAt)
            {
                session.Status = "expired";
            }
            
            return session;
        }
    }

    public async Task<CheckoutResult> ProcessPaymentAsync(string sessionId, CheckoutPaymentRequest request)
    {
        await Task.Delay(200); // Simulate processing time

        CheckoutSession? session;
        lock (_lock)
        {
            session = _sessions.FirstOrDefault(s => s.Id == sessionId);
        }

        if (session == null)
            throw new ArgumentException("Checkout session not found");

        if (session.Status != "pending")
            throw new InvalidOperationException($"Session is not payable. Status: {session.Status}");

        if (DateTime.UtcNow > session.ExpiresAt)
        {
            lock (_lock) { session.Status = "expired"; }
            throw new InvalidOperationException("Session has expired");
        }

        // Update status to processing
        lock (_lock) { session.Status = "processing"; }
        session.PaymentMethod = request.PaymentMethod;

        try
        {
            // Process based on payment method
            string? txHash = null;
            string? transactionId = null;

            switch (request.PaymentMethod.ToLower())
            {
                case "crypto":
                    // Process crypto payment
                    var cryptoTx = await _transactionService.CreateTransactionAsync(new TransactionRequest
                    {
                        FromWallet = request.WalletAddress ?? "external_crypto",
                        ToWallet = "merchant_wallet",
                        Amount = session.Amount,
                        Currency = session.Currency == "USD" ? "USDT" : session.Currency,
                        Type = "payment",
                        Memo = $"Checkout {session.Id}"
                    });
                    txHash = cryptoTx.TxHash;
                    transactionId = cryptoTx.Id;
                    break;

                case "card":
                    // Simulate card processing
                    if (string.IsNullOrEmpty(request.CardNumber) || request.CardNumber.Length < 13)
                        throw new ArgumentException("Invalid card number");
                    
                    transactionId = $"card_{Guid.NewGuid().ToString("N")[..12]}";
                    await Task.Delay(500); // Card processing simulation
                    break;

                case "bank":
                    // Process bank transfer
                    transactionId = $"bank_{Guid.NewGuid().ToString("N")[..12]}";
                    break;

                default:
                    throw new ArgumentException($"Unknown payment method: {request.PaymentMethod}");
            }

            // Mark as completed
            lock (_lock)
            {
                session.Status = "completed";
                session.TransactionId = transactionId;
                session.PaidAt = DateTime.UtcNow;
            }

            // Send webhook if configured
            if (!string.IsNullOrEmpty(session.WebhookUrl))
            {
                _ = SendWebhookAsync(session);
            }

            _logger.LogInformation("Checkout payment completed: {SessionId} via {Method}",
                sessionId, request.PaymentMethod);

            return new CheckoutResult
            {
                SessionId = session.Id,
                Status = "completed",
                TransactionId = transactionId,
                TxHash = txHash,
                AmountPaid = session.Amount,
                Currency = session.Currency,
                PaidAt = session.PaidAt,
                Receipt = GenerateReceipt(session)
            };
        }
        catch (Exception ex)
        {
            lock (_lock) { session.Status = "failed"; }
            _logger.LogError(ex, "Checkout payment failed: {SessionId}", sessionId);
            throw;
        }
    }

    public async Task CancelSessionAsync(string id)
    {
        await Task.Delay(50);

        lock (_lock)
        {
            var session = _sessions.FirstOrDefault(s => s.Id == id);
            if (session == null)
                throw new ArgumentException("Session not found");

            if (session.Status != "pending")
                throw new InvalidOperationException("Only pending sessions can be cancelled");

            session.Status = "cancelled";
        }

        _logger.LogInformation("Checkout session cancelled: {SessionId}", id);
    }

    public async Task<CheckoutSession> ApplyCouponAsync(string sessionId, string couponCode)
    {
        await Task.Delay(50);

        CheckoutSession? session;
        Coupon? coupon;

        lock (_lock)
        {
            session = _sessions.FirstOrDefault(s => s.Id == sessionId);
            coupon = _coupons.FirstOrDefault(c => c.Code.Equals(couponCode, StringComparison.OrdinalIgnoreCase));
        }

        if (session == null)
            throw new ArgumentException("Session not found");

        if (coupon == null || !coupon.IsActive)
            throw new ArgumentException("Invalid or expired coupon code");

        if (coupon.MaxUses.HasValue && coupon.UsedCount >= coupon.MaxUses)
            throw new ArgumentException("Coupon has reached maximum uses");

        if (coupon.MinAmount.HasValue && session.Amount < coupon.MinAmount)
            throw new ArgumentException($"Minimum amount for this coupon is {coupon.MinAmount} {coupon.Currency ?? session.Currency}");

        // Calculate discount
        decimal discount;
        if (coupon.Type == "percent")
        {
            discount = session.Amount * (coupon.Value / 100);
            if (coupon.MaxDiscount.HasValue && discount > coupon.MaxDiscount)
                discount = coupon.MaxDiscount.Value;
        }
        else
        {
            discount = coupon.Value;
        }

        lock (_lock)
        {
            session.Amount -= discount;
            if (session.Amount < 0) session.Amount = 0;
            
            session.Metadata ??= new Dictionary<string, string>();
            session.Metadata["coupon"] = couponCode;
            session.Metadata["discount"] = discount.ToString("F2");

            coupon.UsedCount++;
        }

        _logger.LogInformation("Coupon applied: {CouponCode} to session {SessionId}, discount: {Discount}",
            couponCode, sessionId, discount);

        return session;
    }

    private async Task SendWebhookAsync(CheckoutSession session)
    {
        try
        {
            using var client = new HttpClient();
            var payload = new
            {
                @event = "checkout.completed",
                session_id = session.Id,
                amount = session.Amount,
                currency = session.Currency,
                status = session.Status,
                transaction_id = session.TransactionId,
                paid_at = session.PaidAt,
                metadata = session.Metadata
            };

            // In production, add signature header
            await client.PostAsJsonAsync(session.WebhookUrl, payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send webhook for session {SessionId}", session.Id);
        }
    }

    private string GenerateReceipt(CheckoutSession session)
    {
        return $@"
═══════════════════════════════════════════
           IERAHKWA PAY - RECEIPT
═══════════════════════════════════════════

Transaction ID: {session.TransactionId}
Date: {session.PaidAt:yyyy-MM-dd HH:mm:ss} UTC

───────────────────────────────────────────
{session.Description ?? "Payment"}
───────────────────────────────────────────

Amount Paid: {session.Amount:N2} {session.Currency}
Payment Method: {session.PaymentMethod?.ToUpper()}
Status: COMPLETED ✓

───────────────────────────────────────────
Thank you for your payment!
BDET Central Bank - SWIFT: IERBDETXXX
═══════════════════════════════════════════
";
    }
}
