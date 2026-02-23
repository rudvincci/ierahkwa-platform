namespace Mamey.ApplicationName.BlazorWasm.Models.Support;

public class SupportTicket
{
    public Guid Id { get; set; }
    public string Subject { get; set; }
    public string Description { get; set; }
    public string Status { get; set; } // Open, In Progress, Closed
    public DateTime CreatedDate { get; set; }
}