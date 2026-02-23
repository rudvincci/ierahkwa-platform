using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Aftercare.Application.Exceptions;
using Pupitre.Aftercare.Contracts.Commands;
using Pupitre.Aftercare.Domain.Entities;
using Pupitre.Aftercare.Domain.Repositories;

namespace Pupitre.Aftercare.Application.Commands.Handlers;

internal sealed class AddAftercarePlanHandler : ICommandHandler<AddAftercarePlan>
{
    private readonly IAftercarePlanRepository _aftercareplanRepository;
    private readonly IEventProcessor _eventProcessor;

    public AddAftercarePlanHandler(IAftercarePlanRepository aftercareplanRepository,
        IEventProcessor eventProcessor)
    {
        _aftercareplanRepository = aftercareplanRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(AddAftercarePlan command, CancellationToken cancellationToken = default)
    {
        
        var aftercareplan = await _aftercareplanRepository.GetAsync(command.Id);
        
        if(aftercareplan is not null)
        {
            throw new AftercarePlanAlreadyExistsException(command.Id);
        }

        aftercareplan = AftercarePlan.Create(command.Id, command.Name ?? string.Empty, tags: command.Tags);
        await _aftercareplanRepository.AddAsync(aftercareplan, cancellationToken);
        await _eventProcessor.ProcessAsync(aftercareplan.Events);
    }
}

