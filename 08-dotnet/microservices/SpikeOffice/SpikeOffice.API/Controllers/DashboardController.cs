using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpikeOffice.Core.Interfaces;
using SpikeOffice.Infrastructure.Data;

namespace SpikeOffice.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly SpikeOfficeDbContext _db;
    private readonly ITenantContext _tenant;

    public DashboardController(SpikeOfficeDbContext db, ITenantContext tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    /// <summary>Dashboard stats for current tenant. IERAHKWA: HR summary by department.</summary>
    [HttpGet]
    public async Task<IActionResult> Stats(CancellationToken ct)
    {
        var tid = _tenant.TenantId;
        if (!tid.HasValue)
            return Ok(new { tenant = (object?)null, employees = 0, departments = 0, tasks = 0, pendingLeave = 0 });

        var employees = await _db.Employees.CountAsync(ct);
        var departments = await _db.Departments.CountAsync(ct);
        var tasks = await _db.TaskItems.CountAsync(ct);
        var pendingLeave = await _db.LeaveRequests.CountAsync(l => l.Status == SpikeOffice.Core.Enums.LeaveStatus.Pending, ct);

        return Ok(new
        {
            tenant = new { _tenant.TenantUrlPrefix },
            employees,
            departments,
            tasks,
            pendingLeave,
            ierahkwa = new { integrated = true, chainId = 777777 }
        });
    }
}
