using Microsoft.AspNetCore.Mvc;
using IERAHKWA.Platform.Models;

namespace IERAHKWA.Platform.Controllers;

[ApiController]
[Route("api/usage")]
public class UsageController : ControllerBase
{
    [HttpGet("models")]
    public ActionResult<ApiResponse<List<ModelUsage>>> GetModelUsage()
    {
        var usage = new List<ModelUsage>
        {
            new() { Name = "IERAHKWA-70B", Requests = 1247, Tokens = 2400000, Cost = 45.20m, AvgResponseTime = 1.2 },
            new() { Name = "GPT-4 Turbo", Requests = 342, Tokens = 890000, Cost = 28.50m, AvgResponseTime = 2.1 },
            new() { Name = "Claude 3 Opus", Requests = 189, Tokens = 456000, Cost = 18.90m, AvgResponseTime = 1.8 }
        };

        return Ok(new ApiResponse<List<ModelUsage>> { Success = true, Data = usage });
    }
}

public class ModelUsage
{
    public string Name { get; set; } = string.Empty;
    public int Requests { get; set; }
    public long Tokens { get; set; }
    public decimal Cost { get; set; }
    public double AvgResponseTime { get; set; }
}
