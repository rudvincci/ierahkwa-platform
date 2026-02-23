using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.AIAssessments.Application.Exceptions;
using Pupitre.AIAssessments.Contracts.Commands;
using Pupitre.AIAssessments.Domain.Repositories;

namespace Pupitre.AIAssessments.Application.Commands.Handlers;

internal sealed class DeleteAIAssessmentHandler : ICommandHandler<DeleteAIAssessment>
{
    private readonly IAIAssessmentRepository _aiassessmentRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteAIAssessmentHandler(IAIAssessmentRepository aiassessmentRepository, 
    IEventProcessor eventProcessor)
    {
        _aiassessmentRepository = aiassessmentRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteAIAssessment command, CancellationToken cancellationToken = default)
    {
        var aiassessment = await _aiassessmentRepository.GetAsync(command.Id, cancellationToken);

        if (aiassessment is null)
        {
            throw new AIAssessmentNotFoundException(command.Id);
        }

        await _aiassessmentRepository.DeleteAsync(aiassessment.Id);
        await _eventProcessor.ProcessAsync(aiassessment.Events);
    }
}


