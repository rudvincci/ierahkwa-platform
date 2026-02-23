public class ProductListRequest
{
    public bool? Active { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    // More filters based on categorization, metadata, etc., can be implemented.
}
