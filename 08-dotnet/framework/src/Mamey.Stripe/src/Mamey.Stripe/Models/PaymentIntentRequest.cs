namespace Mamey.Stripe.Models;
public class PaymentIntentRequest
{
    public long Amount { get; set; }
    public string Currency { get; set; }
    public IEnumerable<string> PaymentMethodTypes { get; set; }
    public string ReceiptEmail { get; set; }
    public string CustomerId { get; set; }
    public string Description { get; set; }
    public bool? Confirm { get; set; }
    public string PaymentMethod { get; set; }
    public bool? SetupFutureUsage { get; set; }
    public string ReturnUrl { get; set; }
    public IEnumerable<PaymentIntentMetadata> Metadata { get; set; }
    public IEnumerable<string> Expand { get; set; }
    public string StatementDescriptor { get; set; }
    public string StatementDescriptorSuffix { get; set; }
    public ShippingDetails Shipping { get; set; }
    public TransferData TransferData { get; set; }
    public string ApplicationFeeAmount { get; set; }
    public IEnumerable<string> ReceiptEmails { get; set; }
    public CaptureMethod? CaptureMethod { get; set; }
    public ConfirmationMethod? ConfirmationMethod { get; set; }
    public string OnBehalfOf { get; set; }
    
    // Additional properties and configurations as required by the Stripe Payment Intents API
}
