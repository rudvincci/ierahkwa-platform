using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.Operations.Application.Exceptions;
using Pupitre.Operations.Contracts.Commands;
using Pupitre.Operations.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pupitre.Operations.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateOperationMetricHandler : ICommandHandler<UpdateOperationMetric>
{
    private readonly IOperationMetricRepository _operationmetricRepository;
    private readonly IEventProcessor _eventProcessor;

    public UpdateOperationMetricHandler(
        IOperationMetricRepository operationmetricRepository,
        IEventProcessor eventProcessor)
    {
        _operationmetricRepository = operationmetricRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(UpdateOperationMetric command, CancellationToken cancellationToken = default)
    {
        var operationmetric = await _operationmetricRepository.GetAsync(command.Id);

        if(operationmetric is null)
        {
            throw new OperationMetricNotFoundException(command.Id);
        }

        operationmetric.Update(command.Name, command.Tags);
        await _operationmetricRepository.UpdateAsync(operationmetric);
        await _eventProcessor.ProcessAsync(operationmetric.Events);
    }
}


