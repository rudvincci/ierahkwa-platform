using HRM.Core.Interfaces;
using HRM.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly IHRMService _hrm;

    public LoansController(IHRMService hrm) => _hrm = hrm;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid? employeeId) => Ok(await _hrm.GetLoansAsync(employeeId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Loan l) => Ok(await _hrm.CreateLoanAsync(l));
}
