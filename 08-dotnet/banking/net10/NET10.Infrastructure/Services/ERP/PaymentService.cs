using NET10.Core.Interfaces;
using NET10.Core.Models.ERP;

namespace NET10.Infrastructure.Services.ERP;

/// <summary>
/// Payment Service - Record and manage customer payments
/// </summary>
public class PaymentService : IPaymentService
{
    private static readonly List<Payment> _payments = [];
    private static readonly Dictionary<Guid, int> _paymentCounters = [];
    private readonly IInvoiceService _invoiceService;
    
    public PaymentService(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }
    
    public Task<List<Payment>> GetAllAsync(Guid companyId)
    {
        var payments = _payments.Where(p => p.CompanyId == companyId)
                                .OrderByDescending(p => p.PaymentDate)
                                .ToList();
        return Task.FromResult(payments);
    }
    
    public Task<Payment?> GetByIdAsync(Guid id)
    {
        var payment = _payments.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(payment);
    }
    
    public async Task<Payment> CreateAsync(Payment payment)
    {
        payment.Id = Guid.NewGuid();
        payment.CreatedAt = DateTime.UtcNow;
        
        if (string.IsNullOrEmpty(payment.PaymentNumber))
        {
            payment.PaymentNumber = await GeneratePaymentNumberAsync(payment.CompanyId);
        }
        
        _payments.Add(payment);
        return payment;
    }
    
    public async Task<Payment> ApplyToInvoiceAsync(Guid paymentId, Guid invoiceId, decimal amount)
    {
        var payment = _payments.FirstOrDefault(p => p.Id == paymentId);
        var invoice = await _invoiceService.GetByIdAsync(invoiceId);
        
        if (payment != null && invoice != null)
        {
            // Apply amount to invoice
            var appliedTotal = payment.Allocations.Sum(a => a.Amount);
            var unappliedAmount = payment.Amount - appliedTotal;
            var applicationAmount = Math.Min(amount, Math.Min(unappliedAmount, invoice.Balance));
            
            payment.Allocations.Add(new PaymentAllocation
            {
                InvoiceId = invoiceId,
                InvoiceNumber = invoice.InvoiceNumber,
                Amount = applicationAmount
            });
            
            // Update invoice
            invoice.AmountPaid += applicationAmount;
            if (invoice.AmountPaid >= invoice.Total)
            {
                invoice.PaidAt = DateTime.UtcNow;
            }
            await _invoiceService.UpdateAsync(invoice);
        }
        
        return payment!;
    }
    
    public Task<List<Payment>> GetByCustomerAsync(Guid customerId)
    {
        var payments = _payments.Where(p => p.CustomerId == customerId)
                                .OrderByDescending(p => p.PaymentDate)
                                .ToList();
        return Task.FromResult(payments);
    }
    
    public Task<string> GeneratePaymentNumberAsync(Guid companyId)
    {
        if (!_paymentCounters.ContainsKey(companyId))
        {
            _paymentCounters[companyId] = 1000;
        }
        
        _paymentCounters[companyId]++;
        var number = $"PMT-{_paymentCounters[companyId]:D6}";
        return Task.FromResult(number);
    }
}
