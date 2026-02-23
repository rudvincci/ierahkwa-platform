namespace Mamey.Stripe.Models;

public class Plan
{
    public string Id { get; set; }
    public string ProductId { get; set; }
    public long Amount { get; set; } // Amount in the smallest currency unit
    public string Currency { get; set; }
    public string Interval { get; set; } // Billing interval, e.g., "month", "year"
    public int IntervalCount { get; set; } // Number of intervals between billings
    public string Nickname { get; set; }
    public DateTime Created { get; set; }
    public bool Active { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
    // Consider properties for trial periods or metered billing if applicable.
}
