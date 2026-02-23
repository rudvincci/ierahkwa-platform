using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.AIRecommendations.Application.Exceptions;
using Pupitre.AIRecommendations.Contracts.Commands;
using Pupitre.AIRecommendations.Domain.Repositories;

namespace Pupitre.AIRecommendations.Application.Commands.Handlers;

internal sealed class DeleteAIRecommendationHandler : ICommandHandler<DeleteAIRecommendation>
{
    private readonly IAIRecommendationRepository _airecommendationRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteAIRecommendationHandler(IAIRecommendationRepository airecommendationRepository, 
    IEventProcessor eventProcessor)
    {
        _airecommendationRepository = airecommendationRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteAIRecommendation command, CancellationToken cancellationToken = default)
    {
        var airecommendation = await _airecommendationRepository.GetAsync(command.Id, cancellationToken);

        if (airecommendation is null)
        {
            throw new AIRecommendationNotFoundException(command.Id);
        }

        await _airecommendationRepository.DeleteAsync(airecommendation.Id);
        await _eventProcessor.ProcessAsync(airecommendation.Events);
    }
}


