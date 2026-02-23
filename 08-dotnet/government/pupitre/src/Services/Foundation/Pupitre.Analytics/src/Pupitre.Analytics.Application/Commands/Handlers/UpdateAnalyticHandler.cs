using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.Analytics.Application.Exceptions;
using Pupitre.Analytics.Contracts.Commands;
using Pupitre.Analytics.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pupitre.Analytics.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateAnalyticHandler : ICommandHandler<UpdateAnalytic>
{
    private readonly IAnalyticRepository _analyticRepository;
    private readonly IEventProcessor _eventProcessor;

    public UpdateAnalyticHandler(
        IAnalyticRepository analyticRepository,
        IEventProcessor eventProcessor)
    {
        _analyticRepository = analyticRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(UpdateAnalytic command, CancellationToken cancellationToken = default)
    {
        var analytic = await _analyticRepository.GetAsync(command.Id);

        if(analytic is null)
        {
            throw new AnalyticNotFoundException(command.Id);
        }

        analytic.Update(command.Name, command.Tags);
        await _analyticRepository.UpdateAsync(analytic);
        await _eventProcessor.ProcessAsync(analytic.Events);
    }
}


