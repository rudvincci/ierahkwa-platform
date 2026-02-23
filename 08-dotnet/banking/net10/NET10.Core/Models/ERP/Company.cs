using System;
using System.Collections.Generic;

namespace NET10.Core.Models.ERP
{
    /// <summary>
    /// Company/Business Entity for Multi-Company Support
    /// </summary>
    public class Company
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string LegalName { get; set; } = string.Empty;
        public string TaxId { get; set; } = string.Empty; // GST/VAT Number
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Currency { get; set; } = "USD";
        public string Logo { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Settings
        public TaxSettings TaxSettings { get; set; } = new();
        public InvoiceSettings InvoiceSettings { get; set; } = new();
    }
    
    public class TaxSettings
    {
        public bool EnableGST { get; set; } = true;
        public bool EnableVAT { get; set; } = false;
        public decimal DefaultTaxRate { get; set; } = 16.0m;
        public string TaxLabel { get; set; } = "IVA";
    }
    
    public class InvoiceSettings
    {
        public string Prefix { get; set; } = "INV-";
        public int NextNumber { get; set; } = 1001;
        public int PaymentTermsDays { get; set; } = 30;
        public string DefaultNotes { get; set; } = string.Empty;
        public string DefaultTerms { get; set; } = string.Empty;
    }
}
