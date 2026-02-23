namespace ReportEngine.Core.Models;

public class Report
{
    public Guid Id { get; set; }
    public string ReportCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ReportType Type { get; set; }
    public ReportCategory Category { get; set; }
    public string? Department { get; set; }
    public string DataSource { get; set; } = string.Empty;
    public string? QueryDefinition { get; set; }
    public string? LayoutDefinition { get; set; }
    public string? Parameters { get; set; }
    public string? Filters { get; set; }
    public ReportStatus Status { get; set; }
    public bool IsPublic { get; set; }
    public bool IsScheduled { get; set; }
    public string? ScheduleCron { get; set; }
    public Guid CreatedBy { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int RunCount { get; set; }
    public DateTime? LastRunAt { get; set; }
}

public class ReportExecution
{
    public Guid Id { get; set; }
    public Guid ReportId { get; set; }
    public string ReportName { get; set; } = string.Empty;
    public Guid ExecutedBy { get; set; }
    public string ExecutedByName { get; set; } = string.Empty;
    public ExecutionStatus Status { get; set; }
    public string? Parameters { get; set; }
    public OutputFormat OutputFormat { get; set; }
    public string? OutputUrl { get; set; }
    public long? OutputSize { get; set; }
    public int? RowCount { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? DurationMs { get; set; }
    public string? ErrorMessage { get; set; }
}

public class Dashboard
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Department { get; set; }
    public DashboardType Type { get; set; }
    public bool IsPublic { get; set; }
    public string LayoutDefinition { get; set; } = "[]";
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<DashboardWidget> Widgets { get; set; } = new();
}

public class DashboardWidget
{
    public Guid Id { get; set; }
    public Guid DashboardId { get; set; }
    public string Title { get; set; } = string.Empty;
    public WidgetType Type { get; set; }
    public string DataSource { get; set; } = string.Empty;
    public string? Query { get; set; }
    public string? Configuration { get; set; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int RefreshIntervalSeconds { get; set; }
}

public class ScheduledReport
{
    public Guid Id { get; set; }
    public Guid ReportId { get; set; }
    public string ReportName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CronExpression { get; set; } = string.Empty;
    public OutputFormat OutputFormat { get; set; }
    public string? Parameters { get; set; }
    public string? Recipients { get; set; }
    public DeliveryMethod DeliveryMethod { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastRunAt { get; set; }
    public DateTime? NextRunAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum ReportType { Query, Chart, Table, Pivot, Dashboard, Custom }
public enum ReportCategory { Financial, Operational, HR, Compliance, Executive, Audit, Custom }
public enum ReportStatus { Draft, Published, Archived }
public enum ExecutionStatus { Running, Completed, Failed, Cancelled }
public enum OutputFormat { PDF, Excel, CSV, JSON, HTML, Word }
public enum DashboardType { Executive, Operational, Analytical, KPI }
public enum WidgetType { Chart, Table, KPI, Gauge, Map, Text, Image }
public enum DeliveryMethod { Email, FileShare, API, Portal }
