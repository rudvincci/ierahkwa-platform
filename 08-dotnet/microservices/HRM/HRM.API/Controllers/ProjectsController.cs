using HRM.Core.Interfaces;
using HRM.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IHRMService _hrm;

    public ProjectsController(IHRMService hrm) => _hrm = hrm;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? status) => Ok(await _hrm.GetProjectsAsync(status));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Project p) => Ok(await _hrm.CreateProjectAsync(p));
}
