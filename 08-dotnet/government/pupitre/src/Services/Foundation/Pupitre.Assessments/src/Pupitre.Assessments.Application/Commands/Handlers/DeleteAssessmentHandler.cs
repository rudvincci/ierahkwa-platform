using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Assessments.Application.Exceptions;
using Pupitre.Assessments.Contracts.Commands;
using Pupitre.Assessments.Domain.Repositories;

namespace Pupitre.Assessments.Application.Commands.Handlers;

internal sealed class DeleteAssessmentHandler : ICommandHandler<DeleteAssessment>
{
    private readonly IAssessmentRepository _assessmentRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteAssessmentHandler(IAssessmentRepository assessmentRepository, 
    IEventProcessor eventProcessor)
    {
        _assessmentRepository = assessmentRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteAssessment command, CancellationToken cancellationToken = default)
    {
        var assessment = await _assessmentRepository.GetAsync(command.Id, cancellationToken);

        if (assessment is null)
        {
            throw new AssessmentNotFoundException(command.Id);
        }

        await _assessmentRepository.DeleteAsync(assessment.Id);
        await _eventProcessor.ProcessAsync(assessment.Events);
    }
}


