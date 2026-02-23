using Common.Domain.Entities;
using Common.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Common.Persistence.Services;

public class TenantService : ITenantService
{
    private readonly CommonDbContext _context;
    private int? _currentTenantId;

    public TenantService(CommonDbContext context)
    {
        _context = context;
    }

    public int? GetCurrentTenantId() => _currentTenantId;

    public async Task<Tenant?> GetCurrentTenantAsync()
    {
        if (_currentTenantId == null) return null;
        return await _context.Tenants.FindAsync(_currentTenantId);
    }

    public void SetTenant(int tenantId)
    {
        _currentTenantId = tenantId;
    }
}
