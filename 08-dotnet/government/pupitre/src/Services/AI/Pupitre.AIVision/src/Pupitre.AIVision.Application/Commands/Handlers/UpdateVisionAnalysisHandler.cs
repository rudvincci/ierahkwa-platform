using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.AIVision.Application.Exceptions;
using Pupitre.AIVision.Contracts.Commands;
using Pupitre.AIVision.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pupitre.AIVision.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateVisionAnalysisHandler : ICommandHandler<UpdateVisionAnalysis>
{
    private readonly IVisionAnalysisRepository _visionanalysisRepository;
    private readonly IEventProcessor _eventProcessor;

    public UpdateVisionAnalysisHandler(
        IVisionAnalysisRepository visionanalysisRepository,
        IEventProcessor eventProcessor)
    {
        _visionanalysisRepository = visionanalysisRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(UpdateVisionAnalysis command, CancellationToken cancellationToken = default)
    {
        var visionanalysis = await _visionanalysisRepository.GetAsync(command.Id);

        if(visionanalysis is null)
        {
            throw new VisionAnalysisNotFoundException(command.Id);
        }

        visionanalysis.Update(command.Name, command.Tags);
        await _visionanalysisRepository.UpdateAsync(visionanalysis);
        await _eventProcessor.ProcessAsync(visionanalysis.Events);
    }
}


