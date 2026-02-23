using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Identity.Core.Domain.Repositories;
using Mamey.Government.Modules.Identity.Core.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Modules.Identity.Core.Commands.Handlers;

internal sealed class UpdateUserTenantHandler : ICommandHandler<UpdateUserTenant>
{
    private readonly IUserProfileRepository _repository;

    public UpdateUserTenantHandler(IUserProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(UpdateUserTenant command, CancellationToken cancellationToken = default)
    {
        var userProfile = await _repository.GetAsync(new UserId(command.UserId), cancellationToken);
        if (userProfile is null)
        {
            throw new UserProfileNotFoundException(command.UserId);
        }

        userProfile.UpdateTenant(command.TenantId);
        await _repository.UpdateAsync(userProfile, cancellationToken);
    }
}
