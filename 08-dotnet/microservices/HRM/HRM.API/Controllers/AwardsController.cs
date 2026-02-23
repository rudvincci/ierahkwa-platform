using HRM.Core.Interfaces;
using HRM.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AwardsController : ControllerBase
{
    private readonly IHRMService _hrm;

    public AwardsController(IHRMService hrm) => _hrm = hrm;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid? employeeId) => Ok(await _hrm.GetAwardsAsync(employeeId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Award a) => Ok(await _hrm.CreateAwardAsync(a));
}
