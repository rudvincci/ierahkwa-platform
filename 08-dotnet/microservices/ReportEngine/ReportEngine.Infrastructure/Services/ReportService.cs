using ReportEngine.Core.Interfaces;
using ReportEngine.Core.Models;
using System.Text;
namespace ReportEngine.Infrastructure.Services;

public class ReportService : IReportService
{
    private readonly List<Report> _reports = new();
    private readonly List<ReportExecution> _executions = new();
    private readonly List<Dashboard> _dashboards = new();
    private readonly List<DashboardWidget> _widgets = new();
    private readonly List<ScheduledReport> _schedules = new();

    public Task<Report> CreateReportAsync(Report report) { report.Id = Guid.NewGuid(); report.ReportCode = $"RPT-{_reports.Count + 1:D5}"; report.Status = ReportStatus.Draft; report.CreatedAt = DateTime.UtcNow; _reports.Add(report); return Task.FromResult(report); }
    public Task<Report?> GetReportByIdAsync(Guid id) => Task.FromResult(_reports.FirstOrDefault(r => r.Id == id));
    public Task<IEnumerable<Report>> GetReportsAsync(ReportCategory? category = null, string? department = null, bool? isPublic = null) { var q = _reports.AsEnumerable(); if (category.HasValue) q = q.Where(r => r.Category == category.Value); if (!string.IsNullOrEmpty(department)) q = q.Where(r => r.Department == department); if (isPublic.HasValue) q = q.Where(r => r.IsPublic == isPublic.Value); return Task.FromResult(q); }
    public Task<Report> UpdateReportAsync(Report report) { var e = _reports.FirstOrDefault(r => r.Id == report.Id); if (e != null) { e.Name = report.Name; e.QueryDefinition = report.QueryDefinition; e.UpdatedAt = DateTime.UtcNow; } return Task.FromResult(e ?? report); }
    public Task<Report> PublishReportAsync(Guid id) { var r = _reports.FirstOrDefault(r => r.Id == id); if (r != null) r.Status = ReportStatus.Published; return Task.FromResult(r!); }
    public Task DeleteReportAsync(Guid id) { _reports.RemoveAll(r => r.Id == id); return Task.CompletedTask; }

    public Task<ReportExecution> ExecuteReportAsync(Guid reportId, Guid userId, string userName, OutputFormat format, Dictionary<string, object>? parameters = null)
    {
        var report = _reports.FirstOrDefault(r => r.Id == reportId);
        var execution = new ReportExecution { Id = Guid.NewGuid(), ReportId = reportId, ReportName = report?.Name ?? "", ExecutedBy = userId, ExecutedByName = userName, OutputFormat = format, Status = ExecutionStatus.Running, StartedAt = DateTime.UtcNow, Parameters = parameters != null ? System.Text.Json.JsonSerializer.Serialize(parameters) : null };
        _executions.Add(execution);
        execution.Status = ExecutionStatus.Completed; execution.CompletedAt = DateTime.UtcNow; execution.DurationMs = 100; execution.RowCount = 100; execution.OutputUrl = $"/reports/output/{execution.Id}";
        if (report != null) { report.RunCount++; report.LastRunAt = DateTime.UtcNow; }
        return Task.FromResult(execution);
    }
    public Task<IEnumerable<ReportExecution>> GetExecutionsAsync(Guid? reportId = null, Guid? userId = null) { var q = _executions.AsEnumerable(); if (reportId.HasValue) q = q.Where(e => e.ReportId == reportId.Value); if (userId.HasValue) q = q.Where(e => e.ExecutedBy == userId.Value); return Task.FromResult(q.OrderByDescending(e => e.StartedAt)); }
    public Task<byte[]> GetReportOutputAsync(Guid executionId) => Task.FromResult(Encoding.UTF8.GetBytes("Report Output"));

    public Task<Dashboard> CreateDashboardAsync(Dashboard dashboard) { dashboard.Id = Guid.NewGuid(); dashboard.CreatedAt = DateTime.UtcNow; _dashboards.Add(dashboard); return Task.FromResult(dashboard); }
    public Task<Dashboard?> GetDashboardByIdAsync(Guid id) { var d = _dashboards.FirstOrDefault(d => d.Id == id); if (d != null) d.Widgets = _widgets.Where(w => w.DashboardId == id).ToList(); return Task.FromResult(d); }
    public Task<IEnumerable<Dashboard>> GetDashboardsAsync(string? department = null, bool? isPublic = null) { var q = _dashboards.AsEnumerable(); if (!string.IsNullOrEmpty(department)) q = q.Where(d => d.Department == department); if (isPublic.HasValue) q = q.Where(d => d.IsPublic == isPublic.Value); return Task.FromResult(q); }
    public Task<Dashboard> UpdateDashboardAsync(Dashboard dashboard) { var e = _dashboards.FirstOrDefault(d => d.Id == dashboard.Id); if (e != null) { e.Name = dashboard.Name; e.LayoutDefinition = dashboard.LayoutDefinition; e.UpdatedAt = DateTime.UtcNow; } return Task.FromResult(e ?? dashboard); }
    public Task DeleteDashboardAsync(Guid id) { _dashboards.RemoveAll(d => d.Id == id); _widgets.RemoveAll(w => w.DashboardId == id); return Task.CompletedTask; }

    public Task<DashboardWidget> AddWidgetAsync(DashboardWidget widget) { widget.Id = Guid.NewGuid(); _widgets.Add(widget); return Task.FromResult(widget); }
    public Task<DashboardWidget> UpdateWidgetAsync(DashboardWidget widget) { var e = _widgets.FirstOrDefault(w => w.Id == widget.Id); if (e != null) { e.Title = widget.Title; e.Query = widget.Query; e.PositionX = widget.PositionX; e.PositionY = widget.PositionY; } return Task.FromResult(e ?? widget); }
    public Task RemoveWidgetAsync(Guid widgetId) { _widgets.RemoveAll(w => w.Id == widgetId); return Task.CompletedTask; }
    public Task<object> GetWidgetDataAsync(Guid widgetId) => Task.FromResult<object>(new { Value = 100, Label = "Sample Data" });

    public Task<ScheduledReport> CreateScheduleAsync(ScheduledReport schedule) { schedule.Id = Guid.NewGuid(); schedule.CreatedAt = DateTime.UtcNow; schedule.IsActive = true; _schedules.Add(schedule); return Task.FromResult(schedule); }
    public Task<IEnumerable<ScheduledReport>> GetSchedulesAsync(bool? isActive = null) => Task.FromResult(isActive.HasValue ? _schedules.Where(s => s.IsActive == isActive.Value) : _schedules.AsEnumerable());
    public Task<ScheduledReport> UpdateScheduleAsync(ScheduledReport schedule) { var e = _schedules.FirstOrDefault(s => s.Id == schedule.Id); if (e != null) { e.CronExpression = schedule.CronExpression; e.IsActive = schedule.IsActive; } return Task.FromResult(e ?? schedule); }
    public Task DeleteScheduleAsync(Guid id) { _schedules.RemoveAll(s => s.Id == id); return Task.CompletedTask; }
    public Task ExecuteScheduledReportsAsync() => Task.CompletedTask;

    public Task<ReportStatistics> GetStatisticsAsync() => Task.FromResult(new ReportStatistics { TotalReports = _reports.Count, PublishedReports = _reports.Count(r => r.Status == ReportStatus.Published), TotalDashboards = _dashboards.Count, TotalExecutions = _executions.Count, ExecutionsToday = _executions.Count(e => e.StartedAt.Date == DateTime.UtcNow.Date), ScheduledReports = _schedules.Count(s => s.IsActive), ReportsByCategory = _reports.GroupBy(r => r.Category.ToString()).ToDictionary(g => g.Key, g => g.Count()), MostUsedReports = _reports.OrderByDescending(r => r.RunCount).Take(5).ToList() });
}
