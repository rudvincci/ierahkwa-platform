namespace HRM.Core.Models;

public class Procurement
{
    public Guid Id { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? Vendor { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Requested"; // Requested, Approved, Ordered, Received
    public DateTime RequestDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public string? RequestedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
