using Common.Domain.Entities;

namespace Zoom.Domain.Entities;

public class LiveClass : TenantEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TeacherId { get; set; }
    public int ClassRoomId { get; set; }
    public int? MaterialId { get; set; }
    public DateTime StartDateTime { get; set; }
    public int DurationMinutes { get; set; } = 60;
    public string? ZoomMeetingId { get; set; }
    public string? ZoomJoinUrl { get; set; }
    public string? ZoomStartUrl { get; set; }
    public string? ZoomPassword { get; set; }
    public LiveClassStatus Status { get; set; } = LiveClassStatus.Scheduled;
    public string? RecordingUrl { get; set; }
    public bool IsRecorded { get; set; } = false;
    
    public virtual ICollection<LiveClassAttendance> Attendances { get; set; } = new List<LiveClassAttendance>();
}

public enum LiveClassStatus
{
    Scheduled,
    InProgress,
    Completed,
    Cancelled
}

public class LiveClassAttendance : TenantEntity
{
    public int LiveClassId { get; set; }
    public int StudentId { get; set; }
    public DateTime? JoinedAt { get; set; }
    public DateTime? LeftAt { get; set; }
    public int DurationMinutes { get; set; } = 0;
    
    public virtual LiveClass? LiveClass { get; set; }
}
