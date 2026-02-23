using System;
using System.Collections.Generic;

namespace NET10.Core.Models.ERP
{
    /// <summary>
    /// Payment Receipt
    /// </summary>
    public class Payment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CompanyId { get; set; }
        public string PaymentNumber { get; set; } = string.Empty;
        
        // Customer
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        
        // Payment Details
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public decimal Amount { get; set; } = 0;
        public PaymentMethod Method { get; set; } = PaymentMethod.Cash;
        public string Reference { get; set; } = string.Empty;
        
        // Bank Details (for transfers)
        public string BankName { get; set; } = string.Empty;
        public string BankAccount { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        
        // Applied to Invoices
        public List<PaymentAllocation> Allocations { get; set; } = new();
        
        // Status
        public PaymentRecordStatus Status { get; set; } = PaymentRecordStatus.Completed;
        public string Notes { get; set; } = string.Empty;
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Payment allocation to specific invoice
    /// </summary>
    public class PaymentAllocation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PaymentId { get; set; }
        public Guid InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; } = 0;
    }
    
    public enum PaymentMethod
    {
        Cash,
        BankTransfer,
        CreditCard,
        DebitCard,
        Check,
        PayPal,
        Crypto,
        Other
    }
    
    public enum PaymentRecordStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded,
        Cancelled
    }
}
