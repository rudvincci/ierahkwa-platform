using SpikeOffice.Core.Entities;

namespace SpikeOffice.Core.Interfaces;

public interface IEmployeeService
{
    Task<List<Employee>> ListAsync(Guid tenantId, CancellationToken ct = default);
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
