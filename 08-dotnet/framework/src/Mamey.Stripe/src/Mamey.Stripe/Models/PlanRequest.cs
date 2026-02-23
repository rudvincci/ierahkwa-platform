public class PlanRequest
{
    public string ProductId { get; set; }
    public long Amount { get; set; } // Amount in the smallest currency unit (e.g., cents for USD)
    public string Currency { get; set; }
    public string Interval { get; set; } // e.g., "month", "year"
    public int? IntervalCount { get; set; } // Specifies the number of intervals between subscription billings.
    public string Nickname { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    // Additional properties like usage_type could be added for metered billing plans.
}
