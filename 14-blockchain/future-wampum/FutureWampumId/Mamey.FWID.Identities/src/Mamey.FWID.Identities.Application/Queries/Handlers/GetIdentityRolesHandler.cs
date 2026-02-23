using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Queries;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Types;

namespace Mamey.FWID.Identities.Application.Queries.Handlers;

internal sealed class GetIdentityRolesHandler : IQueryHandler<GetIdentityRoles, List<Guid>>
{
    private readonly IRoleService _roleService;

    public GetIdentityRolesHandler(IRoleService roleService)
    {
        _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
    }

    public async Task<List<Guid>> HandleAsync(GetIdentityRoles query, CancellationToken cancellationToken = default)
    {
        var identityId = new IdentityId(query.IdentityId);
        var roles = await _roleService.GetIdentityRolesAsync(identityId, cancellationToken);
        return roles.Select(r => r.Value).ToList();
    }
}

