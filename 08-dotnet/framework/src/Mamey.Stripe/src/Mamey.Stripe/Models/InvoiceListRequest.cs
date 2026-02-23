public class InvoiceListRequest
{
    public string CustomerId { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public string Status { get; set; }
    // Additional filters based on requirements can be included.
}
