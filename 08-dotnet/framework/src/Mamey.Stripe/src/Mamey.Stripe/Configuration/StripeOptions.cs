namespace Mamey.Stripe.Configuration;

public class StripeOptions
{
    public string ApiKey { get; set; }
    public string WebhookSecret { get; set; } // For validating webhooks

    // General settings
    public string AccountId { get; set; } // For Stripe Connect
    public string SigningSecret { get; set; } // For webhook signing secrets, alternative to WebhookSecret for multiple endpoints

    // Payments and Currency
    public string DefaultCurrency { get; set; } // Default currency for transactions

    // Billing and Subscriptions
    public bool AutoConfirmSubscriptions { get; set; } // Automatically confirm subscriptions if true
    public string InvoicePrefix { get; set; } // Prefix for invoice numbers

    // Checkout Sessions
    public string SuccessUrl { get; set; } // Redirect URL after successful payment
    public string CancelUrl { get; set; } // Redirect URL after cancellation

    // Stripe Radar and Fraud Detection
    public decimal? RadarThreshold { get; set; } // Threshold for Radar fraud detection rules

    // Advanced Configuration
    public int? ApiVersion { get; set; } // Optionally specify the Stripe API version

    // Connect Settings
    public bool? UseConnect { get; set; } // Enable use of Stripe Connect features
    public string ConnectClientId { get; set; } // Client ID for Stripe Connect applications
}
