using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Pupitre.Accessibility.Application.Exceptions;
using Pupitre.Accessibility.Contracts.Commands;
using Pupitre.Accessibility.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pupitre.Accessibility.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateAccessProfileHandler : ICommandHandler<UpdateAccessProfile>
{
    private readonly IAccessProfileRepository _accessprofileRepository;
    private readonly IEventProcessor _eventProcessor;

    public UpdateAccessProfileHandler(
        IAccessProfileRepository accessprofileRepository,
        IEventProcessor eventProcessor)
    {
        _accessprofileRepository = accessprofileRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(UpdateAccessProfile command, CancellationToken cancellationToken = default)
    {
        var accessprofile = await _accessprofileRepository.GetAsync(command.Id);

        if(accessprofile is null)
        {
            throw new AccessProfileNotFoundException(command.Id);
        }

        accessprofile.Update(command.Name, command.Tags);
        await _accessprofileRepository.UpdateAsync(accessprofile);
        await _eventProcessor.ProcessAsync(accessprofile.Events);
    }
}


