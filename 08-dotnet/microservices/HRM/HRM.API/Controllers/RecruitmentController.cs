using HRM.Core.Interfaces;
using HRM.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecruitmentController : ControllerBase
{
    private readonly IHRMService _hrm;

    public RecruitmentController(IHRMService hrm) => _hrm = hrm;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? status) => Ok(await _hrm.GetRecruitmentsAsync(status));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Recruitment r) => Ok(await _hrm.CreateRecruitmentAsync(r));
}
