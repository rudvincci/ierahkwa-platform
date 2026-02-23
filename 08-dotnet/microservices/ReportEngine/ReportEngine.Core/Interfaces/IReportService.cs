using ReportEngine.Core.Models;
namespace ReportEngine.Core.Interfaces;

public interface IReportService
{
    Task<Report> CreateReportAsync(Report report);
    Task<Report?> GetReportByIdAsync(Guid id);
    Task<IEnumerable<Report>> GetReportsAsync(ReportCategory? category = null, string? department = null, bool? isPublic = null);
    Task<Report> UpdateReportAsync(Report report);
    Task<Report> PublishReportAsync(Guid id);
    Task DeleteReportAsync(Guid id);

    Task<ReportExecution> ExecuteReportAsync(Guid reportId, Guid userId, string userName, OutputFormat format, Dictionary<string, object>? parameters = null);
    Task<IEnumerable<ReportExecution>> GetExecutionsAsync(Guid? reportId = null, Guid? userId = null);
    Task<byte[]> GetReportOutputAsync(Guid executionId);

    Task<Dashboard> CreateDashboardAsync(Dashboard dashboard);
    Task<Dashboard?> GetDashboardByIdAsync(Guid id);
    Task<IEnumerable<Dashboard>> GetDashboardsAsync(string? department = null, bool? isPublic = null);
    Task<Dashboard> UpdateDashboardAsync(Dashboard dashboard);
    Task DeleteDashboardAsync(Guid id);

    Task<DashboardWidget> AddWidgetAsync(DashboardWidget widget);
    Task<DashboardWidget> UpdateWidgetAsync(DashboardWidget widget);
    Task RemoveWidgetAsync(Guid widgetId);
    Task<object> GetWidgetDataAsync(Guid widgetId);

    Task<ScheduledReport> CreateScheduleAsync(ScheduledReport schedule);
    Task<IEnumerable<ScheduledReport>> GetSchedulesAsync(bool? isActive = null);
    Task<ScheduledReport> UpdateScheduleAsync(ScheduledReport schedule);
    Task DeleteScheduleAsync(Guid id);
    Task ExecuteScheduledReportsAsync();

    Task<ReportStatistics> GetStatisticsAsync();
}

public class ReportStatistics
{
    public int TotalReports { get; set; }
    public int PublishedReports { get; set; }
    public int TotalDashboards { get; set; }
    public int TotalExecutions { get; set; }
    public int ExecutionsToday { get; set; }
    public int ScheduledReports { get; set; }
    public Dictionary<string, int> ReportsByCategory { get; set; } = new();
    public Dictionary<string, int> ExecutionsByFormat { get; set; } = new();
    public List<Report> MostUsedReports { get; set; } = new();
}
