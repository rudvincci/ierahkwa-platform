using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.AIAssessments.Application.Exceptions;
using Pupitre.AIAssessments.Contracts.Commands;
using Pupitre.AIAssessments.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pupitre.AIAssessments.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateAIAssessmentHandler : ICommandHandler<UpdateAIAssessment>
{
    private readonly IAIAssessmentRepository _aiassessmentRepository;
    private readonly IEventProcessor _eventProcessor;

    public UpdateAIAssessmentHandler(
        IAIAssessmentRepository aiassessmentRepository,
        IEventProcessor eventProcessor)
    {
        _aiassessmentRepository = aiassessmentRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(UpdateAIAssessment command, CancellationToken cancellationToken = default)
    {
        var aiassessment = await _aiassessmentRepository.GetAsync(command.Id);

        if(aiassessment is null)
        {
            throw new AIAssessmentNotFoundException(command.Id);
        }

        aiassessment.Update(command.Name, command.Tags);
        await _aiassessmentRepository.UpdateAsync(aiassessment);
        await _eventProcessor.ProcessAsync(aiassessment.Events);
    }
}


