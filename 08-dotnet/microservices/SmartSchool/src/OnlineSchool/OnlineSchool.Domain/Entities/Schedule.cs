using Common.Domain.Entities;

namespace OnlineSchool.Domain.Entities;

public class Schedule : TenantEntity
{
    public int ClassRoomId { get; set; }
    public int TeacherId { get; set; }
    public int MaterialId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Room { get; set; }
    public bool IsActive { get; set; } = true;
    
    public virtual ClassRoom? ClassRoom { get; set; }
    public virtual Teacher? Teacher { get; set; }
    public virtual Material? Material { get; set; }
}

public class ScheduleSettings : TenantEntity
{
    public TimeSpan SchoolStartTime { get; set; } = new TimeSpan(8, 0, 0);
    public TimeSpan SchoolEndTime { get; set; } = new TimeSpan(15, 0, 0);
    public int PeriodDurationMinutes { get; set; } = 45;
    public int BreakDurationMinutes { get; set; } = 15;
    public int PeriodsPerDay { get; set; } = 8;
}
