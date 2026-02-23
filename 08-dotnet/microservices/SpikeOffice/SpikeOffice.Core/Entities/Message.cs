namespace SpikeOffice.Core.Entities;

public class Message : BaseEntity
{
    public Guid SenderId { get; set; } // SystemUser or Employee
    public string SenderType { get; set; } = "User"; // User, Employee
    public Guid ReceiverId { get; set; }
    public string ReceiverType { get; set; } = "User";
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
}
