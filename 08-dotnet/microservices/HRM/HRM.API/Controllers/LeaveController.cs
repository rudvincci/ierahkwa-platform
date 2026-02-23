using HRM.Core.Interfaces;
using HRM.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaveController : ControllerBase
{
    private readonly IHRMService _hrm;

    public LeaveController(IHRMService hrm) => _hrm = hrm;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid? employeeId, [FromQuery] string? status) =>
        Ok(await _hrm.GetLeavesAsync(employeeId, status));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Leave l) => Ok(await _hrm.CreateLeaveAsync(l));

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] string status)
    {
        var l = await _hrm.UpdateLeaveStatusAsync(id, status);
        return l is null ? NotFound() : Ok(l);
    }
}
