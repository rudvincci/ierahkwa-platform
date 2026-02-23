using Microsoft.EntityFrameworkCore;
using SpikeOffice.Core.Entities;
using SpikeOffice.Core.Interfaces;
using SpikeOffice.Infrastructure.Data;

namespace SpikeOffice.Infrastructure.Services;

public class EmployeeService : IEmployeeService
{
    private readonly SpikeOfficeDbContext _db;

    public EmployeeService(SpikeOfficeDbContext db)
    {
        _db = db;
    }

    public async Task<List<Employee>> ListAsync(Guid tenantId, CancellationToken ct = default)
    {
        return await _db.Employees
            .Where(e => e.TenantId == tenantId && e.IsActive)
            .Include(e => e.Department)
            .Include(e => e.Designation)
            .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
            .ToListAsync(ct);
    }

    public async Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Employees
            .Include(e => e.Department)
            .Include(e => e.Designation)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }
}
