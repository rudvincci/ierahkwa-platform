using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Ministries.Application.Exceptions;
using Pupitre.Ministries.Contracts.Commands;
using Pupitre.Ministries.Domain.Repositories;

namespace Pupitre.Ministries.Application.Commands.Handlers;

internal sealed class DeleteMinistryDataHandler : ICommandHandler<DeleteMinistryData>
{
    private readonly IMinistryDataRepository _ministrydataRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteMinistryDataHandler(IMinistryDataRepository ministrydataRepository, 
    IEventProcessor eventProcessor)
    {
        _ministrydataRepository = ministrydataRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteMinistryData command, CancellationToken cancellationToken = default)
    {
        var ministrydata = await _ministrydataRepository.GetAsync(command.Id, cancellationToken);

        if (ministrydata is null)
        {
            throw new MinistryDataNotFoundException(command.Id);
        }

        await _ministrydataRepository.DeleteAsync(ministrydata.Id);
        await _eventProcessor.ProcessAsync(ministrydata.Events);
    }
}


