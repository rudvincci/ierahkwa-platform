using System.Security.Cryptography;
using IERAHKWA.Platform.Models;

namespace IERAHKWA.Platform.Services;

public interface IPaymentService
{
    Task<Payment> CreatePaymentAsync(PaymentRequest request);
    Task<Payment> ProcessPaymentAsync(string id);
    Task<string> GetStatusAsync(string id);
    Task<Payment> ConfirmPaymentAsync(string id, ConfirmPaymentRequest request);
    Task<Payment> RefundAsync(string id, RefundRequest request);
    Task ProcessWebhookAsync(WebhookPayload payload);
    Task<QRCodeData> GenerateQRAsync(string paymentId);
    Task<List<PaymentMethod>> GetPaymentMethodsAsync();
    Task<Invoice> CreateInvoiceAsync(InvoiceRequest request);
    Task<RecurringPayment> CreateRecurringAsync(RecurringPaymentRequest request);
}

public class PaymentService : IPaymentService
{
    private readonly ILogger<PaymentService> _logger;
    private readonly ITransactionService _transactionService;
    private static readonly List<Payment> _payments = new();
    private static readonly List<Invoice> _invoices = new();
    private static readonly List<RecurringPayment> _recurringPayments = new();
    private static readonly object _lock = new();
    private static int _invoiceCounter = 1000;

    public PaymentService(ILogger<PaymentService> logger, ITransactionService transactionService)
    {
        _logger = logger;
        _transactionService = transactionService;
    }

    public async Task<Payment> CreatePaymentAsync(PaymentRequest request)
    {
        await Task.Delay(100);

        var payment = new Payment
        {
            Id = $"pay_{Guid.NewGuid().ToString("N")[..8]}",
            WalletId = request.WalletId,
            Amount = request.Amount,
            Currency = request.Currency,
            Status = "pending",
            Description = request.Description,
            PaymentAddress = GeneratePaymentAddress(),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(request.ExpiresInMinutes),
            Metadata = request.Metadata
        };

        lock (_lock)
        {
            _payments.Add(payment);
        }

        _logger.LogInformation("Payment created: {PaymentId} - {Amount} {Currency}",
            payment.Id, payment.Amount, payment.Currency);

        return payment;
    }

    public async Task<Payment> ProcessPaymentAsync(string id)
    {
        await Task.Delay(200); // Simular procesamiento

        Payment? payment;
        lock (_lock)
        {
            payment = _payments.FirstOrDefault(p => p.Id == id);
        }

        if (payment == null)
            throw new ArgumentException("Payment not found");

        if (payment.Status != "pending")
            throw new InvalidOperationException($"Cannot process payment with status: {payment.Status}");

        if (DateTime.UtcNow > payment.ExpiresAt)
        {
            lock (_lock)
            {
                payment.Status = "expired";
            }
            throw new InvalidOperationException("Payment has expired");
        }

        // Procesar pago
        lock (_lock)
        {
            payment.Status = "processing";
        }

        // Simular confirmación
        _ = SimulatePaymentConfirmation(id);

        return payment;
    }

    public async Task<string> GetStatusAsync(string id)
    {
        await Task.Delay(50);

        Payment? payment;
        lock (_lock)
        {
            payment = _payments.FirstOrDefault(p => p.Id == id);
        }

        return payment?.Status ?? "not_found";
    }

    public async Task<Payment> ConfirmPaymentAsync(string id, ConfirmPaymentRequest request)
    {
        await Task.Delay(100);

        Payment? payment;
        lock (_lock)
        {
            payment = _payments.FirstOrDefault(p => p.Id == id);
        }

        if (payment == null)
            throw new ArgumentException("Payment not found");

        lock (_lock)
        {
            payment.Status = "completed";
            payment.TxHash = request.TxHash;
            payment.PaidAt = DateTime.UtcNow;
        }

        _logger.LogInformation("Payment confirmed: {PaymentId}", id);

        return payment;
    }

    public async Task<Payment> RefundAsync(string id, RefundRequest request)
    {
        await Task.Delay(150);

        Payment? payment;
        lock (_lock)
        {
            payment = _payments.FirstOrDefault(p => p.Id == id);
        }

        if (payment == null)
            throw new ArgumentException("Payment not found");

        if (payment.Status != "completed")
            throw new InvalidOperationException("Only completed payments can be refunded");

        var refundAmount = request.Amount ?? payment.Amount;

        // Crear transacción de reembolso
        await _transactionService.CreateTransactionAsync(new TransactionRequest
        {
            FromWallet = "system_refund",
            ToWallet = payment.WalletId,
            Amount = refundAmount,
            Currency = payment.Currency,
            Type = "refund",
            Memo = $"Refund for payment {payment.Id}: {request.Reason}"
        });

        lock (_lock)
        {
            payment.Status = "refunded";
        }

        _logger.LogInformation("Payment refunded: {PaymentId} - Amount: {Amount}",
            id, refundAmount);

        return payment;
    }

    public async Task ProcessWebhookAsync(WebhookPayload payload)
    {
        await Task.Delay(50);

        _logger.LogInformation("Webhook received: {Event} for payment {PaymentId}",
            payload.Event, payload.PaymentId);

        // Validar firma (en producción)
        // if (!ValidateSignature(payload)) throw new UnauthorizedAccessException();

        Payment? payment;
        lock (_lock)
        {
            payment = _payments.FirstOrDefault(p => p.Id == payload.PaymentId);
        }

        if (payment == null) return;

        lock (_lock)
        {
            switch (payload.Event)
            {
                case "payment.confirmed":
                    payment.Status = "completed";
                    payment.TxHash = payload.TxHash;
                    payment.PaidAt = DateTime.UtcNow;
                    break;
                case "payment.failed":
                    payment.Status = "failed";
                    break;
                case "payment.expired":
                    payment.Status = "expired";
                    break;
            }
        }
    }

    public async Task<QRCodeData> GenerateQRAsync(string paymentId)
    {
        await Task.Delay(100);

        Payment? payment;
        lock (_lock)
        {
            payment = _payments.FirstOrDefault(p => p.Id == paymentId);
        }

        if (payment == null)
            throw new ArgumentException("Payment not found");

        // Generar datos para QR
        var qrData = new QRCodeData
        {
            PaymentId = payment.Id,
            Address = payment.PaymentAddress ?? "",
            Amount = payment.Amount,
            Currency = payment.Currency,
            // En producción, generar imagen QR real con QRCoder o similar
            QrImage = GeneratePlaceholderQR(payment.PaymentAddress ?? "", payment.Amount, payment.Currency),
            DeepLink = $"ierahkwa://pay?address={payment.PaymentAddress}&amount={payment.Amount}&currency={payment.Currency}"
        };

        return qrData;
    }

    public async Task<List<PaymentMethod>> GetPaymentMethodsAsync()
    {
        await Task.Delay(50);

        return new List<PaymentMethod>
        {
            new()
            {
                Id = "igt",
                Name = "IGT Token",
                Type = "crypto",
                Currencies = new[] { "IGT", "IGT-GOV", "IGT-PM" },
                MinAmount = 1,
                MaxAmount = 10000000,
                Fee = 0.001m,
                IsActive = true
            },
            new()
            {
                Id = "usdt",
                Name = "USDT (Tether)",
                Type = "crypto",
                Currencies = new[] { "USDT" },
                MinAmount = 1,
                MaxAmount = 1000000,
                Fee = 0.002m,
                IsActive = true
            },
            new()
            {
                Id = "eth",
                Name = "Ethereum",
                Type = "crypto",
                Currencies = new[] { "ETH" },
                MinAmount = 0.001m,
                MaxAmount = 10000,
                Fee = 0.003m,
                IsActive = true
            },
            new()
            {
                Id = "btc",
                Name = "Bitcoin",
                Type = "crypto",
                Currencies = new[] { "BTC" },
                MinAmount = 0.0001m,
                MaxAmount = 1000,
                Fee = 0.002m,
                IsActive = true
            },
            new()
            {
                Id = "swift",
                Name = "SWIFT Transfer",
                Type = "fiat",
                Currencies = new[] { "USD", "EUR", "GBP" },
                MinAmount = 100,
                MaxAmount = 10000000,
                Fee = 25,
                IsActive = true
            },
            new()
            {
                Id = "card",
                Name = "Credit/Debit Card",
                Type = "card",
                Currencies = new[] { "USD", "EUR" },
                MinAmount = 10,
                MaxAmount = 50000,
                Fee = 0.029m,
                IsActive = true
            }
        };
    }

    public async Task<Invoice> CreateInvoiceAsync(InvoiceRequest request)
    {
        await Task.Delay(100);

        var invoiceNumber = $"INV-{DateTime.UtcNow:yyyyMM}-{Interlocked.Increment(ref _invoiceCounter)}";

        var invoice = new Invoice
        {
            Id = $"inv_{Guid.NewGuid().ToString("N")[..8]}",
            InvoiceNumber = invoiceNumber,
            CustomerEmail = request.CustomerEmail,
            CustomerName = request.CustomerName,
            Amount = request.Items?.Sum(i => i.Total) ?? request.Amount,
            Currency = request.Currency,
            Status = "draft",
            Description = request.Description,
            Items = request.Items,
            PaymentUrl = $"https://pay.ierahkwa.gov/invoice/{invoiceNumber}",
            CreatedAt = DateTime.UtcNow,
            DueDate = request.DueDate ?? DateTime.UtcNow.AddDays(30)
        };

        lock (_lock)
        {
            _invoices.Add(invoice);
        }

        _logger.LogInformation("Invoice created: {InvoiceNumber} - {Amount} {Currency}",
            invoice.InvoiceNumber, invoice.Amount, invoice.Currency);

        return invoice;
    }

    public async Task<RecurringPayment> CreateRecurringAsync(RecurringPaymentRequest request)
    {
        await Task.Delay(100);

        var nextPaymentDate = request.StartDate ?? DateTime.UtcNow;

        var recurring = new RecurringPayment
        {
            Id = $"rec_{Guid.NewGuid().ToString("N")[..8]}",
            WalletId = request.WalletId,
            Amount = request.Amount,
            Currency = request.Currency,
            Interval = request.Interval,
            ToAddress = request.ToAddress,
            Status = "active",
            NextPaymentDate = nextPaymentDate,
            PaymentCount = 0,
            CreatedAt = DateTime.UtcNow
        };

        lock (_lock)
        {
            _recurringPayments.Add(recurring);
        }

        _logger.LogInformation("Recurring payment created: {RecurringId} - {Amount} {Currency} {Interval}",
            recurring.Id, recurring.Amount, recurring.Currency, recurring.Interval);

        return recurring;
    }

    private async Task SimulatePaymentConfirmation(string paymentId)
    {
        await Task.Delay(5000); // 5 segundos para "confirmar"

        lock (_lock)
        {
            var payment = _payments.FirstOrDefault(p => p.Id == paymentId);
            if (payment != null && payment.Status == "processing")
            {
                payment.Status = "completed";
                payment.TxHash = "0x" + Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLower();
                payment.PaidAt = DateTime.UtcNow;
            }
        }
    }

    private static string GeneratePaymentAddress()
    {
        var bytes = RandomNumberGenerator.GetBytes(20);
        return "0x" + Convert.ToHexString(bytes).ToLower();
    }

    private static string GeneratePlaceholderQR(string address, decimal amount, string currency)
    {
        // En producción, usar QRCoder para generar imagen real
        // Por ahora, retornar un placeholder base64
        return "data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIyMDAiIGhlaWdodD0iMjAwIj48cmVjdCB3aWR0aD0iMjAwIiBoZWlnaHQ9IjIwMCIgZmlsbD0iI2ZmZiIvPjx0ZXh0IHg9IjUwJSIgeT0iNTAlIiB0ZXh0LWFuY2hvcj0ibWlkZGxlIiBmb250LXNpemU9IjE0Ij5RUiBDb2RlPC90ZXh0Pjwvc3ZnPg==";
    }
}
