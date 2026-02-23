using Microsoft.AspNetCore.Mvc;
using IERAHKWA.Platform.Models;

namespace IERAHKWA.Platform.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    [HttpGet("overview")]
    public ActionResult<ApiResponse<DashboardOverview>> GetOverview()
    {
        var overview = new DashboardOverview
        {
            Stats = new DashboardStats
            {
                RequestsToday = 1247,
                LinesGenerated = 48200,
                ServicesOnline = 2,
                Modules = 68
            },
            RecentActivity = new List<ActivityItem>
            {
                new() { Timestamp = DateTime.UtcNow.AddMinutes(-2), UserName = "Admin", Action = "Generated API module", Project = "Banking System", Status = "success" },
                new() { Timestamp = DateTime.UtcNow.AddMinutes(-15), UserName = "Developer", Action = "Refactored code", Project = "Trading Platform", Status = "success" },
                new() { Timestamp = DateTime.UtcNow.AddHours(-1), UserName = "Admin", Action = "Deployed smart contract", Project = "DeFi Protocol", Status = "success" },
                new() { Timestamp = DateTime.UtcNow.AddHours(-3), UserName = "Developer", Action = "Code analysis", Project = "Citizen CRM", Status = "warning" }
            }
        };

        return Ok(new ApiResponse<DashboardOverview> { Success = true, Data = overview });
    }
}

public class DashboardOverview
{
    public DashboardStats Stats { get; set; } = new();
    public List<ActivityItem> RecentActivity { get; set; } = new();
}

public class DashboardStats
{
    public int RequestsToday { get; set; }
    public int LinesGenerated { get; set; }
    public int ServicesOnline { get; set; }
    public int Modules { get; set; }
}

public class ActivityItem
{
    public DateTime Timestamp { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Project { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
