using System;
using System.Collections.Generic;

namespace NET10.Core.Models.ERP
{
    /// <summary>
    /// Customer/Client Entity
    /// </summary>
    public class Customer
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CompanyId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string TaxId { get; set; } = string.Empty;
        
        // Billing Address
        public string BillingAddress { get; set; } = string.Empty;
        public string BillingCity { get; set; } = string.Empty;
        public string BillingState { get; set; } = string.Empty;
        public string BillingCountry { get; set; } = string.Empty;
        public string BillingPostalCode { get; set; } = string.Empty;
        
        // Shipping Address
        public string ShippingAddress { get; set; } = string.Empty;
        public string ShippingCity { get; set; } = string.Empty;
        public string ShippingState { get; set; } = string.Empty;
        public string ShippingCountry { get; set; } = string.Empty;
        public string ShippingPostalCode { get; set; } = string.Empty;
        
        // Credit
        public decimal CreditLimit { get; set; } = 0;
        public int PaymentTermsDays { get; set; } = 30;
        
        // Balance
        public decimal TotalInvoiced { get; set; } = 0;
        public decimal TotalPaid { get; set; } = 0;
        public decimal Balance => TotalInvoiced - TotalPaid;
        
        // Status
        public CustomerStatus Status { get; set; } = CustomerStatus.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastPurchaseDate { get; set; }
        
        // Navigation
        public List<Invoice> Invoices { get; set; } = new();
        public List<Payment> Payments { get; set; } = new();
    }
    
    public enum CustomerStatus
    {
        Active,
        Inactive,
        Blocked,
        PendingApproval
    }
}
