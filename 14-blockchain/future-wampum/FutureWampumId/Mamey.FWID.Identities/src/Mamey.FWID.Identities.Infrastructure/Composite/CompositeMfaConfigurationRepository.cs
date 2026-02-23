using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Infrastructure.EF.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Infrastructure.Composite;

/// <summary>
/// Composite repository for MfaConfiguration entities.
/// Currently uses PostgreSQL directly; MongoDB/Redis can be added later.
/// </summary>
internal class CompositeMfaConfigurationRepository : IMfaConfigurationRepository
{
    private readonly MfaConfigurationPostgresRepository _postgresRepo;
    private readonly ILogger<CompositeMfaConfigurationRepository> _logger;

    public CompositeMfaConfigurationRepository(
        MfaConfigurationPostgresRepository postgresRepo,
        ILogger<CompositeMfaConfigurationRepository> logger)
    {
        _postgresRepo = postgresRepo ?? throw new ArgumentNullException(nameof(postgresRepo));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task AddAsync(MfaConfiguration entity, CancellationToken cancellationToken = default)
        => _postgresRepo.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(MfaConfiguration entity, CancellationToken cancellationToken = default)
        => _postgresRepo.UpdateAsync(entity, cancellationToken);

    public Task DeleteAsync(MfaConfigurationId id, CancellationToken cancellationToken = default)
        => _postgresRepo.DeleteAsync(id, cancellationToken);

    public Task<MfaConfiguration?> GetAsync(MfaConfigurationId id, CancellationToken cancellationToken = default)
        => _postgresRepo.GetAsync(id, cancellationToken);

    public Task<bool> ExistsAsync(MfaConfigurationId id, CancellationToken cancellationToken = default)
        => _postgresRepo.ExistsAsync(id, cancellationToken);

    public Task<MfaConfiguration?> GetByIdentityAndMethodAsync(IdentityId identityId, MfaMethod method, CancellationToken cancellationToken = default)
        => _postgresRepo.GetByIdentityAndMethodAsync(identityId, method, cancellationToken);

    public Task<IReadOnlyList<MfaConfiguration>> GetByIdentityIdAsync(IdentityId identityId, CancellationToken cancellationToken = default)
        => _postgresRepo.GetByIdentityIdAsync(identityId, cancellationToken);

    public Task<IReadOnlyList<MfaConfiguration>> FindAsync(System.Linq.Expressions.Expression<Func<MfaConfiguration, bool>> predicate, CancellationToken cancellationToken = default)
        => _postgresRepo.FindAsync(predicate, cancellationToken);
}

