public class Customer
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Description { get; set; }
    public DateTime Created { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
    // Consider including more detailed fields such as addresses, list of payment methods, subscriptions, and invoice settings.
}
