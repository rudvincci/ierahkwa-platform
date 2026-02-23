using HRM.Core.Interfaces;
using HRM.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly IHRMService _hrm;

    public AttendanceController(IHRMService hrm) => _hrm = hrm;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DateTime? date, [FromQuery] Guid? employeeId) =>
        Ok(await _hrm.GetAttendanceAsync(date, employeeId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Attendance a) => Ok(await _hrm.CreateAttendanceAsync(a));
}
