public class Invoice
{
    public string Id { get; set; }
    public string CustomerId { get; set; }
    public DateTime Created { get; set; }
    public long AmountDue { get; set; } // Amount in smallest currency unit (e.g., cents)
    public string Currency { get; set; }
    public string Status { get; set; }
    public DateTime? DueDate { get; set; }
    public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    public Dictionary<string, string> Metadata { get; set; }
    // Consider adding properties for discounts, taxes, and payment status.
}
