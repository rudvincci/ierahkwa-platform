using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.AISafety.Application.Exceptions;
using Pupitre.AISafety.Contracts.Commands;
using Pupitre.AISafety.Domain.Repositories;

namespace Pupitre.AISafety.Application.Commands.Handlers;

internal sealed class DeleteSafetyCheckHandler : ICommandHandler<DeleteSafetyCheck>
{
    private readonly ISafetyCheckRepository _safetycheckRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteSafetyCheckHandler(ISafetyCheckRepository safetycheckRepository, 
    IEventProcessor eventProcessor)
    {
        _safetycheckRepository = safetycheckRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteSafetyCheck command, CancellationToken cancellationToken = default)
    {
        var safetycheck = await _safetycheckRepository.GetAsync(command.Id, cancellationToken);

        if (safetycheck is null)
        {
            throw new SafetyCheckNotFoundException(command.Id);
        }

        await _safetycheckRepository.DeleteAsync(safetycheck.Id);
        await _eventProcessor.ProcessAsync(safetycheck.Events);
    }
}


