using Mamey.Government.Modules.Certificates.Core.Domain.Entities;
using Mamey.Government.Modules.Certificates.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Tenant.Core.Domain.ValueObjects;
using Mamey.Types;
using TenantId = Mamey.Types.TenantId;

namespace Mamey.Government.Modules.Certificates.Core.Domain.Repositories;

internal interface ICertificateRepository
{
    Task<Certificate?> GetAsync(CertificateId id, CancellationToken cancellationToken = default);
    Task AddAsync(Certificate certificate, CancellationToken cancellationToken = default);
    Task UpdateAsync(Certificate certificate, CancellationToken cancellationToken = default);
    Task DeleteAsync(CertificateId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(CertificateId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Certificate>> BrowseAsync(CancellationToken cancellationToken = default);
    
    // Lookup methods
    Task<Certificate?> GetByCertificateNumberAsync(string certificateNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Certificate>> GetByCitizenAsync(CitizenId citizenId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Certificate>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Certificate>> GetByTypeAsync(CertificateType certificateType, CancellationToken cancellationToken = default);
}
