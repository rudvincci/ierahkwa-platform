using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Identity.Core.Domain.Repositories;
using Mamey.Government.Modules.Identity.Core.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Modules.Identity.Core.Commands.Handlers;

internal sealed class UpdateUserProfileHandler : ICommandHandler<UpdateUserProfile>
{
    private readonly IUserProfileRepository _repository;

    public UpdateUserProfileHandler(IUserProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(UpdateUserProfile command, CancellationToken cancellationToken = default)
    {
        var userProfile = await _repository.GetAsync(new UserId(command.UserId), cancellationToken);
        if (userProfile is null)
        {
            throw new UserProfileNotFoundException(command.UserId);
        }

        userProfile.UpdateProfile(command.Email ?? userProfile.Email, command.DisplayName ?? userProfile.DisplayName);
        await _repository.UpdateAsync(userProfile, cancellationToken);
    }
}
