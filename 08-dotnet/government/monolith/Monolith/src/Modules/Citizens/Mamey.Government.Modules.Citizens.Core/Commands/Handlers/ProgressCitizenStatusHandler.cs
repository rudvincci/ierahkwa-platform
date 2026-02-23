using System;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Citizens.Core.Domain.Repositories;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Citizens.Core.Exceptions;

namespace Mamey.Government.Modules.Citizens.Core.Commands.Handlers;

internal sealed class ProgressCitizenStatusHandler : ICommandHandler<ProgressCitizenStatus>
{
    private readonly ICitizenRepository _repository;

    public ProgressCitizenStatusHandler(ICitizenRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(ProgressCitizenStatus command, CancellationToken cancellationToken = default)
    {
        var citizen = await _repository.GetAsync(new CitizenId(command.CitizenId), cancellationToken);
        if (citizen is null)
        {
            throw new CitizenNotFoundException(command.CitizenId);
        }

        var newStatus = Enum.Parse<CitizenshipStatus>(command.NewStatus, ignoreCase: true);
        
        // Validate status progression
        ValidateStatusProgression(citizen.Status, newStatus);
        
        citizen.ChangeStatus(newStatus, command.Reason);
        
        await _repository.UpdateAsync(citizen, cancellationToken);
    }

    private static void ValidateStatusProgression(CitizenshipStatus current, CitizenshipStatus target)
    {
        var isValid = (current, target) switch
        {
            (CitizenshipStatus.Probationary, CitizenshipStatus.Resident) => true,
            (CitizenshipStatus.Resident, CitizenshipStatus.Citizen) => true,
            _ => false
        };

        if (!isValid)
        {
            throw new InvalidStatusProgressionException(current.ToString(), target.ToString());
        }
    }
}
