using HRM.Core.Interfaces;
using HRM.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProcurementController : ControllerBase
{
    private readonly IHRMService _hrm;

    public ProcurementController(IHRMService hrm) => _hrm = hrm;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? status) => Ok(await _hrm.GetProcurementsAsync(status));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Procurement p) => Ok(await _hrm.CreateProcurementAsync(p));
}
