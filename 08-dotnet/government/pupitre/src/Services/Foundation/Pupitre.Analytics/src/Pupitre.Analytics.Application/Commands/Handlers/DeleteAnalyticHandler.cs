using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Analytics.Application.Exceptions;
using Pupitre.Analytics.Contracts.Commands;
using Pupitre.Analytics.Domain.Repositories;

namespace Pupitre.Analytics.Application.Commands.Handlers;

internal sealed class DeleteAnalyticHandler : ICommandHandler<DeleteAnalytic>
{
    private readonly IAnalyticRepository _analyticRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteAnalyticHandler(IAnalyticRepository analyticRepository, 
    IEventProcessor eventProcessor)
    {
        _analyticRepository = analyticRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteAnalytic command, CancellationToken cancellationToken = default)
    {
        var analytic = await _analyticRepository.GetAsync(command.Id, cancellationToken);

        if (analytic is null)
        {
            throw new AnalyticNotFoundException(command.Id);
        }

        await _analyticRepository.DeleteAsync(analytic.Id);
        await _eventProcessor.ProcessAsync(analytic.Events);
    }
}


