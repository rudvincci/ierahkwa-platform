namespace Mamey.Stripe.Models;

public class RefundRequest
{
    public string ChargeId { get; set; } // The ID of the charge to refund.
    public string PaymentIntentId { get; set; } // The ID of the payment intent to refund if the payment was made using PaymentIntent.
    public long? Amount { get; set; } // Amount to refund (can be less than the charge amount for partial refunds). 
    public Dictionary<string, string> Metadata { get; set; } // A set of key-value pairs that you can attach to the refund object.
    public string Reason { get; set; } // String indicating the reason for the refund. Valid options are `duplicate`, `fraudulent`, and `requested_by_customer`.
    public bool? RefundApplicationFee { get; set; } // Whether to refund the application fee for the charge.
    public bool? ReverseTransfer { get; set; } // Whether to reverse the transfer for the charge.
    
    // Additional properties for expanded refund capabilities
    public string Currency { get; set; } // Currency of the refund. This must be the same currency as the charge.
    public string Source { get; set; } // Source of the refund. This is typically the ID of a connected account.
    public string Description { get; set; } // An arbitrary string attached to the refund for your internal use.
    
    public RefundRequest()
    {
        Metadata = new Dictionary<string, string>();
    }
}
