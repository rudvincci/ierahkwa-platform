using HRM.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RewardPointsController : ControllerBase
{
    private readonly IHRMService _hrm;

    public RewardPointsController(IHRMService hrm) => _hrm = hrm;

    [HttpGet("employee/{employeeId:guid}")]
    public async Task<IActionResult> GetByEmployee(Guid employeeId) =>
        Ok(await _hrm.GetRewardPointsAsync(employeeId));

    [HttpGet("employee/{employeeId:guid}/total")]
    public async Task<IActionResult> GetTotal(Guid employeeId) =>
        Ok(new { total = await _hrm.GetEmployeeTotalPointsAsync(employeeId) });
}
