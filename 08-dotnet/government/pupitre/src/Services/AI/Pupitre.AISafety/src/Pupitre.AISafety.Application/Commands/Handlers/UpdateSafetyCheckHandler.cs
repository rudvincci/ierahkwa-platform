using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.AISafety.Application.Exceptions;
using Pupitre.AISafety.Contracts.Commands;
using Pupitre.AISafety.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pupitre.AISafety.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateSafetyCheckHandler : ICommandHandler<UpdateSafetyCheck>
{
    private readonly ISafetyCheckRepository _safetycheckRepository;
    private readonly IEventProcessor _eventProcessor;

    public UpdateSafetyCheckHandler(
        ISafetyCheckRepository safetycheckRepository,
        IEventProcessor eventProcessor)
    {
        _safetycheckRepository = safetycheckRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(UpdateSafetyCheck command, CancellationToken cancellationToken = default)
    {
        var safetycheck = await _safetycheckRepository.GetAsync(command.Id);

        if(safetycheck is null)
        {
            throw new SafetyCheckNotFoundException(command.Id);
        }

        safetycheck.Update(command.Name, command.Tags);
        await _safetycheckRepository.UpdateAsync(safetycheck);
        await _eventProcessor.ProcessAsync(safetycheck.Events);
    }
}


