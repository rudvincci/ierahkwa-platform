public class ChargeListRequest
{
    public string CustomerId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    // Additional filters based on requirements can be included.
}
