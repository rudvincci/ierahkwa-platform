using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Operations.Application.Exceptions;
using Pupitre.Operations.Contracts.Commands;
using Pupitre.Operations.Domain.Repositories;

namespace Pupitre.Operations.Application.Commands.Handlers;

internal sealed class DeleteOperationMetricHandler : ICommandHandler<DeleteOperationMetric>
{
    private readonly IOperationMetricRepository _operationmetricRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteOperationMetricHandler(IOperationMetricRepository operationmetricRepository, 
    IEventProcessor eventProcessor)
    {
        _operationmetricRepository = operationmetricRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteOperationMetric command, CancellationToken cancellationToken = default)
    {
        var operationmetric = await _operationmetricRepository.GetAsync(command.Id, cancellationToken);

        if (operationmetric is null)
        {
            throw new OperationMetricNotFoundException(command.Id);
        }

        await _operationmetricRepository.DeleteAsync(operationmetric.Id);
        await _eventProcessor.ProcessAsync(operationmetric.Events);
    }
}


