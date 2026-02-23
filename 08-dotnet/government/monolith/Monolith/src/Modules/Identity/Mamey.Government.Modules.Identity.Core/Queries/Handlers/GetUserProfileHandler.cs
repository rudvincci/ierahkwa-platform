using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Identity.Core.Domain.Repositories;
using Mamey.Government.Modules.Identity.Core.DTO;
using Mamey.Government.Modules.Identity.Core.Mappings;
using Mamey.Types;

namespace Mamey.Government.Modules.Identity.Core.Queries.Handlers;

internal sealed class GetUserProfileHandler : IQueryHandler<GetUserProfile, UserProfileDto?>
{
    private readonly IUserProfileRepository _repository;

    public GetUserProfileHandler(IUserProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserProfileDto?> HandleAsync(GetUserProfile query, CancellationToken cancellationToken = default)
    {
        var userProfile = await _repository.GetAsync(new UserId(query.UserId), cancellationToken);
        return userProfile?.AsDto();
    }
}
