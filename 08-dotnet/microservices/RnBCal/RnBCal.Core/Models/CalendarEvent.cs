namespace RnBCal.Core.Models;

/// <summary>
/// Calendar Event for ICS Generation
/// </summary>
public class CalendarEvent
{
    public string Uid { get; set; } = Guid.NewGuid().ToString();
    public string Summary { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string OrganizerEmail { get; set; } = string.Empty;
    public string OrganizerName { get; set; } = string.Empty;
    public List<string> AttendeeEmails { get; set; } = new();
    public CalendarEventStatus Status { get; set; } = CalendarEventStatus.Confirmed;
    public int ReminderMinutesBefore { get; set; } = 60;
    public string? Categories { get; set; }
    public int Priority { get; set; } = 5; // 1-9, where 1 is highest
    
    // Custom properties
    public Dictionary<string, string> CustomProperties { get; set; } = new();
}

public enum CalendarEventStatus
{
    Tentative,
    Confirmed,
    Cancelled
}
