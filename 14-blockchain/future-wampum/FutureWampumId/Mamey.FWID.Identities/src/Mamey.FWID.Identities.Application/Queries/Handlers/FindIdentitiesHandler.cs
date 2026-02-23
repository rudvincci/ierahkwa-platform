using System.Linq.Expressions;
using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Contracts.DTOs;
using Mamey.FWID.Identities.Contracts.Queries;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;

namespace Mamey.FWID.Identities.Application.Queries.Handlers;

/// <summary>
/// Handler for finding identities with filters.
/// </summary>
internal sealed class FindIdentitiesHandler : IQueryHandler<FindIdentities, List<IdentityDto>>
{
    private readonly IIdentityRepository _repository;

    public FindIdentitiesHandler(IIdentityRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<IdentityDto>> HandleAsync(FindIdentities query, CancellationToken cancellationToken = default)
    {
        Expression<Func<Identity, bool>>? predicate = null;

        if (!string.IsNullOrWhiteSpace(query.Zone) || query.Status.HasValue)
        {
            // Convert contract status to domain status for comparison
            var domainStatus = query.Status.HasValue 
                ? (Domain.ValueObjects.IdentityStatus)(int)query.Status.Value 
                : (Domain.ValueObjects.IdentityStatus?)null;
            
            predicate = identity =>
                (string.IsNullOrWhiteSpace(query.Zone) || identity.Zone == query.Zone) &&
                (!domainStatus.HasValue || identity.Status == domainStatus.Value);
        }

        var identities = predicate != null
            ? await _repository.FindAsync(predicate, cancellationToken)
            : await _repository.BrowseAsync(cancellationToken);

        return identities.Select(identity => new IdentityDto
        {
            Id = identity.Id.Value,
            Name = identity.Name,
            Status = (Contracts.IdentityStatus)(int)identity.Status,
            Zone = identity.Zone,
            CreatedAt = identity.CreatedAt,
            VerifiedAt = identity.VerifiedAt,
            LastVerifiedAt = identity.LastVerifiedAt
        }).ToList();
    }
}

