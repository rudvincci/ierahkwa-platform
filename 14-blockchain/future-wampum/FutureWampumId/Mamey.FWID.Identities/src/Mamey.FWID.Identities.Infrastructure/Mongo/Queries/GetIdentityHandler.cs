using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Contracts.DTOs;
using Mamey.FWID.Identities.Contracts.Queries;
using Mamey.FWID.Identities.Domain.Repositories;
using GetIdentityQuery = Mamey.FWID.Identities.Contracts.Queries.GetIdentity;

namespace Mamey.FWID.Identities.Infrastructure.Mongo.Queries;

/// <summary>
/// MongoDB query handler for getting an identity by identifier.
/// Uses composite repository pattern (Redis → Mongo → Postgres).
/// </summary>
internal sealed class GetIdentityHandler : IQueryHandler<GetIdentityQuery, IdentityDto>
{
    private readonly IIdentityRepository _repository;

    public GetIdentityHandler(IIdentityRepository repository)
    {
        _repository = repository;
    }

    public async Task<IdentityDto> HandleAsync(GetIdentityQuery query, CancellationToken cancellationToken = default)
    {
        var identity = await _repository.GetAsync(query.IdentityId, cancellationToken);
        
        if (identity == null)
        {
            return null!; // API will return 404
        }

        // Convert entity to DTO
        return new IdentityDto
        {
            Id = identity.Id,
            Name = identity.Name,
            Status = (Contracts.IdentityStatus)(int)identity.Status,
            Zone = identity.Zone,
            CreatedAt = identity.CreatedAt,
            VerifiedAt = identity.VerifiedAt,
            LastVerifiedAt = identity.LastVerifiedAt
        };
    }
}


