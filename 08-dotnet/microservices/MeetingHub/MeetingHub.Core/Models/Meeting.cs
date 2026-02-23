namespace MeetingHub.Core.Models;

public class Meeting
{
    public Guid Id { get; set; }
    public string MeetingCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public MeetingType Type { get; set; }
    public MeetingStatus Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? TimeZone { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrenceRule { get; set; }
    public Guid OrganizerId { get; set; }
    public string OrganizerName { get; set; } = string.Empty;
    public string OrganizerEmail { get; set; } = string.Empty;
    public string? Department { get; set; }
    public Guid? RoomId { get; set; }
    public string? RoomName { get; set; }
    public string? JoinUrl { get; set; }
    public string? HostUrl { get; set; }
    public string? Password { get; set; }
    public bool RequiresRegistration { get; set; }
    public bool WaitingRoomEnabled { get; set; }
    public bool RecordingEnabled { get; set; }
    public bool TranscriptionEnabled { get; set; }
    public string? RecordingUrl { get; set; }
    public string? TranscriptUrl { get; set; }
    public int MaxParticipants { get; set; } = 100;
    public int CurrentParticipants { get; set; }
    public string? Agenda { get; set; }
    public string? Minutes { get; set; }
    public string? Tags { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int Duration { get; set; } // minutes

    public List<MeetingParticipant> Participants { get; set; } = new();
    public List<MeetingAttachment> Attachments { get; set; } = new();
}

public class MeetingParticipant
{
    public Guid Id { get; set; }
    public Guid MeetingId { get; set; }
    public Guid? UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ParticipantRole Role { get; set; }
    public ParticipantStatus Status { get; set; }
    public DateTime? JoinedAt { get; set; }
    public DateTime? LeftAt { get; set; }
    public int TotalMinutes { get; set; }
    public bool AudioEnabled { get; set; }
    public bool VideoEnabled { get; set; }
    public bool ScreenSharing { get; set; }
    public string? IpAddress { get; set; }
    public string? Device { get; set; }
}

public class MeetingRoom
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? Building { get; set; }
    public string? Floor { get; set; }
    public int Capacity { get; set; }
    public bool IsVirtual { get; set; }
    public bool HasVideoConference { get; set; }
    public bool HasProjector { get; set; }
    public bool HasWhiteboard { get; set; }
    public string? Equipment { get; set; }
    public string? PhotoUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Department { get; set; }
    public Guid? ManagedBy { get; set; }
}

public class MeetingAttachment
{
    public Guid Id { get; set; }
    public Guid MeetingId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public Guid UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; }
}

public enum MeetingType { OneOnOne, Team, Department, AllHands, Webinar, Training, Interview, External }
public enum MeetingStatus { Scheduled, InProgress, Completed, Cancelled, Postponed }
public enum ParticipantRole { Host, CoHost, Presenter, Attendee, Guest }
public enum ParticipantStatus { Invited, Accepted, Declined, Tentative, Joined, Left, NoShow }
