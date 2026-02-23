using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Operations.Application.Exceptions;
using Pupitre.Operations.Contracts.Commands;
using Pupitre.Operations.Domain.Entities;
using Pupitre.Operations.Domain.Repositories;

namespace Pupitre.Operations.Application.Commands.Handlers;

internal sealed class AddOperationMetricHandler : ICommandHandler<AddOperationMetric>
{
    private readonly IOperationMetricRepository _operationmetricRepository;
    private readonly IEventProcessor _eventProcessor;

    public AddOperationMetricHandler(IOperationMetricRepository operationmetricRepository,
        IEventProcessor eventProcessor)
    {
        _operationmetricRepository = operationmetricRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(AddOperationMetric command, CancellationToken cancellationToken = default)
    {
        
        var operationmetric = await _operationmetricRepository.GetAsync(command.Id);
        
        if(operationmetric is not null)
        {
            throw new OperationMetricAlreadyExistsException(command.Id);
        }

        operationmetric = OperationMetric.Create(command.Id, command.Name ?? string.Empty, tags: command.Tags);
        await _operationmetricRepository.AddAsync(operationmetric, cancellationToken);
        await _eventProcessor.ProcessAsync(operationmetric.Events);
    }
}

