public class Subscription
{
    public string Id { get; set; }
    public string CustomerId { get; set; }
    public List<SubscriptionItem> Items { get; set; } = new List<SubscriptionItem>();
    public string Status { get; set; } // e.g., active, canceled
    public DateTime StartDate { get; set; }
    public DateTime? TrialEnd { get; set; }
    public DateTime? CurrentPeriodEnd { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
    // Additional properties for handling billing, trial periods, and invoice settings can be included.
}
