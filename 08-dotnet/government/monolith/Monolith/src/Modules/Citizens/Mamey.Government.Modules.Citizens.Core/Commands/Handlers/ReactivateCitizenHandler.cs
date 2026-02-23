using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Citizens.Core.Domain.Repositories;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Citizens.Core.Exceptions;

namespace Mamey.Government.Modules.Citizens.Core.Commands.Handlers;

internal sealed class ReactivateCitizenHandler : ICommandHandler<ReactivateCitizen>
{
    private readonly ICitizenRepository _repository;

    public ReactivateCitizenHandler(ICitizenRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(ReactivateCitizen command, CancellationToken cancellationToken = default)
    {
        var citizen = await _repository.GetAsync(new CitizenId(command.CitizenId), cancellationToken);
        if (citizen is null)
        {
            throw new CitizenNotFoundException(command.CitizenId);
        }

        if (citizen.Status != CitizenshipStatus.Suspended)
        {
            throw new InvalidStatusProgressionException(citizen.Status.ToString(), "Reactivated");
        }

        // Determine target status - use provided status or find the last non-suspended status
        var targetStatus = command.TargetStatus;
        if (!targetStatus.HasValue)
        {
            // Find the last valid status before suspension
            var lastValidStatus = citizen.StatusHistory
                .LastOrDefault(h => h.Status != CitizenshipStatus.Suspended && h.Status != CitizenshipStatus.Inactive);
            
            targetStatus = lastValidStatus?.Status ?? CitizenshipStatus.Probationary;
        }

        citizen.ChangeStatus(targetStatus.Value, $"Reactivated by {command.ReactivatedBy}: {command.Reason}");
        
        await _repository.UpdateAsync(citizen, cancellationToken);
    }
}
