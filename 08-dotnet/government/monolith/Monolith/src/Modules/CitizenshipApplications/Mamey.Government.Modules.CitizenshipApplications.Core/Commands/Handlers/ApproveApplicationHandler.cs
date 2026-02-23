using Mamey.CQRS.Commands;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;
using Mamey.Government.Modules.CitizenshipApplications.Core.Events;
using Mamey.Government.Modules.CitizenshipApplications.Core.Exceptions;
using Mamey.Government.Modules.CitizenshipApplications.Core.Services;
using Mamey.MicroMonolith.Abstractions.Messaging;
using AppId = Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects.ApplicationId;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Commands.Handlers;

internal sealed class ApproveApplicationHandler : ICommandHandler<ApproveApplication>
{
    private readonly IApplicationRepository _repository;
    private readonly IContext _context;
    private readonly IMessageBroker _messageBroker;

    public ApproveApplicationHandler(
        IApplicationRepository repository,
        IContext context,
        IMessageBroker messageBroker)
    {
        _repository = repository;
        _context = context;
        _messageBroker = messageBroker;
    }

    public async Task HandleAsync(ApproveApplication command, CancellationToken cancellationToken = default)
    {
        var application = await _repository.GetAsync(new AppId(command.ApplicationId), cancellationToken);
        if (application is null)
        {
            throw new ApplicationNotFoundException(command.ApplicationId);
        }

        // Get the approver from the authenticated user context
        var approvedBy = _context.UserId;
        application.Approve(approvedBy);

        await _repository.UpdateAsync(application, cancellationToken);
        _messageBroker.PublishAsync(new ApplicationApprovedEvent(application.Id, application.ApplicationNumber, application.ApprovedBy, (DateTime)application.ApprovedAt!));
    }
}
