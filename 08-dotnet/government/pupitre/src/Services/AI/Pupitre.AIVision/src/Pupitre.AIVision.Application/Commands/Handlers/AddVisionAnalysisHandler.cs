using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.AIVision.Application.Exceptions;
using Pupitre.AIVision.Contracts.Commands;
using Pupitre.AIVision.Domain.Entities;
using Pupitre.AIVision.Domain.Repositories;

namespace Pupitre.AIVision.Application.Commands.Handlers;

internal sealed class AddVisionAnalysisHandler : ICommandHandler<AddVisionAnalysis>
{
    private readonly IVisionAnalysisRepository _visionanalysisRepository;
    private readonly IEventProcessor _eventProcessor;

    public AddVisionAnalysisHandler(IVisionAnalysisRepository visionanalysisRepository,
        IEventProcessor eventProcessor)
    {
        _visionanalysisRepository = visionanalysisRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(AddVisionAnalysis command, CancellationToken cancellationToken = default)
    {
        
        var visionanalysis = await _visionanalysisRepository.GetAsync(command.Id);
        
        if(visionanalysis is not null)
        {
            throw new VisionAnalysisAlreadyExistsException(command.Id);
        }

        visionanalysis = VisionAnalysis.Create(command.Id, command.Name ?? string.Empty, tags: command.Tags);
        await _visionanalysisRepository.AddAsync(visionanalysis, cancellationToken);
        await _eventProcessor.ProcessAsync(visionanalysis.Events);
    }
}

