namespace HRM.Core.Models;

public class Notification
{
    public Guid Id { get; set; }
    public Guid? EmployeeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "Info"; // Info, Warning, Success, Alert
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
