using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.GLEs.Application.Exceptions;
using Pupitre.GLEs.Contracts.Commands;
using Pupitre.GLEs.Domain.Repositories;

namespace Pupitre.GLEs.Application.Commands.Handlers;

internal sealed class DeleteGLEHandler : ICommandHandler<DeleteGLE>
{
    private readonly IGLERepository _gleRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteGLEHandler(IGLERepository gleRepository, 
    IEventProcessor eventProcessor)
    {
        _gleRepository = gleRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteGLE command, CancellationToken cancellationToken = default)
    {
        var gle = await _gleRepository.GetAsync(command.Id, cancellationToken);

        if (gle is null)
        {
            throw new GLENotFoundException(command.Id);
        }

        await _gleRepository.DeleteAsync(gle.Id);
        await _eventProcessor.ProcessAsync(gle.Events);
    }
}


