using Zoom.Domain.Entities;

namespace Zoom.Application.DTOs;

public class LiveClassDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public int ClassRoomId { get; set; }
    public string? ClassRoomName { get; set; }
    public int? MaterialId { get; set; }
    public string? MaterialName { get; set; }
    public DateTime StartDateTime { get; set; }
    public int DurationMinutes { get; set; }
    public string? ZoomMeetingId { get; set; }
    public string? ZoomJoinUrl { get; set; }
    public string? ZoomStartUrl { get; set; }
    public string? ZoomPassword { get; set; }
    public LiveClassStatus Status { get; set; }
    public string? RecordingUrl { get; set; }
    public bool IsRecorded { get; set; }
    public int AttendeeCount { get; set; }
}

public class CreateLiveClassDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ClassRoomId { get; set; }
    public int? MaterialId { get; set; }
    public DateTime StartDateTime { get; set; }
    public int DurationMinutes { get; set; } = 60;
}

public class UpdateLiveClassDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDateTime { get; set; }
    public int DurationMinutes { get; set; }
}

public class LiveClassAttendanceDto
{
    public int Id { get; set; }
    public int LiveClassId { get; set; }
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public DateTime? JoinedAt { get; set; }
    public DateTime? LeftAt { get; set; }
    public int DurationMinutes { get; set; }
}

public class ZoomSettingsDto
{
    public int Id { get; set; }
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
    public string? AccountId { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? WebhookSecret { get; set; }
    public bool IsActive { get; set; }
    public bool AutoRecord { get; set; }
    public bool EnableWaitingRoom { get; set; }
    public bool MuteParticipantsOnEntry { get; set; }
}

public class CreateZoomSettingsDto
{
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
    public string? AccountId { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? WebhookSecret { get; set; }
    public bool AutoRecord { get; set; } = false;
    public bool EnableWaitingRoom { get; set; } = true;
    public bool MuteParticipantsOnEntry { get; set; } = true;
}

public class ZoomMeetingResponse
{
    public string MeetingId { get; set; } = string.Empty;
    public string JoinUrl { get; set; } = string.Empty;
    public string StartUrl { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
