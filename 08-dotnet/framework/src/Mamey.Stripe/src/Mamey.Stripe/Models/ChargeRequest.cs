public class ChargeRequest
{
    public string CustomerId { get; set; }
    public long Amount { get; set; }
    public string Currency { get; set; }
    public string Description { get; set; }
    public string SourceId { get; set; } // Token or payment method for the charge
    public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    // Optional properties like receipt email, shipping info, etc., can be added based on requirements.
}
