using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.AIAssessments.Application.Exceptions;
using Pupitre.AIAssessments.Contracts.Commands;
using Pupitre.AIAssessments.Domain.Entities;
using Pupitre.AIAssessments.Domain.Repositories;

namespace Pupitre.AIAssessments.Application.Commands.Handlers;

internal sealed class AddAIAssessmentHandler : ICommandHandler<AddAIAssessment>
{
    private readonly IAIAssessmentRepository _aiassessmentRepository;
    private readonly IEventProcessor _eventProcessor;

    public AddAIAssessmentHandler(IAIAssessmentRepository aiassessmentRepository,
        IEventProcessor eventProcessor)
    {
        _aiassessmentRepository = aiassessmentRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(AddAIAssessment command, CancellationToken cancellationToken = default)
    {
        
        var aiassessment = await _aiassessmentRepository.GetAsync(command.Id);
        
        if(aiassessment is not null)
        {
            throw new AIAssessmentAlreadyExistsException(command.Id);
        }

        aiassessment = AIAssessment.Create(command.Id, command.Name ?? string.Empty, tags: command.Tags);
        await _aiassessmentRepository.AddAsync(aiassessment, cancellationToken);
        await _eventProcessor.ProcessAsync(aiassessment.Events);
    }
}

