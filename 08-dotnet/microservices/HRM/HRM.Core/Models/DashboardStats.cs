namespace HRM.Core.Models;

public class DashboardStats
{
    public int TotalEmployees { get; set; }
    public int ActiveEmployees { get; set; }
    public int PresentToday { get; set; }
    public int OnLeave { get; set; }
    public int PendingLeaves { get; set; }
    public int OpenRecruitments { get; set; }
    public int ActiveProjects { get; set; }
    public decimal MonthlyPayroll { get; set; }
    public List<ChartData> AttendanceChart { get; set; } = new();
    public List<ChartData> DepartmentChart { get; set; } = new();
}

public class ChartData
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
}
