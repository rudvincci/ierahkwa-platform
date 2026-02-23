using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.AIRecommendations.Application.Exceptions;
using Pupitre.AIRecommendations.Contracts.Commands;
using Pupitre.AIRecommendations.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pupitre.AIRecommendations.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateAIRecommendationHandler : ICommandHandler<UpdateAIRecommendation>
{
    private readonly IAIRecommendationRepository _airecommendationRepository;
    private readonly IEventProcessor _eventProcessor;

    public UpdateAIRecommendationHandler(
        IAIRecommendationRepository airecommendationRepository,
        IEventProcessor eventProcessor)
    {
        _airecommendationRepository = airecommendationRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(UpdateAIRecommendation command, CancellationToken cancellationToken = default)
    {
        var airecommendation = await _airecommendationRepository.GetAsync(command.Id);

        if(airecommendation is null)
        {
            throw new AIRecommendationNotFoundException(command.Id);
        }

        airecommendation.Update(command.Name, command.Tags);
        await _airecommendationRepository.UpdateAsync(airecommendation);
        await _eventProcessor.ProcessAsync(airecommendation.Events);
    }
}


