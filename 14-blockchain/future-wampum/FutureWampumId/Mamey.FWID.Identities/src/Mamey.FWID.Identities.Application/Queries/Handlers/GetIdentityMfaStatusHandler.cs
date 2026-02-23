using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Queries;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;

namespace Mamey.FWID.Identities.Application.Queries.Handlers;

internal sealed class GetIdentityMfaStatusHandler : IQueryHandler<GetIdentityMfaStatus, MfaStatusDto>
{
    private readonly IIdentityRepository _identityRepository;
    private readonly IMfaConfigurationRepository _mfaConfigurationRepository;

    public GetIdentityMfaStatusHandler(
        IIdentityRepository identityRepository,
        IMfaConfigurationRepository mfaConfigurationRepository)
    {
        _identityRepository = identityRepository ?? throw new ArgumentNullException(nameof(identityRepository));
        _mfaConfigurationRepository = mfaConfigurationRepository ?? throw new ArgumentNullException(nameof(mfaConfigurationRepository));
    }

    public async Task<MfaStatusDto> HandleAsync(GetIdentityMfaStatus query, CancellationToken cancellationToken = default)
    {
        var identityId = new IdentityId(query.IdentityId);
        var identity = await _identityRepository.GetAsync(identityId, cancellationToken);
        if (identity == null)
            throw new InvalidOperationException("Identity not found");

        var configs = await _mfaConfigurationRepository.GetByIdentityIdAsync(identityId, cancellationToken);
        var enabledConfigs = configs.Where(c => c.IsEnabled).ToList();

        return new MfaStatusDto
        {
            IsEnabled = identity.MultiFactorEnabled,
            PreferredMethod = identity.PreferredMfaMethod.HasValue 
                ? (Contracts.MfaMethod)(int)identity.PreferredMfaMethod.Value 
                : null,
            EnabledMethods = enabledConfigs.Select(c => (Contracts.MfaMethod)(int)c.Method).ToList()
        };
    }
}

