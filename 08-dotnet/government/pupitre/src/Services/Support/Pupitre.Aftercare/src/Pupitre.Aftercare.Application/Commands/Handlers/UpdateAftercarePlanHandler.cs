using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.Aftercare.Application.Exceptions;
using Pupitre.Aftercare.Contracts.Commands;
using Pupitre.Aftercare.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pupitre.Aftercare.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateAftercarePlanHandler : ICommandHandler<UpdateAftercarePlan>
{
    private readonly IAftercarePlanRepository _aftercareplanRepository;
    private readonly IEventProcessor _eventProcessor;

    public UpdateAftercarePlanHandler(
        IAftercarePlanRepository aftercareplanRepository,
        IEventProcessor eventProcessor)
    {
        _aftercareplanRepository = aftercareplanRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(UpdateAftercarePlan command, CancellationToken cancellationToken = default)
    {
        var aftercareplan = await _aftercareplanRepository.GetAsync(command.Id);

        if(aftercareplan is null)
        {
            throw new AftercarePlanNotFoundException(command.Id);
        }

        aftercareplan.Update(command.Name, command.Tags);
        await _aftercareplanRepository.UpdateAsync(aftercareplan);
        await _eventProcessor.ProcessAsync(aftercareplan.Events);
    }
}


