namespace SpikeOffice.Core.Entities;

public class ExpenseClaim : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Paid
    public string? ReceiptUrl { get; set; }
}
