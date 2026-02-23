namespace Mamey.Stripe.Models;

public class CustomerRequest
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Phone { get; set; }
    public Dictionary<string, string> Metadata { get; set; } // Custom metadata to associate with the customer
    public string Balance { get; set; } // Customer's balance (can be used in subsequent invoices)
    public string Currency { get; set; } // Default currency for billing
    public List<string> PaymentMethodIds { get; set; } // List of PaymentMethod IDs to attach to the customer
    public string InvoicePrefix { get; set; } // Prefix for invoices created for this customer
    public bool? InvoiceSettingsCustomFields { get; set; } // Enable custom fields in customer's invoices
    public string PreferredLocales { get; set; } // Preferred locales for the customer which may affect emailed invoice localization
    public string TaxExempt { get; set; } // Customer's tax status. One of `none`, `exempt`, or `reverse`
    public Address ShippingAddress { get; set; } // Shipping address for the customer
    public Address BillingAddress { get; set; } // Billing address for the customer
    public DateTime? Birthday { get; set; } // Customer's birthday, useful for age verification and customer insights
    
    public CustomerRequest()
    {
        Metadata = new Dictionary<string, string>();
        PaymentMethodIds = new List<string>();
    }
}
