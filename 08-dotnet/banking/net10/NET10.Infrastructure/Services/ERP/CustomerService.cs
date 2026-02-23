using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NET10.Core.Interfaces;
using NET10.Core.Models.ERP;

namespace NET10.Infrastructure.Services.ERP
{
    public class CustomerService : ICustomerService
    {
        private static readonly List<Customer> _customers = new();
        private static int _customerCounter = 1000;
        
        public Task<List<Customer>> GetAllAsync(Guid companyId)
        {
            var customers = _customers.Where(c => c.CompanyId == companyId)
                                      .OrderBy(c => c.Name)
                                      .ToList();
            return Task.FromResult(customers);
        }
        
        public Task<Customer?> GetByIdAsync(Guid id)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == id);
            return Task.FromResult(customer);
        }
        
        public Task<Customer> CreateAsync(Customer customer)
        {
            customer.Id = Guid.NewGuid();
            customer.Code = $"CUST-{++_customerCounter:D5}";
            customer.CreatedAt = DateTime.UtcNow;
            _customers.Add(customer);
            return Task.FromResult(customer);
        }
        
        public Task<Customer> UpdateAsync(Customer customer)
        {
            var index = _customers.FindIndex(c => c.Id == customer.Id);
            if (index >= 0)
            {
                _customers[index] = customer;
            }
            return Task.FromResult(customer);
        }
        
        public Task<bool> DeleteAsync(Guid id)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == id);
            if (customer != null)
            {
                _customers.Remove(customer);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        
        public Task<List<Customer>> SearchAsync(Guid companyId, string searchTerm)
        {
            var term = searchTerm.ToLower();
            var customers = _customers.Where(c => 
                c.CompanyId == companyId &&
                (c.Name.ToLower().Contains(term) ||
                 c.Code.ToLower().Contains(term) ||
                 c.Email.ToLower().Contains(term) ||
                 c.Phone.Contains(term)))
                .OrderBy(c => c.Name)
                .ToList();
            return Task.FromResult(customers);
        }
        
        public Task<CustomerStatement> GetStatementAsync(Guid customerId, DateTime fromDate, DateTime toDate)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == customerId);
            if (customer == null)
            {
                return Task.FromResult(new CustomerStatement());
            }
            
            var statement = new CustomerStatement
            {
                Customer = customer,
                FromDate = fromDate,
                ToDate = toDate,
                OpeningBalance = 0,
                Lines = new List<StatementLine>(),
                TotalInvoiced = customer.TotalInvoiced,
                TotalPaid = customer.TotalPaid,
                ClosingBalance = customer.Balance
            };
            
            // Add invoice lines
            foreach (var invoice in customer.Invoices.Where(i => 
                i.InvoiceDate >= fromDate && i.InvoiceDate <= toDate))
            {
                statement.Lines.Add(new StatementLine
                {
                    Date = invoice.InvoiceDate,
                    Type = "Invoice",
                    Reference = invoice.InvoiceNumber,
                    Description = $"Invoice #{invoice.InvoiceNumber}",
                    Debit = invoice.Total,
                    Credit = 0,
                    Balance = 0
                });
            }
            
            // Add payment lines
            foreach (var payment in customer.Payments.Where(p => 
                p.PaymentDate >= fromDate && p.PaymentDate <= toDate))
            {
                statement.Lines.Add(new StatementLine
                {
                    Date = payment.PaymentDate,
                    Type = "Payment",
                    Reference = payment.PaymentNumber,
                    Description = $"Payment - {payment.Method}",
                    Debit = 0,
                    Credit = payment.Amount,
                    Balance = 0
                });
            }
            
            // Sort by date and calculate running balance
            statement.Lines = statement.Lines.OrderBy(l => l.Date).ToList();
            decimal runningBalance = statement.OpeningBalance;
            foreach (var line in statement.Lines)
            {
                runningBalance += line.Debit - line.Credit;
                line.Balance = runningBalance;
            }
            
            return Task.FromResult(statement);
        }
    }
}
