using Microsoft.AspNetCore.Mvc;
using IERAHKWA.Platform.Models;

namespace IERAHKWA.Platform.Controllers;

[ApiController]
[Route("api/settings")]
public class SettingsController : ControllerBase
{
    [HttpPost]
    public ActionResult<ApiResponse<object>> SaveSettings([FromBody] SettingsRequest request)
    {
        // Save settings (in production, would save to database)
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Data = new { message = "Settings saved successfully" }
        });
    }
}

public class SettingsRequest
{
    public string? DefaultModel { get; set; }
    public bool AutoComplete { get; set; }
    public bool BackgroundAgents { get; set; }
}
