using Mamey.ApplicationName.Modules.Identity.Core.DTO;
using Mamey.ApplicationName.Modules.Identity.Core.Mappings;
using Mamey.ApplicationName.Modules.Identity.Core.Queries;
using Mamey.Auth.Identity.Entities;
using Mamey.CQRS.Queries;
using Mamey.Persistence.Redis;
using Mamey.Persistence.SQL;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Mamey.ApplicationName.Modules.Identity.Core.EF.Queries;

internal sealed class BrowseUserHandler : PagedQueryBase, IQueryHandler<BrowseUsers, PagedResult<ApplicationUserDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICache _cache;
    public BrowseUserHandler( ICache cache, UserManager<ApplicationUser> userManager)
    {
        _cache = cache;
        _userManager = userManager;
    }

    public async Task<PagedResult<ApplicationUserDto>> HandleAsync(BrowseUsers query,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement Cache
        var users = await _userManager.Users
            .AsNoTracking()
            // .Where(u => 
            //     !string.IsNullOrEmpty(query.FullName) && (query.FullName != null && u.FullName.ToLower().Contains(query.FullName.ToLower())
            //                                               || !string.IsNullOrEmpty(query.Email) && u.Email.ToLower().Contains(query.Email.ToLower())
            //                                               || query.EmailConfirmed.HasValue && u.EmailConfirmed == query.EmailConfirmed
            //                                               || query.LockoutEnabled.HasValue && u.LockoutEnabled == query.LockoutEnabled))
            .Select(u => u.AsDto()).PaginateAsync();

        return users;
        // throw new NotImplementedException();
    }
}