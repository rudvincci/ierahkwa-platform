namespace Mamey.Stripe.Models;

public class InvoiceItem
{
    public string Description { get; set; }
    public long Amount { get; set; }
    public string Currency { get; set; }
    public Dictionary<string, string> Metadata { get; set; } // Additional metadata for the invoice item
    public List<string> TaxRates { get; set; } // Specific tax rates to apply to this invoice item

    public InvoiceItem()
    {
        Metadata = new Dictionary<string, string>();
        TaxRates = new List<string>();
    }
}
