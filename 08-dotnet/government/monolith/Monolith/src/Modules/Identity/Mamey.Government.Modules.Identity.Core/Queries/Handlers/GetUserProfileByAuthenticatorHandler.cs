using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Identity.Core.Domain.Repositories;
using Mamey.Government.Modules.Identity.Core.DTO;
using Mamey.Government.Modules.Identity.Core.Mappings;

namespace Mamey.Government.Modules.Identity.Core.Queries.Handlers;

internal sealed class GetUserProfileByAuthenticatorHandler : IQueryHandler<GetUserProfileByAuthenticator, UserProfileDto?>
{
    private readonly IUserProfileRepository _repository;

    public GetUserProfileByAuthenticatorHandler(IUserProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserProfileDto?> HandleAsync(GetUserProfileByAuthenticator query, CancellationToken cancellationToken = default)
    {
        var userProfile = await _repository.GetByAuthenticatorAsync(query.Issuer, query.Subject, cancellationToken);
        return userProfile?.AsDto();
    }
}
