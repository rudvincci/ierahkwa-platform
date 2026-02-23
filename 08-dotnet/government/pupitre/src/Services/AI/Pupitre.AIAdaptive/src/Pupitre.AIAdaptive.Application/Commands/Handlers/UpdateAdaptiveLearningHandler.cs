using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.AIAdaptive.Application.Exceptions;
using Pupitre.AIAdaptive.Contracts.Commands;
using Pupitre.AIAdaptive.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pupitre.AIAdaptive.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateAdaptiveLearningHandler : ICommandHandler<UpdateAdaptiveLearning>
{
    private readonly IAdaptiveLearningRepository _adaptivelearningRepository;
    private readonly IEventProcessor _eventProcessor;

    public UpdateAdaptiveLearningHandler(
        IAdaptiveLearningRepository adaptivelearningRepository,
        IEventProcessor eventProcessor)
    {
        _adaptivelearningRepository = adaptivelearningRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(UpdateAdaptiveLearning command, CancellationToken cancellationToken = default)
    {
        var adaptivelearning = await _adaptivelearningRepository.GetAsync(command.Id);

        if(adaptivelearning is null)
        {
            throw new AdaptiveLearningNotFoundException(command.Id);
        }

        adaptivelearning.Update(command.Name, command.Tags);
        await _adaptivelearningRepository.UpdateAsync(adaptivelearning);
        await _eventProcessor.ProcessAsync(adaptivelearning.Events);
    }
}


