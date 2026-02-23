public class CustomerListRequest
{
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public string Email { get; set; }
    // Additional filtering criteria can be specified as required.
}
