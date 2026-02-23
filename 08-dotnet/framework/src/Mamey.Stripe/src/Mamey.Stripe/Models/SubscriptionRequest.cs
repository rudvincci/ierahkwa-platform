namespace Mamey.Stripe.Models;

public class SubscriptionRequest
{
    public string CustomerId { get; set; } // The ID of the customer who will be billed.
    public List<SubscriptionItem> Items { get; set; } // A list of subscription items, each with an attached plan.
    public string DefaultPaymentMethod { get; set; } // ID of the default payment method for the subscription.
    public long? TrialEnd { get; set; } // Unix timestamp representing the end of the trial period the customer will get before being charged.
    public string Coupon { get; set; } // The code of the coupon to apply to this subscription.
    public Dictionary<string, string> Metadata { get; set; } // Set of key-value pairs that you can attach to a subscription object.
    public string Status { get; set; } // The status of the subscription, e.g., 'active'. Only settable on creation.
    public List<string> Expand { get; set; } // Specifies which fields in the response should be expanded.
    public int? BillingCycleAnchor { get; set; } // Determines the date of the first full invoice, and, for plans with `month` or `year` intervals, the day of the month for subsequent invoices.
    public bool? Prorate { get; set; } // Whether to prorate when making changes to the subscription.
    public string CancelAtPeriodEnd { get; set; } // If set to true, the subscription will be canceled at the end of the current period.
    public Dictionary<string, SubscriptionSchedule> SubscriptionSchedule { get; set; } // For subscription schedule management.
    public List<TaxRate> DefaultTaxRates { get; set; } // The default tax rates to apply to the subscription.
    public string PlanId { get; set; }
    public SubscriptionRequest()
    {
        Items = new List<SubscriptionItem>();
        Metadata = new Dictionary<string, string>();
        Expand = new List<string>();
        SubscriptionSchedule = new Dictionary<string, SubscriptionSchedule>();
        DefaultTaxRates = new List<TaxRate>();
    }
}
