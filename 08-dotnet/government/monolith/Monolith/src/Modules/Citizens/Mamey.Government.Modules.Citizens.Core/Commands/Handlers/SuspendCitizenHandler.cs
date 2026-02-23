using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Citizens.Core.Domain.Repositories;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Citizens.Core.Exceptions;

namespace Mamey.Government.Modules.Citizens.Core.Commands.Handlers;

internal sealed class SuspendCitizenHandler : ICommandHandler<SuspendCitizen>
{
    private readonly ICitizenRepository _repository;

    public SuspendCitizenHandler(ICitizenRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(SuspendCitizen command, CancellationToken cancellationToken = default)
    {
        var citizen = await _repository.GetAsync(new CitizenId(command.CitizenId), cancellationToken);
        if (citizen is null)
        {
            throw new CitizenNotFoundException(command.CitizenId);
        }

        if (citizen.Status == CitizenshipStatus.Inactive)
        {
            throw new InvalidStatusProgressionException(citizen.Status.ToString(), CitizenshipStatus.Suspended.ToString());
        }

        citizen.ChangeStatus(CitizenshipStatus.Suspended, $"Suspended by {command.SuspendedBy}: {command.Reason}");
        
        await _repository.UpdateAsync(citizen, cancellationToken);
    }
}
