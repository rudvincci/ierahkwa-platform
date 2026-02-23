using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Citizens.Core.Domain.Repositories;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Citizens.Core.Exceptions;

namespace Mamey.Government.Modules.Citizens.Core.Commands.Handlers;

internal sealed class DeactivateCitizenHandler : ICommandHandler<DeactivateCitizen>
{
    private readonly ICitizenRepository _repository;

    public DeactivateCitizenHandler(ICitizenRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(DeactivateCitizen command, CancellationToken cancellationToken = default)
    {
        var citizen = await _repository.GetAsync(new CitizenId(command.CitizenId), cancellationToken);
        if (citizen is null)
        {
            throw new CitizenNotFoundException(command.CitizenId);
        }

        citizen.Deactivate(command.Reason ?? "No reason provided");
        citizen.ChangeStatus(CitizenshipStatus.Inactive, command.Reason);
        
        await _repository.UpdateAsync(citizen, cancellationToken);
    }
}
