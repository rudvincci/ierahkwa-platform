public class SubscriptionListRequest
{
    public string CustomerId { get; set; }
    public string Status { get; set; } // Optional filtering by status (e.g., "active")
    // Additional filtering options could include date ranges for start date, trial end, etc.
}
