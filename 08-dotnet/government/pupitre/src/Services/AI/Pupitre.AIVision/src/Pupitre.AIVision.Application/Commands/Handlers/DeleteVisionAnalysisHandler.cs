using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.AIVision.Application.Exceptions;
using Pupitre.AIVision.Contracts.Commands;
using Pupitre.AIVision.Domain.Repositories;

namespace Pupitre.AIVision.Application.Commands.Handlers;

internal sealed class DeleteVisionAnalysisHandler : ICommandHandler<DeleteVisionAnalysis>
{
    private readonly IVisionAnalysisRepository _visionanalysisRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteVisionAnalysisHandler(IVisionAnalysisRepository visionanalysisRepository, 
    IEventProcessor eventProcessor)
    {
        _visionanalysisRepository = visionanalysisRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteVisionAnalysis command, CancellationToken cancellationToken = default)
    {
        var visionanalysis = await _visionanalysisRepository.GetAsync(command.Id, cancellationToken);

        if (visionanalysis is null)
        {
            throw new VisionAnalysisNotFoundException(command.Id);
        }

        await _visionanalysisRepository.DeleteAsync(visionanalysis.Id);
        await _eventProcessor.ProcessAsync(visionanalysis.Events);
    }
}


