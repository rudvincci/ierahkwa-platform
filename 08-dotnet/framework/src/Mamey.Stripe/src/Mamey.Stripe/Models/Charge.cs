public class Charge
{
    public string Id { get; set; }
    public string CustomerId { get; set; }
    public long Amount { get; set; }
    public string Currency { get; set; }
    public bool Paid { get; set; }
    public string Status { get; set; }
    public string Description { get; set; }
    public DateTime Created { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
    // Additional properties like receipt URL, refund status, etc., can be included as needed.
}
