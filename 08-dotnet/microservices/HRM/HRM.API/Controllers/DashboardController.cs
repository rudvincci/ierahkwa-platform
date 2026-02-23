using HRM.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IHRMService _hrm;

    public DashboardController(IHRMService hrm) => _hrm = hrm;

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats() =>
        Ok(await _hrm.GetDashboardStatsAsync());
}
