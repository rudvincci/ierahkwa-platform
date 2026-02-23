using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Educators.Application.Exceptions;
using Pupitre.Educators.Contracts.Commands;
using Pupitre.Educators.Domain.Repositories;

namespace Pupitre.Educators.Application.Commands.Handlers;

internal sealed class DeleteEducatorHandler : ICommandHandler<DeleteEducator>
{
    private readonly IEducatorRepository _educatorRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteEducatorHandler(IEducatorRepository educatorRepository, 
    IEventProcessor eventProcessor)
    {
        _educatorRepository = educatorRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteEducator command, CancellationToken cancellationToken = default)
    {
        var educator = await _educatorRepository.GetAsync(command.Id, cancellationToken);

        if (educator is null)
        {
            throw new EducatorNotFoundException(command.Id);
        }

        await _educatorRepository.DeleteAsync(educator.Id);
        await _eventProcessor.ProcessAsync(educator.Events);
    }
}


