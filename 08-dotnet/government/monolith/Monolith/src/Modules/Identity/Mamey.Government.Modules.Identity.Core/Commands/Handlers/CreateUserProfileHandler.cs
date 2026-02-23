using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Identity.Core.Domain.Entities;
using Mamey.Government.Modules.Identity.Core.Domain.Repositories;
using Mamey.Government.Modules.Identity.Core.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Modules.Identity.Core.Commands.Handlers;

internal sealed class CreateUserProfileHandler : ICommandHandler<CreateUserProfile>
{
    private readonly IUserProfileRepository _repository;

    public CreateUserProfileHandler(IUserProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(CreateUserProfile command, CancellationToken cancellationToken = default)
    {
        // Check if a profile already exists for this authenticator
        var existing = await _repository.GetByAuthenticatorAsync(
            command.AuthenticatorIssuer, command.AuthenticatorSubject, cancellationToken);
        if (existing is not null)
        {
            throw new UserProfileAlreadyExistsException(command.AuthenticatorIssuer, command.AuthenticatorSubject);
        }

        var userProfile = new UserProfile(
            new UserId(command.Id),
            command.AuthenticatorIssuer,
            command.AuthenticatorSubject,
            command.Email,
            command.DisplayName,
            command.TenantId);

        await _repository.AddAsync(userProfile, cancellationToken);
    }
}
