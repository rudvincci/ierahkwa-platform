using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.AIRecommendations.Application.Exceptions;
using Pupitre.AIRecommendations.Contracts.Commands;
using Pupitre.AIRecommendations.Domain.Entities;
using Pupitre.AIRecommendations.Domain.Repositories;

namespace Pupitre.AIRecommendations.Application.Commands.Handlers;

internal sealed class AddAIRecommendationHandler : ICommandHandler<AddAIRecommendation>
{
    private readonly IAIRecommendationRepository _airecommendationRepository;
    private readonly IEventProcessor _eventProcessor;

    public AddAIRecommendationHandler(IAIRecommendationRepository airecommendationRepository,
        IEventProcessor eventProcessor)
    {
        _airecommendationRepository = airecommendationRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(AddAIRecommendation command, CancellationToken cancellationToken = default)
    {
        
        var airecommendation = await _airecommendationRepository.GetAsync(command.Id);
        
        if(airecommendation is not null)
        {
            throw new AIRecommendationAlreadyExistsException(command.Id);
        }

        airecommendation = AIRecommendation.Create(command.Id, command.Name ?? string.Empty, tags: command.Tags);
        await _airecommendationRepository.AddAsync(airecommendation, cancellationToken);
        await _eventProcessor.ProcessAsync(airecommendation.Events);
    }
}

