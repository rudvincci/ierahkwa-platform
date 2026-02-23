using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Aftercare.Application.Exceptions;
using Pupitre.Aftercare.Contracts.Commands;
using Pupitre.Aftercare.Domain.Repositories;

namespace Pupitre.Aftercare.Application.Commands.Handlers;

internal sealed class DeleteAftercarePlanHandler : ICommandHandler<DeleteAftercarePlan>
{
    private readonly IAftercarePlanRepository _aftercareplanRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteAftercarePlanHandler(IAftercarePlanRepository aftercareplanRepository, 
    IEventProcessor eventProcessor)
    {
        _aftercareplanRepository = aftercareplanRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteAftercarePlan command, CancellationToken cancellationToken = default)
    {
        var aftercareplan = await _aftercareplanRepository.GetAsync(command.Id, cancellationToken);

        if (aftercareplan is null)
        {
            throw new AftercarePlanNotFoundException(command.Id);
        }

        await _aftercareplanRepository.DeleteAsync(aftercareplan.Id);
        await _eventProcessor.ProcessAsync(aftercareplan.Events);
    }
}


