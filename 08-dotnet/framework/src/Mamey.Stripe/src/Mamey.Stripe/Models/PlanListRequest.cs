public class PlanListRequest
{
    public string ProductId { get; set; }
    public bool? Active { get; set; }
    // Further filtering options could include currency, created time range, etc.
}
