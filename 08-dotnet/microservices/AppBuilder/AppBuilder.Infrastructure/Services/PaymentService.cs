using AppBuilder.Core.Interfaces;
using AppBuilder.Core.Models;
using Microsoft.Extensions.Logging;

namespace AppBuilder.Infrastructure.Services;

/// <summary>PayPal, Bank Transfer. IERAHKWA Appy: accept payments from day one.</summary>
public class PaymentService : IPaymentService
{
    private static readonly List<Invoice> _invoices = new();
    private static int _invoiceSeq;
    private static readonly object _lock = new();
    private readonly ISubscriptionService _subs;
    private readonly ILogger<PaymentService> _log;

    public PaymentService(ISubscriptionService subs, ILogger<PaymentService> log)
    {
        _subs = subs;
        _log = log;
    }

    public string CreatePayPalSubscription(string userId, string planId, string returnUrl, string cancelUrl)
    {
        var id = "px_" + Guid.NewGuid().ToString("N")[..16];
        return returnUrl + $"?mock=1&paymentId={id}&planId={planId}&userId={userId}";
    }

    public (bool Ok, string? Error) ProcessPayPalCallback(string paymentId, string userId, string planId)
    {
        lock (_lock)
        {
            _subs.Subscribe(userId, planId, PaymentMethod.PayPal);
            var inv = new Invoice
            {
                Id = Guid.NewGuid().ToString(),
                Number = "INV-" + DateTime.UtcNow.Year + "-" + (++_invoiceSeq).ToString("D5"),
                UserId = userId,
                Amount = _subs.GetPlanById(planId)?.PriceMonthly ?? 0,
                Currency = "USD",
                PaymentMethod = PaymentMethod.PayPal,
                Status = PaymentStatus.Paid,
                PayPalTransactionId = paymentId,
                Description = "Subscription – PayPal",
                PaidAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            _invoices.Add(inv);
            _log.LogInformation("IERAHKWA Appy: PayPal callback processed {PaymentId}", paymentId);
            return (true, null);
        }
    }

    public Invoice CreateBankTransferInvoice(string userId, decimal amount, string description)
    {
        lock (_lock)
        {
            var inv = new Invoice
            {
                Id = Guid.NewGuid().ToString(),
                Number = "INV-" + DateTime.UtcNow.Year + "-" + (++_invoiceSeq).ToString("D5"),
                UserId = userId,
                Amount = amount,
                Currency = "USD",
                PaymentMethod = PaymentMethod.BankTransfer,
                Status = PaymentStatus.Pending,
                Description = description,
                DueDate = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };
            _invoices.Add(inv);
            return inv;
        }
    }

    public Invoice? GetInvoice(string id)
    {
        lock (_lock) return _invoices.FirstOrDefault(i => i.Id == id);
    }

    public IReadOnlyList<Invoice> GetUserInvoices(string userId)
    {
        lock (_lock) return _invoices.Where(i => i.UserId == userId).OrderByDescending(i => i.CreatedAt).ToList();
    }
}
