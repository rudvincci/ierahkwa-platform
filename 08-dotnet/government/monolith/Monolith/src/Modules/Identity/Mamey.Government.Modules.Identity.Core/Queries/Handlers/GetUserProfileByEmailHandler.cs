using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Identity.Core.Domain.Repositories;
using Mamey.Government.Modules.Identity.Core.DTO;
using Mamey.Government.Modules.Identity.Core.Mappings;

namespace Mamey.Government.Modules.Identity.Core.Queries.Handlers;

internal sealed class GetUserProfileByEmailHandler : IQueryHandler<GetUserProfileByEmail, UserProfileDto?>
{
    private readonly IUserProfileRepository _repository;

    public GetUserProfileByEmailHandler(IUserProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserProfileDto?> HandleAsync(GetUserProfileByEmail query, CancellationToken cancellationToken = default)
    {
        var userProfile = await _repository.GetByEmailAsync(query.Email, cancellationToken);
        return userProfile?.AsDto();
    }
}
