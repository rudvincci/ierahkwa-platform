using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Queries;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.Queries.Handlers;

internal sealed class GetIdentityPermissionsHandler : IQueryHandler<GetIdentityPermissions, List<Guid>>
{
    private readonly IPermissionService _permissionService;

    public GetIdentityPermissionsHandler(IPermissionService permissionService)
    {
        _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
    }

    public async Task<List<Guid>> HandleAsync(GetIdentityPermissions query, CancellationToken cancellationToken = default)
    {
        var identityId = new IdentityId(query.IdentityId);
        var permissions = await _permissionService.GetIdentityPermissionsAsync(identityId, cancellationToken);
        return permissions.Select(p => p.Value).ToList();
    }
}

