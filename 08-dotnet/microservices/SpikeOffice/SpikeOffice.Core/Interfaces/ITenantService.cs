using SpikeOffice.Core.Entities;

namespace SpikeOffice.Core.Interfaces;

public interface ITenantService
{
    Task<Tenant?> GetByUrlPrefixAsync(string urlPrefix, CancellationToken ct = default);
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
