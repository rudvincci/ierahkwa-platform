using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Analytics.Application.Exceptions;
using Pupitre.Analytics.Contracts.Commands;
using Pupitre.Analytics.Domain.Entities;
using Pupitre.Analytics.Domain.Repositories;

namespace Pupitre.Analytics.Application.Commands.Handlers;

internal sealed class AddAnalyticHandler : ICommandHandler<AddAnalytic>
{
    private readonly IAnalyticRepository _analyticRepository;
    private readonly IEventProcessor _eventProcessor;

    public AddAnalyticHandler(IAnalyticRepository analyticRepository,
        IEventProcessor eventProcessor)
    {
        _analyticRepository = analyticRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(AddAnalytic command, CancellationToken cancellationToken = default)
    {
        
        var analytic = await _analyticRepository.GetAsync(command.Id);
        
        if(analytic is not null)
        {
            throw new AnalyticAlreadyExistsException(command.Id);
        }

        analytic = Analytic.Create(command.Id, command.Name ?? string.Empty, tags: command.Tags);
        await _analyticRepository.AddAsync(analytic, cancellationToken);
        await _eventProcessor.ProcessAsync(analytic.Events);
    }
}

