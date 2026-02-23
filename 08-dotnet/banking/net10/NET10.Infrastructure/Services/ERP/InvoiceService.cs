using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NET10.Core.Interfaces;
using NET10.Core.Models.ERP;

namespace NET10.Infrastructure.Services.ERP
{
    public class InvoiceService : IInvoiceService
    {
        private static readonly List<Invoice> _invoices = new();
        private static readonly Dictionary<Guid, int> _invoiceCounters = new();
        
        public Task<List<Invoice>> GetAllAsync(Guid companyId)
        {
            var invoices = _invoices.Where(i => i.CompanyId == companyId)
                                    .OrderByDescending(i => i.InvoiceDate)
                                    .ToList();
            return Task.FromResult(invoices);
        }
        
        public Task<Invoice?> GetByIdAsync(Guid id)
        {
            var invoice = _invoices.FirstOrDefault(i => i.Id == id);
            return Task.FromResult(invoice);
        }
        
        public Task<Invoice?> GetByNumberAsync(Guid companyId, string invoiceNumber)
        {
            var invoice = _invoices.FirstOrDefault(i => 
                i.CompanyId == companyId && 
                i.InvoiceNumber.Equals(invoiceNumber, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(invoice);
        }
        
        public async Task<Invoice> CreateAsync(Invoice invoice)
        {
            if (string.IsNullOrEmpty(invoice.InvoiceNumber))
            {
                invoice.InvoiceNumber = await GenerateInvoiceNumberAsync(invoice.CompanyId);
            }
            
            invoice.Id = Guid.NewGuid();
            invoice.CreatedAt = DateTime.UtcNow;
            
            // Calculate tax details
            invoice.TaxDetails = CalculateTaxDetails(invoice.Items);
            
            _invoices.Add(invoice);
            return invoice;
        }
        
        public Task<Invoice> UpdateAsync(Invoice invoice)
        {
            var index = _invoices.FindIndex(i => i.Id == invoice.Id);
            if (index >= 0)
            {
                invoice.TaxDetails = CalculateTaxDetails(invoice.Items);
                _invoices[index] = invoice;
            }
            return Task.FromResult(invoice);
        }
        
        public Task<bool> DeleteAsync(Guid id)
        {
            var invoice = _invoices.FirstOrDefault(i => i.Id == id);
            if (invoice != null)
            {
                _invoices.Remove(invoice);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        
        public Task<string> GenerateInvoiceNumberAsync(Guid companyId)
        {
            if (!_invoiceCounters.ContainsKey(companyId))
            {
                _invoiceCounters[companyId] = 1000;
            }
            
            _invoiceCounters[companyId]++;
            var number = $"INV-{_invoiceCounters[companyId]:D6}";
            return Task.FromResult(number);
        }
        
        public Task<Invoice> SendInvoiceAsync(Guid invoiceId, string email)
        {
            var invoice = _invoices.FirstOrDefault(i => i.Id == invoiceId);
            if (invoice != null)
            {
                invoice.Status = InvoiceStatus.Sent;
                invoice.SentAt = DateTime.UtcNow;
                // TODO: Send email
            }
            return Task.FromResult(invoice!);
        }
        
        public Task<List<Invoice>> GetByCustomerAsync(Guid customerId)
        {
            var invoices = _invoices.Where(i => i.CustomerId == customerId)
                                    .OrderByDescending(i => i.InvoiceDate)
                                    .ToList();
            return Task.FromResult(invoices);
        }
        
        public Task<List<Invoice>> GetOverdueAsync(Guid companyId)
        {
            var today = DateTime.UtcNow.Date;
            var invoices = _invoices.Where(i => 
                i.CompanyId == companyId && 
                i.DueDate < today && 
                i.Balance > 0)
                .OrderBy(i => i.DueDate)
                .ToList();
            return Task.FromResult(invoices);
        }
        
        public Task<InvoiceSummary> GetSummaryAsync(Guid companyId, DateTime fromDate, DateTime toDate)
        {
            var invoices = _invoices.Where(i => 
                i.CompanyId == companyId && 
                i.InvoiceDate >= fromDate && 
                i.InvoiceDate <= toDate)
                .ToList();
            
            var today = DateTime.UtcNow.Date;
            
            var summary = new InvoiceSummary
            {
                TotalInvoices = invoices.Count,
                TotalAmount = invoices.Sum(i => i.Total),
                TotalPaid = invoices.Sum(i => i.AmountPaid),
                TotalOutstanding = invoices.Sum(i => i.Balance),
                PaidCount = invoices.Count(i => i.Balance <= 0),
                UnpaidCount = invoices.Count(i => i.Balance > 0 && i.DueDate >= today),
                OverdueCount = invoices.Count(i => i.Balance > 0 && i.DueDate < today)
            };
            
            return Task.FromResult(summary);
        }
        
        private List<TaxDetail> CalculateTaxDetails(List<InvoiceItem> items)
        {
            return items
                .Where(i => i.IsTaxable)
                .GroupBy(i => new { i.TaxCode, i.TaxRate })
                .Select(g => new TaxDetail
                {
                    TaxCode = g.Key.TaxCode,
                    TaxName = g.Key.TaxCode,
                    Rate = g.Key.TaxRate,
                    TaxableAmount = g.Sum(i => i.LineTotal),
                    TaxAmount = g.Sum(i => i.TaxAmount)
                })
                .ToList();
        }
    }
}
