using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Contracts.DTOs;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;
using GetIdentityQuery = Mamey.FWID.Identities.Contracts.Queries.GetIdentity;

namespace Mamey.FWID.Identities.Application.Queries.Handlers;

/// <summary>
/// Handler for getting an identity by identifier.
/// </summary>
internal sealed class GetIdentityHandler : IQueryHandler<GetIdentityQuery, IdentityDto>
{
    private readonly IIdentityRepository _repository;
    private readonly IMemoryCache _cache;

    public GetIdentityHandler(
        IIdentityRepository repository,
        IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<IdentityDto> HandleAsync(GetIdentityQuery query, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"identity:{query.IdentityId}";

        if (_cache.TryGetValue(cacheKey, out IdentityDto? cachedIdentity) && cachedIdentity != null)
        {
            return cachedIdentity;
        }

        var identityId = new IdentityId(query.IdentityId);
        var identity = await _repository.GetAsync(identityId, cancellationToken);
        if (identity == null)
            throw new IdentityNotFoundException(identityId);

        var dto = new IdentityDto
        {
            Id = identity.Id.Value,
            Name = identity.Name,
            Status = (Contracts.IdentityStatus)(int)identity.Status,
            Zone = identity.Zone,
            CreatedAt = identity.CreatedAt,
            VerifiedAt = identity.VerifiedAt,
            LastVerifiedAt = identity.LastVerifiedAt
        };

        _cache.Set(cacheKey, dto, TimeSpan.FromMinutes(5));
        return dto;
    }
}

