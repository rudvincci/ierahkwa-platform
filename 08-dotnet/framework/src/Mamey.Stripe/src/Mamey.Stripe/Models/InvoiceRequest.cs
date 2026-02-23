namespace Mamey.Stripe.Models;

public class InvoiceRequest
{
    public string CustomerId { get; set; }
    public decimal? AmountPaid { get; set; } // Amount paid (used when marking an invoice as paid outside of Stripe)
    public long? ApplicationFeeAmount { get; set; } // The fee in cents that will be applied to the invoice and transferred to the application owner’s Stripe account (for Stripe Connect)
    public bool AutoAdvance { get; set; } // Automatically finalize and pay the invoice
    public string Currency { get; set; } // Currency in which subscription is charged
    public string Description { get; set; } // Custom description for the invoice
    public Dictionary<string, string> Metadata { get; set; } // Set of key-value pairs for storing additional information
    public DateTime? DueDate { get; set; } // The date on which payment for this invoice is due
    public bool? Paid { get; set; } // Whether the invoice was paid
    public List<InvoiceItem> InvoiceItems { get; set; } // List of invoice items to be added to the invoice
    public string SubscriptionId { get; set; } // The ID of the subscription to invoice (if any)
    public List<string> TaxRateIds { get; set; } // Default tax rates to apply to the invoice
    public decimal? Discount { get; set; } // A discount to apply to the customer for this invoice
    // Add additional properties as necessary according to the Stripe API documentation

    public InvoiceRequest()
    {
        Metadata = new Dictionary<string, string>();
        InvoiceItems = new List<InvoiceItem>();
        TaxRateIds = new List<string>();
    }
}
