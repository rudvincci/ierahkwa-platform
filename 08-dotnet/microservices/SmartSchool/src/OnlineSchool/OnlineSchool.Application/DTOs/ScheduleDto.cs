namespace OnlineSchool.Application.DTOs;

public class ScheduleDto
{
    public int Id { get; set; }
    public int ClassRoomId { get; set; }
    public string? ClassRoomName { get; set; }
    public int TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public int MaterialId { get; set; }
    public string? MaterialName { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Room { get; set; }
    public bool IsActive { get; set; }
}

public class CreateScheduleDto
{
    public int ClassRoomId { get; set; }
    public int TeacherId { get; set; }
    public int MaterialId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Room { get; set; }
}

public class UpdateScheduleDto
{
    public int Id { get; set; }
    public int ClassRoomId { get; set; }
    public int TeacherId { get; set; }
    public int MaterialId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Room { get; set; }
    public bool IsActive { get; set; }
}

public class ScheduleSettingsDto
{
    public int Id { get; set; }
    public TimeSpan SchoolStartTime { get; set; }
    public TimeSpan SchoolEndTime { get; set; }
    public int PeriodDurationMinutes { get; set; }
    public int BreakDurationMinutes { get; set; }
    public int PeriodsPerDay { get; set; }
}

public class WeeklyScheduleDto
{
    public int ClassRoomId { get; set; }
    public string? ClassRoomName { get; set; }
    public IEnumerable<DayScheduleDto> Days { get; set; } = new List<DayScheduleDto>();
}

public class DayScheduleDto
{
    public DayOfWeek DayOfWeek { get; set; }
    public IEnumerable<ScheduleDto> Schedules { get; set; } = new List<ScheduleDto>();
}
