using Mamey.Government.Modules.Certificates.Core.Domain.Entities;
using Mamey.Government.Modules.Certificates.Core.Domain.Repositories;
using Mamey.Government.Modules.Certificates.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Types;
using Mamey.Persistence.Redis;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Certificates.Core.Redis.Repositories;

internal class CertificateRedisRepository : ICertificateRepository
{
    private readonly ICache _cache;
    private readonly ILogger<CertificateRedisRepository> _logger;
    private const string CertificatePrefix = "certificates:certificates:";

    public CertificateRedisRepository(
        ICache cache,
        ILogger<CertificateRedisRepository> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<Certificate?> GetAsync(CertificateId id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cache.GetAsync<Certificate>($"{CertificatePrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get certificate from Redis: {CertificateId}", id.Value);
            return null;
        }
    }

    public async Task AddAsync(Certificate certificate, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.SetAsync($"{CertificatePrefix}{certificate.Id.Value}", certificate, TimeSpan.FromHours(1));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to add certificate to Redis: {CertificateId}", certificate.Id.Value);
        }
    }

    public async Task UpdateAsync(Certificate certificate, CancellationToken cancellationToken = default)
    {
        await AddAsync(certificate, cancellationToken);
    }

    public async Task DeleteAsync(CertificateId id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.DeleteAsync<Certificate>($"{CertificatePrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete certificate from Redis: {CertificateId}", id.Value);
        }
    }

    public async Task<bool> ExistsAsync(CertificateId id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cache.KeyExistsAsync($"{CertificatePrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check certificate existence in Redis: {CertificateId}", id.Value);
            return false;
        }
    }

    public Task<IReadOnlyList<Certificate>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Certificate>>(new List<Certificate>());
    }

    public Task<Certificate?> GetByCertificateNumberAsync(string certificateNumber, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<Certificate?>(null);
    }

    public Task<IReadOnlyList<Certificate>> GetByCitizenAsync(CitizenId citizenId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Certificate>>(new List<Certificate>());
    }

    public Task<IReadOnlyList<Certificate>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Certificate>>(new List<Certificate>());
    }

    public Task<IReadOnlyList<Certificate>> GetByTypeAsync(CertificateType certificateType, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Certificate>>(new List<Certificate>());
    }
}
