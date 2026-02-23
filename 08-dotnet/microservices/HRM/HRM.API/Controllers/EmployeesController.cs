using HRM.Core.Interfaces;
using HRM.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IHRMService _hrm;

    public EmployeesController(IHRMService hrm) => _hrm = hrm;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _hrm.GetEmployeesAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var e = await _hrm.GetEmployeeAsync(id);
        return e is null ? NotFound() : Ok(e);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Employee e) => Ok(await _hrm.CreateEmployeeAsync(e));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Employee e)
    {
        var u = await _hrm.UpdateEmployeeAsync(id, e);
        return u is null ? NotFound() : Ok(u);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) =>
        await _hrm.DeleteEmployeeAsync(id) ? NoContent() : NotFound();
}
