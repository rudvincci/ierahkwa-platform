using Microsoft.AspNetCore.Mvc;
using SpikeOffice.Core.Interfaces;

namespace SpikeOffice.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _svc;
    private readonly ITenantContext _tenant;

    public EmployeesController(IEmployeeService svc, ITenantContext tenant)
    {
        _svc = svc;
        _tenant = tenant;
    }

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        if (!_tenant.TenantId.HasValue)
            return BadRequest(new { error = "Tenant required. Use /t/{urlPrefix}/api/employees or X-Tenant-Prefix / ?tenant=" });
        var list = await _svc.ListAsync(_tenant.TenantId.Value, ct);
        return Ok(list.Select(e => new
        {
            e.Id, e.EmployeeCode, e.FirstName, e.LastName, e.Email, DepartmentName = e.Department?.Name, DesignationName = e.Designation?.Name, e.BasicSalary, e.IsActive
        }));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var e = await _svc.GetByIdAsync(id, ct);
        if (e == null) return NotFound();
        return Ok(new
        {
            e.Id, e.EmployeeCode, e.FirstName, e.LastName, e.Email, e.Phone, e.JoinDate, e.Department, e.Designation,
            e.BasicSalary, e.BankAccount, e.Address, e.IsActive
        });
    }
}
