using Mamey.Portal.Citizenship.Application.Models;

namespace Mamey.Portal.Citizenship.Application.Services;

public interface IApplicationStatusStore
{
    Task<ApplicationStatusDto?> GetByNumberAsync(string tenantId, string applicationNumber, CancellationToken ct = default);
    Task<ApplicationStatusDto?> GetByIdAsync(string tenantId, Guid applicationId, CancellationToken ct = default);
    Task<ApplicationStatusDto?> GetByEmailAsync(string tenantId, string email, CancellationToken ct = default);
}
