namespace MeetingHub.Core.Models;

public class CalendarEvent
{
    public Guid Id { get; set; }
    public Guid CalendarId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public EventType Type { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool AllDay { get; set; }
    public string? TimeZone { get; set; }
    public string? Location { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrenceRule { get; set; }
    public Guid CreatedBy { get; set; }
    public string? Color { get; set; }
    public EventVisibility Visibility { get; set; }
    public bool IsBusy { get; set; } = true;
    public string? ReminderMinutes { get; set; }
    public Guid? MeetingId { get; set; }
    public string? ExternalEventId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<EventAttendee> Attendees { get; set; } = new();
}

public class Calendar
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public CalendarType Type { get; set; }
    public Guid OwnerId { get; set; }
    public string? OwnerName { get; set; }
    public string? Department { get; set; }
    public string Color { get; set; } = "#3B82F6";
    public bool IsDefault { get; set; }
    public bool IsShared { get; set; }
    public bool IsPublic { get; set; }
    public string? TimeZone { get; set; }
    public string? SyncSource { get; set; }
    public string? SyncToken { get; set; }
    public DateTime? LastSyncAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class EventAttendee
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Guid? UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public AttendeeRole Role { get; set; }
    public AttendeeResponse Response { get; set; }
    public DateTime? RespondedAt { get; set; }
    public string? Comment { get; set; }
}

public class RoomBooking
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public Guid? MeetingId { get; set; }
    public Guid? EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public Guid BookedBy { get; set; }
    public string BookedByName { get; set; } = string.Empty;
    public BookingStatus Status { get; set; }
    public string? Notes { get; set; }
    public bool RequiresApproval { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum EventType { Meeting, Appointment, Task, Reminder, Holiday, Birthday, OutOfOffice, Travel, Other }
public enum EventVisibility { Public, Private, Confidential }
public enum CalendarType { Personal, Team, Department, Organization, Resource, Holiday }
public enum AttendeeRole { Required, Optional, Resource }
public enum AttendeeResponse { NeedsAction, Accepted, Declined, Tentative }
public enum BookingStatus { Pending, Confirmed, Cancelled, Completed }
