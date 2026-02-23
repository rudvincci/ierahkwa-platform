using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.AITutors.Application.Exceptions;
using Pupitre.AITutors.Contracts.Commands;
using Pupitre.AITutors.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pupitre.AITutors.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateTutorHandler : ICommandHandler<UpdateTutor>
{
    private readonly ITutorRepository _tutorRepository;
    private readonly IEventProcessor _eventProcessor;

    public UpdateTutorHandler(
        ITutorRepository tutorRepository,
        IEventProcessor eventProcessor)
    {
        _tutorRepository = tutorRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(UpdateTutor command, CancellationToken cancellationToken = default)
    {
        var tutor = await _tutorRepository.GetAsync(command.Id);

        if(tutor is null)
        {
            throw new TutorNotFoundException(command.Id);
        }

        tutor.Update(command.Name, command.Tags);
        await _tutorRepository.UpdateAsync(tutor);
        await _eventProcessor.ProcessAsync(tutor.Events);
    }
}


