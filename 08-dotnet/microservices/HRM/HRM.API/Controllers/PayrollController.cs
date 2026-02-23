using HRM.Core.Interfaces;
using HRM.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PayrollController : ControllerBase
{
    private readonly IHRMService _hrm;

    public PayrollController(IHRMService hrm) => _hrm = hrm;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? period, [FromQuery] Guid? employeeId) =>
        Ok(await _hrm.GetPayrollAsync(period, employeeId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Payroll p) => Ok(await _hrm.CreatePayrollAsync(p));
}
