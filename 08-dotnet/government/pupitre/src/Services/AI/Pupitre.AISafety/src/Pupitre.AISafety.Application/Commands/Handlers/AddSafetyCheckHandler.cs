using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.AISafety.Application.Exceptions;
using Pupitre.AISafety.Contracts.Commands;
using Pupitre.AISafety.Domain.Entities;
using Pupitre.AISafety.Domain.Repositories;

namespace Pupitre.AISafety.Application.Commands.Handlers;

internal sealed class AddSafetyCheckHandler : ICommandHandler<AddSafetyCheck>
{
    private readonly ISafetyCheckRepository _safetycheckRepository;
    private readonly IEventProcessor _eventProcessor;

    public AddSafetyCheckHandler(ISafetyCheckRepository safetycheckRepository,
        IEventProcessor eventProcessor)
    {
        _safetycheckRepository = safetycheckRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(AddSafetyCheck command, CancellationToken cancellationToken = default)
    {
        
        var safetycheck = await _safetycheckRepository.GetAsync(command.Id);
        
        if(safetycheck is not null)
        {
            throw new SafetyCheckAlreadyExistsException(command.Id);
        }

        safetycheck = SafetyCheck.Create(command.Id, command.Name ?? string.Empty, tags: command.Tags);
        await _safetycheckRepository.AddAsync(safetycheck, cancellationToken);
        await _eventProcessor.ProcessAsync(safetycheck.Events);
    }
}

